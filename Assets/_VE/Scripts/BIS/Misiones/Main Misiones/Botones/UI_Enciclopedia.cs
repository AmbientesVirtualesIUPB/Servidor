using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Enciclopedia : MonoBehaviour
{
    public static UI_Enciclopedia instancia;

    [Header("Panel con la información del objeto")]
    public GameObject panel;

    [Header("Elementos UI")]
    public TMP_Text textoTitulo;
    public TMP_Text textoDescripcion;
    public Image imagenIcono;

    [Header("Texto mostrado cuando la info está bloqueada")]
    [TextArea]
    public string textoCodificado = "Información clasificada...\nAnaliza este objeto para desbloquearla.";

    private void Awake()
    {
        instancia = this;
        if (panel != null)
            panel.SetActive(false);
    }

    // ----------------------------------------------------------------------
    // Muestra info decodificada si está desbloqueada, o codificada si no
    // ----------------------------------------------------------------------
    public void MostrarObjeto(ObjetoMisionBase obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("[UI_Enciclopedia] Objeto nulo.");
            return;
        }

        if (panel != null)
            panel.SetActive(true);

        // TÍTULO
        if (textoTitulo != null)
            textoTitulo.text = obj.nombreObjeto;

        // Comprobar GestorObjetos
        bool infoDesbloqueada = false;
        if (GestorObjetos.instancia != null)
        {
            infoDesbloqueada = GestorObjetos.instancia.InfoDesbloqueada(obj);
        }
        else
        {
            Debug.LogWarning("[UI_Enciclopedia] No hay GestorObjetos en la escena. Se mostrará texto codificado.");
        }

        Debug.Log($"[Enciclopedia] Mostrando {obj.nombreObjeto} | Desbloqueada: {infoDesbloqueada} | Desc: '{obj.descripcion}'");

        // DESCRIPCIÓN
        if (textoDescripcion != null)
            textoDescripcion.text = infoDesbloqueada ? obj.descripcion : textoCodificado;

        // IMAGEN SOLO SI EXISTE
        if (imagenIcono != null)
        {
            if (obj.icono != null)
            {
                imagenIcono.gameObject.SetActive(true);
                imagenIcono.sprite = obj.icono;
            }
            else
            {
                imagenIcono.gameObject.SetActive(false);
            }
        }
    }

    public void Cerrar()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
