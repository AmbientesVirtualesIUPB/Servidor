using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonObjetoInfo : MonoBehaviour
{
    [Header("Objeto que representa este botón")]
    public ObjetoMisionBase objeto;

    [Header("Referencias UI del botón")]
    public TMP_Text textoNombre;      // Texto del botón
    public GameObject avisoNuevo;     // Indicador tipo "Avisa"

    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(AbrirInformacion);
    }

    private void Start()
    {
        ActualizarEstadoVisual();
    }

    // -------------------------------------------------------------------
    // Cambia el nombre del botón y activa/desactiva el aviso
    // -------------------------------------------------------------------
    public void ActualizarEstadoVisual()
    {
        if (objeto == null)
        {
            Debug.LogWarning("Botón sin objeto asignado.");
            return;
        }

        // Nombre del botón
        if (textoNombre != null)
            textoNombre.text = objeto.nombreObjeto;

        // Estados del objeto
        bool infoDesbloqueada = GestorObjetos.instancia.InfoDesbloqueada(objeto);

        // Aviso de nueva información descubierta
        if (avisoNuevo != null)
            avisoNuevo.SetActive(infoDesbloqueada);
    }

    // -------------------------------------------------------------------
    // Cuando se presiona un botón → abre la enciclopedia con el objeto
    // -------------------------------------------------------------------
    private void AbrirInformacion()
    {
        UI_Enciclopedia.instancia.MostrarObjeto(objeto);

        // Quitar aviso de "nuevo"
        if (avisoNuevo != null)
            avisoNuevo.SetActive(false);
    }
}
