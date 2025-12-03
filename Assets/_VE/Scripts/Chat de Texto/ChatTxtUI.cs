using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("Morion Servidor/Chat/Gestor UI")]
public class ChatTxtUI : MonoBehaviour
{
    public GameObject   prMensaje;
    public GameObject   prespacio;
    public Transform    padre;
    public MensajeChat  msj;
    public TMP_InputField inpMensaje;
    public string       destinatario = "all";
    public int          numeroMsjs = 0;
    public Text         txtTitulo;



    public MessageOnly  mensaje = new MessageOnly("Para funcionar se requiere que algun elemento tenga el GestionChatTxt", MessageTypeCustom.Info);

    // Start is called before the first frame update
    void Start()
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

    public void Inicializar(string nombre)
    {
        txtTitulo.text = nombre;
    }

    public void RecibirMensaje(MensajeChat msj)
    {
        if (destinatario.Equals(msj.destinatario) && destinatario.Equals("all") || (msj.destinatario.Equals(GestionChatTxt.singleton.nombreUsuario)&&destinatario.Equals(msj.nombreUsuario)))
        {
            GameObject nuevoMSJ = Instantiate(prMensaje, padre);
            MensajeChatUI msjUI = nuevoMSJ.GetComponent<MensajeChatUI>();
            if (msjUI != null)
            {
                msjUI.Inicializar(msj, false);
            }
            nuevoMSJ.SetActive(true);
            Invoke("Repintar", 0.01f);
            numeroMsjs++;
        }

    }


    public void Limpiar()
    {
        Transform[] lista = padre.GetComponentsInChildren<Transform>();
        for (int i = 0; i < lista.Length; i++)
        {
            if (padre.transform != lista[i].transform && lista[i].gameObject.activeSelf && lista[i].gameObject.name.Substring(0,1) != "E")
            {
                Destroy(lista[i].gameObject);
            }
        }
    }

    void Repintar()
    {
        Destroy(Instantiate(prespacio, padre),0.1f);
    }

    public void EnviarMensaje()
    {
        if (inpMensaje.text.Length < 1) return;
        MensajeChat msj = new MensajeChat();
        msj.nombreUsuario = "yo";
        msj.msj = inpMensaje.text;

        EnviarMensaje(inpMensaje.text);
        inpMensaje.text = "";


        GameObject nuevoMSJ = Instantiate(prMensaje, padre);
        MensajeChatUI msjUI = nuevoMSJ.GetComponent<MensajeChatUI>();
        if (msjUI != null)
        {
            msjUI.Inicializar(msj, true);
        }
        nuevoMSJ.SetActive(true);

        Invoke("Repintar", 0.01f);

    }

    void EnviarMensaje(string msj)
    {
        GestionChatTxt.singleton.EnviarMensaje(msj, destinatario);
    }
}
