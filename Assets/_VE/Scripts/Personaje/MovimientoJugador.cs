
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovimientoJugador : MonoBehaviour
{
    public float velocidadInicial; // Velocidad de movimiento del personaje
    public float velocidad = 2f; // Velocidad de movimiento del personaje
    public Transform camaraOrbital;  // Referencia a la cámara orbital
    public Transform camaraOrbital2;  // Referencia a la cámara orbital
    public bool noMover;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // evita que el Rigidbody gire con colisiones
        velocidadInicial = velocidad;
    }

    void FixedUpdate()
    {
        if (!noMover)
        {
            // Leer entrada horizontal (A/D) y vertical (W/S)
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Dirección de movimiento basada en la cámara

            Vector3 direccion = camaraOrbital.forward * vertical + camaraOrbital.right * horizontal;
            if (camaraOrbital2.gameObject.activeSelf)
            {
                direccion = camaraOrbital2.forward * vertical + camaraOrbital2.right * horizontal;
            }

            direccion.y = 0f; // Eliminar componente vertical para evitar que vuele
            direccion.Normalize();  // Normalizar para que la velocidad sea constante en diagonal

            Vector3 movimiento = direccion * velocidad; // Movimiento deseado
            // Combinar movimiento horizontal con la velocidad vertical actual (gravedad)
            Vector3 nuevaVelocidad = new Vector3(movimiento.x, rb.velocity.y, movimiento.z);
            rb.velocity = nuevaVelocidad; // Asignar nueva velocidad al Rigidbody

            //rotar el personaje hacia la dirección de movimiento
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion); // Calcular rotación hacia dirección
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 10f); // Rotación suave
            }
        }       
    }
}
