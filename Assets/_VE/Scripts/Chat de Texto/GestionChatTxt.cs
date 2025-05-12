using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Morion Servidor/Chat/Gestion Chat de texto")]
public class GestionChatTxt : MonoBehaviour
{
    public bool debugEnConsola = false;
    public string nombreUsuario;
    public List<MensajeChat> mensajes = new List<MensajeChat>();
    public RecibirMensajeDelegate recibirMensaje;

    public delegate void RecibirMensajeDelegate(MensajeChat msj);

    public static GestionChatTxt singleton;

    /// Registrar funciones en el servidor
    private void Start()
    {
        GestionMensajesServidor.singeton.RegistrarAccion("CDT00", RecibirMensaje);
        recibirMensaje += ImprimirMensaje;

        //Pendiente terminar la estructura completa del singleton
        singleton = this;
    }

    public void Inicializar(string _nombreUsuario)
    {
        nombreUsuario = _nombreUsuario;
    }

    public void EnviarMensaje(string msj)
    {
        MensajeChat msjChat = new MensajeChat();
        msjChat.nombreUsuario = nombreUsuario;
        msjChat.msj = msj;
        GestionMensajesServidor.singeton.EnviarMensaje(JsonUtility.ToJson(msj));
    }

    public void RecibirMensaje(string msj)
    {
        MensajeChat msjChat = JsonUtility.FromJson<MensajeChat>(msj);
        mensajes.Add(msjChat);
        recibirMensaje(msjChat);
    }

    void ImprimirMensaje(MensajeChat msj)
    {
        if (debugEnConsola)
        {
            print("Mensaje Recibido: [" + msj.nombreUsuario + "] = " + msj.msj);
        }
    }
}

[System.Serializable]
public class MensajeChat
{
    public string id;
    public string nombreUsuario;
    public string msj;
}
