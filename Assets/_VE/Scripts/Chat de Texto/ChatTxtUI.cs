using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Morion Servidor/Chat/Gestor UI")]
public class ChatTxtUI : MonoBehaviour
{
    public GameObject prMensaje;
    public Transform padre;
    public MensajeChat msj;
    public MessageOnly mensaje = new MessageOnly("Para que este componente funcione se requiere que algun elemento tenga el GestionChatTxt, que es un singleton", MessageTypeCustom.Info);

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
            msjUI.Inicializar(msj);
        }
    }
}
