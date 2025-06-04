using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Morion Servidor/Chat/UI Mensajes")]
public class MensajeChatUI : MonoBehaviour
{
    public Text txtNombre;
    public Image img;
    public Text txtMsj;
    public GameObject espacio1;
    public GameObject espacio2;
    bool esPropio;
    string nombre;

    public void Inicializar(MensajeChat msj)
    {
        txtNombre.text  = msj.nombreUsuario;
        txtMsj.text     = msj.msj;
    }
    public void Inicializar(MensajeChat msj, bool propio)
    {
        Inicializar(msj);
        espacio1.SetActive(propio);
        espacio2.SetActive(!propio);
        esPropio = propio;
        nombre = msj.nombreUsuario;
        if (propio)
        {
            img.color = new Color(0.3f, 1, 0.3f);
        }
    }

    public void EntrarPrivado()
    {
        if (!esPropio)
        {
            GestorChatTxtUI.singleton.CrearSubChat(nombre);
        }
    }
}
