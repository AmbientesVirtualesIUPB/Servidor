using System.Collections;
using UnityEngine;

public class Apuntador : MonoBehaviour
{
    [Tooltip("Ángulos objetivo para moverse")]
    public float[] angulosDisparo;
    [Tooltip("Referencia al script que obtiene el ángulo actual")]
    public ControlGraficoBallesta graficoBallesta;

    [Tooltip("Índice actual del ángulo objetivo en el arreglo")]
    public int indiceObjetivo = 0;

    [Tooltip("Velocidad del movimiento en grados por segundo")]
    public float velocidad = 90f;

    private Coroutine rotacionEnCurso;

    /// <summary>
    /// Inicia la rotación hacia el ángulo en angulosDisparo[indiceObjetivo].
    /// </summary>
    [ContextMenu("Mover hacia ángulo objetivo")]
    public void IniciarMovimiento()
    {
        // Si ya hay una corutina en ejecución, detenerla
        if (rotacionEnCurso != null)
        {
            StopCoroutine(rotacionEnCurso);
        }
        // Iniciar nueva corutina
        rotacionEnCurso = StartCoroutine(MoverAHaciaObjetivo());
    }

    /// <summary>
    /// Coroutine que se encarga de rotar desde el ángulo actual hasta el objetivo, a velocidad uniforme
    /// </summary>
    private IEnumerator MoverAHaciaObjetivo()
    {
        if (angulosDisparo == null || angulosDisparo.Length == 0)
        {
            Debug.LogWarning("No hay ángulos en angulosDisparo.");
            yield break;
        }

        // Clamping de índice
        indiceObjetivo = Mathf.Clamp(indiceObjetivo, 0, angulosDisparo.Length - 1);

        float anguloDestino = angulosDisparo[indiceObjetivo];
        float anguloActual = graficoBallesta.GetDireccion();

        // Mover hasta alcanzar con cierta tolerancia
        while (!Mathf.Approximately(anguloActual, anguloDestino))
        {
            // Interpolación suave hacia destino
            anguloActual = Mathf.MoveTowardsAngle(anguloActual, anguloDestino, velocidad * Time.deltaTime);

            // Aplicar el ángulo actualizado al ControlGraficoBallesta
            graficoBallesta.CambiarDireccion(anguloActual);

            yield return null; // esperar al siguiente frame
        }

        rotacionEnCurso = null; // marcado como finalizado
    }
    /// <summary>
    /// Cambia el valor del indice y de una vez animar
    /// </summary>
    /// <param name="indice"></param>
    public void CambiarIndice(int indice)
    {
        indiceObjetivo = indice;
        IniciarMovimiento();
    }

    /// <summary>
    /// Opcional: avanzar al siguiente índice en el arreglo
    /// </summary>
    public void SiguienteDestino()
    {
        if (angulosDisparo == null || angulosDisparo.Length == 0)
            return;

        indiceObjetivo = (indiceObjetivo + 1) % angulosDisparo.Length;
    }
}
