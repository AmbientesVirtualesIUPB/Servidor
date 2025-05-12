using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Morion Servidor/Chat/UI Mensajes")]
public class MensajeChatUI : MonoBehaviour
{
    public Text txtNombre;
    public Text txtMsj;

    public void Inicializar(MensajeChat msj)
    {
        txtNombre.text  = msj.nombreUsuario;
        txtMsj.text     = msj.msj;
    }
}
