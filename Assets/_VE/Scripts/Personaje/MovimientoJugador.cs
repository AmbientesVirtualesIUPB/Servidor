
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoJugador : MonoBehaviour
{
    public float velocidadInicial; // Velocidad de movimiento del personaje
    public float velocidad = 2f; // Velocidad de movimiento del personaje
    public Transform camaraOrbital;  // Referencia a la c�mara orbital
    public bool noMover;

    public bool detener;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // evita que el Rigidbody gire con colisiones
        velocidadInicial = velocidad;
    }

    void FixedUpdate()
    {
        if (!detener && camaraOrbital != null)
        {
            // Leer entrada horizontal (A/D) y vertical (W/S)
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Direcci�n de movimiento basada en la c�mara
            Vector3 direccion = camaraOrbital.forward * vertical + camaraOrbital.right * horizontal;
            direccion.y = 0f; // Eliminar componente vertical para evitar que vuele
            direccion.Normalize();  // Normalizar para que la velocidad sea constante en diagonal

            Vector3 movimiento = direccion * velocidad; // Movimiento deseado

            // Combinar movimiento horizontal con la velocidad vertical actual (gravedad)
            Vector3 nuevaVelocidad = new Vector3(movimiento.x, rb.velocity.y, movimiento.z);
            rb.velocity = nuevaVelocidad; // Asignar nueva velocidad al Rigidbody

            //rotar el personaje hacia la direcci�n de movimiento
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion); // Calcular rotaci�n hacia direcci�n
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 10f); // Rotaci�n suave
            }
        }    
    }

    /// <summary>
    /// Para detener la rotacion de la camara
    /// </summary>
    public void DeneterJugador()
    {
        detener = true;
    }

    /// <summary>
    /// Para continuar con la rotacion de la camara
    /// </summary>
    public void HabilitarJugador()
    {
        detener = false;
    }
}
