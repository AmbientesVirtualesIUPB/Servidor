using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorChatTxtUI : MonoBehaviour
{
    public static GestorChatTxtUI   singleton;
    public Transform        padre;
    public GameObject       prChat;
    public string           destinatarioSubChat = "all";
    public Dictionary<string, ChatTxtUI> diccionarioChats = new Dictionary<string, ChatTxtUI>();
    public ChatTxtUI        general;

    public GameObject       botonChats;
    public Transform        padreBotonesChats;


    private void Awake()
    {
        singleton = this;
        if (padre == null)
        {
            padre = transform;
        }
    }

    /// <summary>
    /// Sirve para mostrar u ocultar un chat, por nombre de usuario
    /// </summary>
    /// <param name="nombre"></param>
    public void MostrarOcultar(string nombre)
    {
        if (nombre == "all")
        {
            general.gameObject.SetActive(!general.gameObject.activeSelf);
        }
        else
        {
            if (diccionarioChats.ContainsKey(nombre))
            {
                diccionarioChats[nombre].gameObject.SetActive(!diccionarioChats[nombre].gameObject.activeSelf);
            }
        }
    }

    private void Start()
    {
        if (GestionChatTxt.singleton != null)
        {
            GestionChatTxt.singleton.recibirMensaje += RecibirMensaje;
        }
        else
        {
            Debug.LogError("No se encuentra el gestor de mensajes como singleton en las escenas activas.");
        }
    }

    /// <summary>
    /// Un subchat es un chat interno con un usuario especifico
    /// </summary>
    /// <param name="destinatario"></param>
    public void CrearSubChat(string destinatario)
    {
        if (diccionarioChats.ContainsKey(destinatario) && diccionarioChats[destinatario] == null)
        {
            diccionarioChats.Remove(destinatario);
        }
        if (!diccionarioChats.ContainsKey(destinatario))
        {
            destinatarioSubChat = destinatario;
            GameObject sct = Instantiate(prChat, padre);
            ChatTxtUI cti = sct.GetComponent<ChatTxtUI>();
            cti.destinatario = destinatarioSubChat;
            cti.Limpiar();
            cti.Inicializar(destinatarioSubChat);
            diccionarioChats.Add(destinatario, cti);
            BtnChatMostrar btncm = Instantiate(botonChats, padreBotonesChats).GetComponent<BtnChatMostrar>();
            btncm.nombre = destinatario;
        }
        else
        {
            diccionarioChats[destinatario].gameObject.SetActive(true);
        }

    }
    /// <summary>
    /// Qué hacer cuando se reciba un mensaje desde el servidor de node
    /// </summary>
    MensajeChat msj_bk;
    public void RecibirMensaje(MensajeChat msj)
    {
        if (msj_bk!=msj && msj.destinatario.Equals(GestionChatTxt.singleton.nombreUsuario))
        {
            CrearSubChat(msj.nombreUsuario);

            if (diccionarioChats.ContainsKey(msj.nombreUsuario) && diccionarioChats[msj.nombreUsuario].numeroMsjs < 1)
            {
                diccionarioChats[msj.nombreUsuario].RecibirMensaje(msj);
            }
        }

    }
}
