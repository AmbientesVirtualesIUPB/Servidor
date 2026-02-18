using UnityEngine;

public class MovimientoPingPongSuave : MonoBehaviour
{
    [Header("Destino")]
    public Transform objetivo;

    [Header("Movimiento")]
    public float velocidad = 3f;
    public float suavidadRotacion = 5f;

    [Header("Opciones")]
    public bool usarRotacion = true;
    public bool loop = true;
    public bool iniciarAutomatico = true;

    Vector3 posicionInicial;
    Quaternion rotacionInicial;
    bool activo;

    void Start()
    {
        posicionInicial = transform.localPosition;
        rotacionInicial = transform.localRotation;

        activo = iniciarAutomatico;
    }

    void Update()
    {
        if (!activo || objetivo == null)
            return;

        // Movimiento suave hacia objetivo
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            objetivo.localPosition,
            Time.deltaTime * velocidad
        );

        if (usarRotacion)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                objetivo.localRotation,
                Time.deltaTime * suavidadRotacion
            );
        }

        // Detectar llegada
        if (Vector3.Distance(transform.localPosition, objetivo.localPosition) < 0.02f)
        {
            if (!loop)
            {
                activo = false;
                return;
            }

            // TELETRANSPORTE instantáneo al inicio
            transform.localPosition = posicionInicial;
            transform.localRotation = rotacionInicial;
        }
    }

    // Control externo
    public void Iniciar() => activo = true;
    public void Detener() => activo = false;

    public void Reiniciar()
    {
        transform.localPosition = posicionInicial;
        transform.localRotation = rotacionInicial;
    }
}
