using UnityEngine;

public class MovimientoPingPongSuave : MonoBehaviour
{
    [Header("Destino")]
    public Transform[] objetivos;
    int indice;

    [Header("Movimiento")]
    public float velocidad = 3f;
    public float suavidadRotacion = 5f;

    [Header("Opciones")]
    public bool usarRotacion = true;
    public bool loop = true;
    public bool iniciarAutomatico = true;

    float t;
    Vector3 posicionInicial;
    Quaternion rotacionInicial;
    bool activo;

    float velocidadAjuste;

    void Start()
    {
        posicionInicial = transform.localPosition;
        rotacionInicial = transform.localRotation;

        activo = iniciarAutomatico;

        velocidadAjuste = (posicionInicial - objetivos[0].localPosition).magnitude;
    }

    void Update()
    {
        if (!activo || objetivos == null)
            return;

        // Movimiento suave hacia objetivo
        transform.localPosition = Vector3.Lerp(
            (indice == 0) ? posicionInicial : objetivos[indice - 1].localPosition,
            objetivos[indice].localPosition,
            t
        );

        if (usarRotacion)
        {
            transform.localRotation = Quaternion.Slerp(
                (indice == 0) ? rotacionInicial : objetivos[indice - 1].localRotation,
                objetivos[indice].localRotation,
                t
            );
        }

        t += Time.deltaTime * velocidad / velocidadAjuste;

        // Detectar llegada
        if (t > 1)
        {
            if (!loop)
            {
                activo = false;
                return;
            }

            // TELETRANSPORTE instantáneo al inicio
            transform.localPosition = posicionInicial;
            transform.localRotation = rotacionInicial;
            t = 0;
            indice = (indice + 1) % objetivos.Length;
            velocidadAjuste = ((indice == 0) ? posicionInicial : objetivos[indice - 1].localPosition - objetivos[indice].localPosition).magnitude;
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
