using UnityEngine;

public class IAves : MonoBehaviour
{
    [Header("Configuracion de Area")]
    public Vector3 puntoUno;
    public Vector3 puntoDos;

    [Header("Resultado")]
    public Vector3 target;
    private Vector3 bTarget;

    [Header("Vuelo")]
    public float velocidad = 5;
    public float velRotacion = 20;
    public float rangoDinamico;

    float rd2;
    void Start()
    {
        ActualizarRangoDinamico();
        // Actualiza el target al iniciar el juego
        ActualizarTarget();
    }

    private void Update()
    {
        Quaternion br = transform.rotation;
        transform.LookAt(target);
        transform.rotation = Quaternion.Lerp(br, transform.rotation, velRotacion * Time.deltaTime);
        transform.Translate(0, 0, velocidad * Time.deltaTime);
        if ((transform.position - target).sqrMagnitude < rd2)
        {
            ActualizarTarget();
        }
    }

    /// <summary>
    /// Calcula un punto aleatorio entre puntoUno y puntoDos y lo guarda en target.
    /// </summary>
    [ContextMenu("Actualizar Target")]
    public void ActualizarTarget()
    {
        bTarget = target;
        target = ObtenerPuntoAleatorio();
    }

    /// <summary>
    /// Actualiza el cuadrado del rango dinamico por optimización.
    /// </summary>
    [ContextMenu("Actualizar Target")]
    void ActualizarRangoDinamico()
    {
        rd2 = rangoDinamico * rangoDinamico;
    }

    public Vector3 ObtenerPuntoAleatorio()
    {
        // Buscamos los valores minimos y maximos para que funcione 
        // sin importar el orden en que se pongan los puntos en el inspector
        float x = Random.Range(Mathf.Min(puntoUno.x, puntoDos.x), Mathf.Max(puntoUno.x, puntoDos.x));
        float y = Random.Range(Mathf.Min(puntoUno.y, puntoDos.y), Mathf.Max(puntoUno.y, puntoDos.y));
        float z = Random.Range(Mathf.Min(puntoUno.z, puntoDos.z), Mathf.Max(puntoUno.z, puntoDos.z));

        return new Vector3(x, y, z);
    }

    private void OnDrawGizmosSelected()
    {
        // 1. Dibujar el cubo que delimita el area
        // El centro del cubo es el promedio de ambos vectores
        Vector3 centro = (puntoUno + puntoDos) / 2f;

        // El tamaño es la diferencia absoluta entre sus ejes
        Vector3 tamano = new Vector3(
            Mathf.Abs(puntoUno.x - puntoDos.x),
            Mathf.Abs(puntoUno.y - puntoDos.y),
            Mathf.Abs(puntoUno.z - puntoDos.z)
        );

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(centro, tamano);

        // 2. Dibujar las esquinas para visualizarlas mejor
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(puntoUno, 0.15f);
        Gizmos.DrawSphere(puntoDos, 0.15f);

        // 3. Dibujar el Target actual
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bTarget, target); // Una linea desde el centro al objetivo
        Gizmos.DrawSphere(target, 0.25f);
    }
}