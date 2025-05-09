using System;
using System.Collections.Generic;
using UnityEngine;
using Best.WebSockets;

[AddComponentMenu("Morion Servidor/Servidor")]
[RequireComponent(typeof(GestionMensajesServidor))]
public class Servidor : MonoBehaviour
{
    public delegate void Evento();
    [HideInInspector]
    public string   url             = "ws://127.0.0.1:8080";
    public string   puerto          = "8080";
    public bool     debugEnPantalla = false;
    public bool     debugEnConsola  = false;
    public bool     autoconectar    = false;

    [ConditionalHide("debugEnPantalla", true)]
    public UnityEngine.UI.Text txtDebug;
    WebSocket       ws;
    [HideInInspector]
    public GestionMensajesServidor gestorMensajes;
    public Evento   EventoConectado;
    public static Servidor singleton;
    [HideInInspector]
    public string   estado          = "((inactivo))";
    public bool     conectado       = false;


    private void Awake()
    {
        singleton       = this;
        gestorMensajes  = GetComponent<GestionMensajesServidor>();
        EventoConectado += Vacio;
    }

    void Vacio()
    {
        return;
    }

    public string GetURL()
	{
        return "ws://" +
                ConfiguracionGeneral.configuracionDefault.GetIP() +
                ":" +
                puerto;
    }

    private void Start()
    {
        url = GetURL();
        if(autoconectar) Conectar();
    }
    public void CambairURL(string n_url)
    {
        url = n_url;
    }

    public void Conectar()
    {
        estado = "(( Conectando... ))";
        var webSocket = new WebSocket(new Uri(url));
        ws = webSocket;
        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnClosed += OnWebSocketClosed;
        webSocket.Open();

    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        if(debugEnConsola) Debug.Log("Websocket abierto!");
        estado = "(( <color=#00ff00> Conectado :D </color> ))";
        if (txtDebug != null)
        {
            txtDebug.text += "\n" + ("Websocket abierto!");
        }
        conectado = true;
        if (EventoConectado != null)
            EventoConectado();
        //Presentacion p = ControlUsuario.singleton.GetPresentacion();
        //string pJson = JsonUtility.ToJson(p);

        //webSocket.Send("PR00" + pJson);
        webSocket.Send("AC00 ");
    }

    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        if (debugEnConsola) Debug.Log("Mensaje recibido: " + message);
        if (txtDebug != null)
		{
            txtDebug.text += "\n" + ("Mensaje recibido: " + message);
            if (txtDebug.text.Length > 500)
            {
                txtDebug.text = txtDebug.text.Substring(txtDebug.text.Length - 455);
            }
        }
        if (gestorMensajes != null)
        {
            gestorMensajes.RecibirMensaje(message.Substring(2));
        }
    }

    private void OnWebSocketClosed(WebSocket webSocket, WebSocketStatusCodes code, string message)
    {
        if (debugEnConsola) Debug.Log("WebSocket Cerrado D: !");
        
        conectado = false;
        if (code == WebSocketStatusCodes.NormalClosure)
        {
            estado = "(( <color=#800000> Desconectado normalmente </color> ))";
        }
        else
        {
            estado = "(( <color=#ff0000> Desconectado por error D: </color> ))";
        }
    }


    public void EnviarMensaje(string msj)
    {
        ws.Send(msj);
    }
}
