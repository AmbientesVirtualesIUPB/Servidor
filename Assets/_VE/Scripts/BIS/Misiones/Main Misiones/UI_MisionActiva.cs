using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UI_MisionActiva : MonoBehaviour
{
    public static UI_MisionActiva instancia;

    [Header("Panel de misión activa")]
    public GameObject panel;

    [Header("Campos de UI (TextMeshPro)")]
    public GameObject textoNombreMision;
    public GameObject textoFase;
    public GameObject textoDescripcionFase;
    public GameObject textoObjetivos;


    private void Awake()
    {
        instancia = this;

        if (panel != null)
            panel.SetActive(false);
    }

    // -------------------------------------------
    // Cuando el gestor inicia misión → se llama esto
    // -------------------------------------------
    public void MostrarMision(DatosDeMision mision, FaseBase fase)
    {
        if (panel != null)
            panel.SetActive(true);

        var ciNombre = textoNombreMision.GetComponent<ControlIdioma>();
        ciNombre.texto = mision.IdNombreMision;
        ciNombre.tmp = textoNombreMision.GetComponent<TextMeshProUGUI>();
        ciNombre.ActualizarTexto();

        var ciFase = textoFase.GetComponent<ControlIdioma>();
        ciFase.texto = fase.idNombreFase;
        ciFase.tmp = textoFase.GetComponent<TextMeshProUGUI>();
        ciFase.ActualizarTexto();

        var ciDesc = textoDescripcionFase.GetComponent<ControlIdioma>();
        ciDesc.texto = fase.idDescripcionFase;
        ciDesc.tmp = textoDescripcionFase.GetComponent<TextMeshProUGUI>();
        ciDesc.ActualizarTexto();


    }

    // -------------------------------------------
    // Cuando el gestor cambia fase → se llama esto 
    // -------------------------------------------
    public void ActualizarFase(FaseBase fase)
    {
        textoFase.GetComponent<ControlIdioma>().texto = fase.idNombreFase;
        textoDescripcionFase.GetComponent<ControlIdioma>().texto = fase.idDescripcionFase;
    }

    // -------------------------------------------
    // Cuando la misión termina → ocultar UI
    // -------------------------------------------
    public void OcultarMision()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
