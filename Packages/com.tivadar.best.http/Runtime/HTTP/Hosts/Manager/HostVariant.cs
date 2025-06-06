using Best.HTTP.Hosts.Connections;
using Best.HTTP.Hosts.Connections.File;
using Best.HTTP.Hosts.Settings;
using Best.HTTP.Shared;
using Best.HTTP.Shared.Extensions;
using Best.HTTP.Shared.Logger;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Best.HTTP.HostSetting
{
    /// <summary>
    /// An enumeration representing the protocol support for a host.
    /// </summary>
    public enum HostProtocolSupport : byte
    {
        /// <summary>
        /// Protocol support is unknown or undetermined.
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// The host supports HTTP/1.
        /// </summary>
        HTTP1 = 0x01,

        /// <summary>
        /// The host supports HTTP/2.
        /// </summary>
        HTTP2 = 0x02,

        /// <summary>
        /// This is a file-based host.
        /// </summary>
        File = 0x03,
    }

    /// <summary>
    /// <para>The HostVariant class is a critical component in managing HTTP connections and handling HTTP requests for a specific host. It maintains a queue of requests and a list of active connections associated with the host, ensuring efficient utilization of available resources. Additionally, it supports protocol version detection (HTTP/1 or HTTP/2) for optimized communication with the host.</para>
    /// <list type="bullet">
    ///     <item><description>It maintains a queue of requests to ensure efficient and controlled use of available connections.</description></item>
    ///     <item><description>It supports HTTP/1 and HTTP/2 protocol versions, allowing requests to be sent using the appropriate protocol based on the host's protocol support.</description></item>
    ///     <item><description>Provides methods for sending requests, recycling connections, managing connection state, and handling the shutdown of connections and the host variant itself.</description></item>
    ///     <item><description>It includes logging for diagnostic purposes, helping to monitor and debug the behavior of connections and requests.</description></item>
    /// </list>
    /// <para>In summary, the HostVariant class plays a central role in managing HTTP connections and requests for a specific host, ensuring efficient and reliable communication with that host while supporting different protocol versions.</para>
    /// </summary>
    public class HostVariant
    {
        public HostKey Host { get; private set; }

        public HostProtocolSupport ProtocolSupport { get; private set; }

        public DateTime LastProtocolSupportUpdate { get; private set; }

        public LoggingContext Context { get; private set; }

        // All the connections. Free and processing ones too.
        protected List<ConnectionBase> Connections = new List<ConnectionBase>();

        // Queued requests that aren't passed yet to a connection.
        protected Queue<HTTPRequest> Queue = new Queue<HTTPRequest>();

        // Host-variant settings
        protected HostVariantSettings _settings;

        // Cached list
        protected List<KeyValuePair<int, ConnectionBase>> availableConnections;

        public HostVariant(HostKey host)
        {
            this.Host = host;
            if (this.Host.Uri.IsFile)
                this.ProtocolSupport = HostProtocolSupport.File;

            this.Context = new LoggingContext(this);
            this.Context.Add("Host", this.Host.Host);

            this._settings = HTTPManager.PerHostSettings.Get(this).HostVariantSettings;
            this.availableConnections = new List<KeyValuePair<int, ConnectionBase>>(2);
        }

        public virtual void AddProtocol(HostProtocolSupport protocolSupport)
        {
            this.LastProtocolSupportUpdate = HTTPManager.CurrentFrameDateTime;

            var oldProtocol = this.ProtocolSupport;

            if (oldProtocol != protocolSupport)
            {
                this.ProtocolSupport = protocolSupport;

                HTTPManager.Logger.Information(nameof(HostVariant), $"AddProtocol({oldProtocol} => {protocolSupport})", this.Context);
            }

            TryToSendQueuedRequests();
        }

        public virtual HostVariant Send(HTTPRequest request)
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HostVariant), $"Send({request})", this.Context);

            request.Context.Remove(nameof(HostVariant));
            request.Context.Add(nameof(HostVariant), this.Context);

            this.Queue.Enqueue(request);
            return TryToSendQueuedRequests();
        }

        public virtual HostVariant TryToSendQueuedRequests()
        {
            if (this.Queue.Count == 0)
                return this;

            (int activeConnections, int theoreticalMaximumPerConnection, int assignedRequests) = QueryAnyAvailableOrNew(ref availableConnections);

            if (availableConnections.Count == 0)
            {
#if !UNITY_WEBGL || UNITY_EDITOR
                if (activeConnections > 0 && this.ProtocolSupport == HostProtocolSupport.Unknown)
                    return this;
#endif

                if (activeConnections < this._settings.MaxConnectionPerVariant)
                {
                    int queueSize = this.Queue.Count;
                    int currentMaximum = (activeConnections * theoreticalMaximumPerConnection) - assignedRequests;

                    while (activeConnections < this._settings.MaxConnectionPerVariant && currentMaximum < queueSize)
                    {
                        availableConnections.Add(new KeyValuePair<int, ConnectionBase>(0, CreateNew()));
                        currentMaximum += theoreticalMaximumPerConnection;
                        activeConnections++;
                    }
                }
                else
                    return this;
            }

            // Sort connections by theirs key (assigned requests count)
            availableConnections.Sort((a, b) => a.Key - b.Key);

            while (this.Queue.Count > 0 && availableConnections.Count > 0)
            {
                var nextRequest = this.Queue.Peek();

                // If the queue is large, or timeouts are set low, a request might be in a queue while its state is set to > Finished.
                //  So we have to prevent sending it again.

                if (nextRequest.State <= HTTPRequestStates.Queued)
                {
                    var kvp = availableConnections[0];

                    if (HTTPManager.Logger.IsDiagnostic)
                    {
	                    nextRequest.Context.Remove(nameof(HostVariant));
	                    nextRequest.Context.Add(nameof(HostVariant), this.Context);

                        var key = kvp.Value.GetType().Name;
                        nextRequest.Context.Add(key, kvp.Value.Context);

                        HTTPManager.Logger.Information(nameof(HostVariant), $"Send({nextRequest.Context.Hash}, {key} => {kvp.Value.Context.Hash})", nextRequest.Context);
                    }

                    RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(nextRequest, HTTPRequestStates.Processing, null));

                    OnConnectionStartedProcessingRequest(kvp.Value, nextRequest);
                    // then start process the request
                    kvp.Value.Process(nextRequest);

                    if (kvp.Key + 1 >= kvp.Value.MaxAssignedRequests)
                        availableConnections.RemoveAt(0);
                    else
                        availableConnections[0] = new KeyValuePair<int, ConnectionBase>(kvp.Key + 1, kvp.Value);

                    availableConnections.Sort((a, b) => a.Key - b.Key);
                }

                this.Queue.Dequeue();
            }

            return this;
        }

        public virtual (int activeConnections, int theoreticalMaximumPerConnection, int assignedRequests) QueryAnyAvailableOrNew(ref List<KeyValuePair<int, ConnectionBase>> connectionCollector)
        {
            int activeConnections = 0;
            int maxAssignedRequest = 1;
            int assignedRequests = 0;

            connectionCollector.Clear();

            // Check the last created connection first. This way, if a higher level protocol is present that can handle more requests (== HTTP/2) that protocol will be chosen
            //  and others will be closed when their inactivity time is reached.
            for (int i = Connections.Count - 1; i >= 0; --i)
            {
                var conn = Connections[i];

                if (conn.State == HTTPConnectionStates.Initial ||
                    conn.State == HTTPConnectionStates.Free ||
                    (conn.CanProcessMultiple && conn.AssignedRequests < conn.MaxAssignedRequests))
                    connectionCollector.Add(new KeyValuePair<int, ConnectionBase>(conn.AssignedRequests, conn));

                maxAssignedRequest = Math.Max(maxAssignedRequest, conn.MaxAssignedRequests);
                assignedRequests += conn.AssignedRequests;

                activeConnections++;
            }

            return (activeConnections, Math.Max(1, (int)(maxAssignedRequest * this._settings.MaxAssignedRequestsFactor)), assignedRequests);
        }

        public virtual ConnectionBase CreateNew()
        {
            if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HostVariant), $"CreateNew({this.Host})", this.Context);

            ConnectionBase conn = this._settings.ConnectionFactory?.Invoke(this._settings, this);

            if (conn == null)
            {
                if (this.ProtocolSupport == HostProtocolSupport.File)
                    conn = new FileConnection(this.Host);
                else
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    conn = new Best.HTTP.Hosts.Connections.WebGL.WebGLXHRConnection(this.Host);
#else
                    conn = new HTTPOverTCPConnection(this.Host);
#endif
                }
            }

            Connections.Add(conn);

            return conn;
        }

        protected virtual void OnConnectionStartedProcessingRequest(ConnectionBase connection, HTTPRequest request)
        {

        }

        public virtual HostVariant RecycleConnection(ConnectionBase conn)
        {
            conn.State = HTTPConnectionStates.Free;

            Best.HTTP.Shared.Extensions.Timer.Add(new TimerData(TimeSpan.FromSeconds(1), conn, CloseConnectionAfterInactivity));

            return this;
        }

        protected virtual bool RemoveConnectionImpl(ConnectionBase conn, HTTPConnectionStates setState)
        {
            HTTPManager.Logger.Information(typeof(HostVariant).Name, $"RemoveConnectionImpl({conn}, {setState})", this.Context);

            conn.State = setState;
            conn.Dispose();

            bool found = this.Connections.Remove(conn);

            if (!found) // 
                HTTPManager.Logger.Information(typeof(HostVariant).Name, $"RemoveConnectionImpl - Couldn't find connection! key: {conn.HostKey}", this.Context);

            return found;
        }

        public virtual HostVariant RemoveConnection(ConnectionBase conn, HTTPConnectionStates setState)
        {
            RemoveConnectionImpl(conn, setState);

            return this;
        }

        public ConnectionBase Find(Predicate<ConnectionBase> match) => this.Connections.Find(match);

        public bool HasConnection(ConnectionBase connection) => this.Connections.Contains(connection);

        protected virtual bool CloseConnectionAfterInactivity(DateTime now, object context)
        {
            var conn = context as ConnectionBase;

            bool closeConnection = conn.State == HTTPConnectionStates.Free && now - conn.LastProcessTime >= conn.KeepAliveTime;
            if (closeConnection)
            {
                HTTPManager.Logger.Information(typeof(HostVariant).Name, string.Format("CloseConnectionAfterInactivity - [{0}] Closing! State: {1}, Now: {2}, LastProcessTime: {3}, KeepAliveTime: {4}",
                    conn.ToString(), conn.State, now.ToString(System.Globalization.CultureInfo.InvariantCulture), conn.LastProcessTime.ToString(System.Globalization.CultureInfo.InvariantCulture), conn.KeepAliveTime), this.Context);

                RemoveConnection(conn, HTTPConnectionStates.Closed);
                return false;
            }

            // repeat until the connection's state is free
            return conn.State == HTTPConnectionStates.Free;
        }

        public virtual void RemoveAllIdleConnections()
        {
            for (int i = 0; i < this.Connections.Count; i++)
                if (this.Connections[i].State == HTTPConnectionStates.Free)
                {
                    int countBefore = this.Connections.Count;
                    RemoveConnection(this.Connections[i], HTTPConnectionStates.Closed);

                    if (countBefore != this.Connections.Count)
                        i--;
                }
        }

        public virtual void Shutdown()
        {
            this.Queue.Clear();

            foreach (var conn in this.Connections)
            {
                // Swallow any exceptions, we are quitting anyway.
                try
                {
                    conn.Shutdown(ShutdownTypes.Immediate);
                }
                catch { }
            }
            //this.Connections.Clear();
        }

        public virtual void SaveTo(System.IO.BinaryWriter bw)
        {
            bw.Write(this.LastProtocolSupportUpdate.ToBinary());
            bw.Write((byte)this.ProtocolSupport);
        }

        public virtual void LoadFrom(int version, System.IO.BinaryReader br)
        {
            this.LastProtocolSupportUpdate = DateTime.FromBinary(br.ReadInt64());
            this.ProtocolSupport = (HostProtocolSupport)br.ReadByte();

            if (DateTime.UtcNow - this.LastProtocolSupportUpdate >= TimeSpan.FromDays(1))
            {
                if (HTTPManager.Logger.IsDiagnostic)
                    HTTPManager.Logger.Verbose(nameof(HostVariant), $"LoadFrom - Too Old! LastProtocolSupportUpdate: {this.LastProtocolSupportUpdate.ToString(CultureInfo.InvariantCulture)}, ProtocolSupport: {this.ProtocolSupport}", this.Context);
                this.ProtocolSupport = HostProtocolSupport.Unknown;
            }
            else if (HTTPManager.Logger.IsDiagnostic)
                HTTPManager.Logger.Verbose(nameof(HostVariant), $"LoadFrom - LastProtocolSupportUpdate: {this.LastProtocolSupportUpdate.ToString(CultureInfo.InvariantCulture)}, ProtocolSupport: {this.ProtocolSupport}", this.Context);
        }

        public override string ToString() => $"{this.Host}, {this.Queue.Count}/{this.Connections?.Count}, {this.ProtocolSupport}";
    }
}
