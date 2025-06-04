using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TituloChatTxt : MonoBehaviour
{
    public Text txtNombre;

    public void AbrirChat()
    {
        GestorChatTxtUI.singleton.CrearSubChat(txtNombre.text);
    }
}
