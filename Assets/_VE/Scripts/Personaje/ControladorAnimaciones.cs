using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ControladorAnimaciones : MonoBehaviour
{
    public MovimientoJugador movimientoJugador;
    public float velocidadCorrer;
    public int tiempoSprint = 5;
    public float velocidadSuperCorrer;
    public Vector3 posicionAnterior;

    Animator animator;
    bool corriendo = false;
    float tiempoCorriendo = 0f;
    float tiempoSalto = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Activar el modo correr mientras mantengo Shift presionado
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            corriendo = true;
            tiempoCorriendo = 0f; // reiniciamos el contador al empezar
            movimientoJugador.velocidad = velocidadCorrer;
        }
        // Desactivar el modo correr al dejar de presionar el Shift
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            corriendo = false;
            tiempoCorriendo = 0f;
            animator.SetBool("CorriendoRapido", false);
            movimientoJugador.velocidad = movimientoJugador.velocidadInicial;
        }

        // Si está corriendo, incrementa el tiempo
        if (corriendo)
        {
            tiempoCorriendo += Time.deltaTime;

            // Al pasar los 5 segundos, activar super carrera
            if (tiempoCorriendo > tiempoSprint)
            {
                animator.SetBool("CorriendoRapido", true);
                movimientoJugador.velocidad = velocidadSuperCorrer;
            }
        }
        // Enviar este estado al Animator
        animator.SetBool("Corriendo", corriendo);

        if (animator.GetFloat("Velocidad") == 0)
        {
            tiempoCorriendo = 0f;
            animator.SetBool("CorriendoRapido", false);
            movimientoJugador.velocidad = movimientoJugador.velocidadInicial;
        }

        if (Input.GetKeyDown(KeyCode.Space) && tiempoSalto > 2)
        {
            tiempoSalto = 0f;
            movimientoJugador.velocidad = 0;
            animator.SetFloat("Velocidad", 0);
            movimientoJugador.noMover = true;
            animator.SetTrigger("Saltar");
        }
        else if (tiempoSalto > 2)
        {
            movimientoJugador.noMover = false;
        }

        tiempoSalto += Time.deltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        posicionAnterior = transform.position;
        StartCoroutine(Actualizador());
    }

    IEnumerator Actualizador()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            animator.SetFloat("Velocidad", (transform.position - posicionAnterior).magnitude * 50);
            posicionAnterior = transform.position;
        }
    }

}
