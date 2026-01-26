using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BtnUsuarioLista : MonoBehaviour
{
    public Image imgBoton;
    public TextMeshProUGUI txtBoton;
    public string idUsuario;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicializar(string nombre, string id)
    {
        idUsuario = id;
        txtBoton.text = nombre;
        txtBoton.rectTransform.anchoredPosition = Vector2.zero;
        txtBoton.rectTransform.anchoredPosition3D = Vector3.zero;
        imgBoton.color = Color.white;
        txtBoton.color = Color.black;
    }

    public void Activar()
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnMotor", 0.7f); // Ejecutamos el efecto nombrado

        GestionMensajesServidor.singeton.EnviarMensaje("MS09",idUsuario);

        ServidorMotores.singleton.ActivarMecanico(idUsuario);
    }
}
