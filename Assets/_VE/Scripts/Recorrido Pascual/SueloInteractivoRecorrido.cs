using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SueloInteractivoRecorrido : MonoBehaviour
{
    [Header("Referencias Obligatorias")]
    public ParticleSystem particulas;
    public GameObject canvasWorldSpace; // Hace referencia al canvas que nos indica que tecla oprimir
    public bool interactuar;
    private MovimientoJugador movimientoJugador;

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (interactuar)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(Interactuar());
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (!ValidarOwner(other)) return;
            interactuar = true;
            canvasWorldSpace.SetActive(true); // Activamos canvas visual
        }
    }

    public bool ValidarOwner(Collider other)
    {
        MorionID morionID = other.GetComponent<MorionID>();

        if (morionID != null && !morionID.isOwner)
        {
            return false;
        }
        else
        {
            morionID = other.GetComponentInChildren<MorionID>();
            if (morionID != null && !morionID.isOwner) return false;
        }
        return true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!ValidarOwner(other)) return;
            movimientoJugador = other.GetComponent<MovimientoJugador>();  // Obtenemos una referencia al movimiento del jugador que interactua
        }
    }

    private void OnTriggerExit(Collider other)
    {
        movimientoJugador = null;  // Eliminamos la referencia al movimiento del jugador que interactua

        if (other.CompareTag("Player"))
        {
            if (!ValidarOwner(other)) return;
            interactuar = false;
            canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual
        }
    }

    private IEnumerator Interactuar()
    {
        particulas.Play();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("C_Motores");
    }
}
