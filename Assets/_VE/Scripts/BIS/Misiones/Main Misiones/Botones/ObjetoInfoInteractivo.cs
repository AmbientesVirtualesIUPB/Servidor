using UnityEngine;

public class ObjetoInfoInteractivo : MonoBehaviour
{
    public ObjetoMisionBase objeto;
    public string tagJugador = "Player";

    private bool jugadorDentro = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = true;
        UI_Recoleccion.instancia.MostrarMensaje("Presiona E para inspeccionar " + objeto.nombreObjeto);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = false;
        UI_Recoleccion.instancia.OcultarMensaje();
    }

    private void Update()
    {
        if (!jugadorDentro) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Siempre se marca como descubierto
            GestorObjetos.instancia.MarcarDescubierto(objeto);

            // Si NO requiere análisis → info directa
            if (!objeto.requiereAnalisisParaInfo)
            {
                GestorObjetos.instancia.DesbloquearInformacion(objeto);
            }

            // Opcional: abrir la enciclopedia al toque
            if (UI_Enciclopedia.instancia != null)
                UI_Enciclopedia.instancia.MostrarObjeto(objeto);
        }
    }
}
