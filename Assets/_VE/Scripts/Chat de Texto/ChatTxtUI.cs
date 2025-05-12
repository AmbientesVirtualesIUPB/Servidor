using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Morion Servidor/Chat/Gestor UI")]
public class ChatTxtUI : MonoBehaviour
{
    public GameObject prMensaje;
    public Transform padre;
    public MensajeChat msj;
    public InputField inpMensaje;


    public MessageOnly mensaje = new MessageOnly("Para funcionar se requiere que algun elemento tenga el GestionChatTxt", MessageTypeCustom.Info);

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

    public void RecibirMensaje(MensajeChat msj)
    {
        GameObject nuevoMSJ = Instantiate(prMensaje, padre);
        MensajeChatUI msjUI = nuevoMSJ.GetComponent<MensajeChatUI>();
        if (msjUI != null)
        {
            msjUI.Inicializar(msj, false);
        }
        nuevoMSJ.SetActive(true);
    }

    public void EnviarMensaje()
    {
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

    }

    void EnviarMensaje(string msj)
    {
        GestionChatTxt.singleton.EnviarMensaje(msj);
    }
}
