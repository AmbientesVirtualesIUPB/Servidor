using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorChatTxtUI : MonoBehaviour
{
    public static GestorChatTxtUI singleton;
    public Transform padre;
    public GameObject prChat;
    public string destinatarioSubChat = "all";
    public Dictionary<string, ChatTxtUI> diccionarioChats = new Dictionary<string, ChatTxtUI>();

    private void Awake()
    {
        singleton = this;
        if (padre == null)
        {
            padre = transform;
        }
    }

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
            diccionarioChats.Add(destinatario, cti);
        }
        else
        {
            diccionarioChats[destinatario].gameObject.SetActive(true);
        }

    }

}
