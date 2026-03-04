using UnityEngine;

public class CamaraOrbital : MonoBehaviour
{
    [InfoMessage("Este es una referencia importante, asegúrate de configurarla correctamente.", MessageTypeCustom.Warning)]
    public Transform jugador;             // Transform del personaje a seguir

    [Header("Pivote / Anti-clip")]
    public Transform pivote;              // Punto desde donde se lanza el raycast (ej: un hijo en el pecho/cabeza)
    public LayerMask layersQueTapan;      // Capas que pueden tapar al personaje (Walls, Props, etc.)
    public float margenColision = 0.15f;  // Para que la cámara no quede pegada a la pared
    public float distanciaMinima = 0.35f; // Para no meterse dentro del personaje

    [Header("Cámara")]
    public float distancia = 1.76f;       // Distancia de la cámara al personaje
    public float altura = 1.0f;           // Altura desde el centro del personaje
    public float sensibilidadMouse = 7.0f;// Sensibilidad al mover el mouse
    public float anguloMin = -20f;        // Límite inferior del ángulo vertical
    public float anguloMax = 80f;         // Límite superior del ángulo vertical

    public bool detenerRotacion;          // Para controlar si rota o no
    public bool bloquearMouse;            // Para bloquear el mouse en la mitad de la pantalla
    public bool mouseInvisible;           // para ocultar el mouse

    private float rotX = 0f; // Rotación acumulada en X (horizontal)
    private float rotY = 0f; // Rotación acumulada en Y (vertical)

    public static CamaraOrbital singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
        else Destroy(this);
    }

    void Start()
    {
        Vector3 angulos = transform.eulerAngles;
        rotX = angulos.y;
        rotY = angulos.x;

        if (bloquearMouse) Cursor.lockState = CursorLockMode.Locked;
        if (mouseInvisible) CursorInvisible();
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        // Fallback: si no asignas pivote, usa el objetivo con altura
        Vector3 posicionObjetivo = jugador.position + Vector3.up * altura;
        if (pivote == null) pivote = jugador;

        if (!detenerRotacion)
        {
            if (Input.GetMouseButton(1))
            {
                rotX += Input.GetAxis("Mouse X") * sensibilidadMouse;
                rotY -= Input.GetAxis("Mouse Y") * sensibilidadMouse;
            }

            rotY = Mathf.Clamp(rotY, anguloMin, anguloMax);

            Quaternion rotacion = Quaternion.Euler(rotY, rotX, 0f);

            // Posición deseada (la que tenías antes)
            Vector3 offset = rotacion * new Vector3(0, 0, -distancia);
            Vector3 posicionDeseada = posicionObjetivo + offset;

            // === NUEVO: Anti-clip con raycast desde pivote hacia la cámara ===
            Vector3 origenRay = pivote.position; // normalmente un punto más alto (pecho/cabeza)
            Vector3 dir = (posicionDeseada - origenRay);
            float largo = dir.magnitude;

            Vector3 posicionFinal = posicionDeseada;

            if (largo > 0.0001f)
            {
                dir /= largo; // normalize

                // Raycast: si algo tapa, acorta la distancia
                if (Physics.Raycast(origenRay, dir, out RaycastHit hit, largo, layersQueTapan, QueryTriggerInteraction.Ignore))
                {
                    float nuevaDist = Mathf.Max(distanciaMinima, hit.distance - margenColision);
                    posicionFinal = origenRay + dir * nuevaDist;
                }
            }

            transform.position = posicionFinal;
            transform.rotation = rotacion;

            if (mouseInvisible && Input.GetMouseButtonDown(0))
            {
                CursorInvisible();
            }
        }
    }

    public void CursorVisible()
    {
        if (mouseInvisible) Cursor.visible = true;
    }

    public void CursorInvisible()
    {
        if (mouseInvisible) Cursor.visible = false;
    }

    public void DeneterCamara()
    {
        detenerRotacion = true;
    }

    public void HabilitarCamara()
    {
        detenerRotacion = false;
    }
}