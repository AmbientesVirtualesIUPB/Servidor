using System.Collections;
using UnityEngine;

public class AgregarDisolver : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public Material materialDisolver;
    public float tiempoDisolver = 1f;

    [Header("OPCIONAL")]
    public MeshRenderer[] meshRendererHijos;

    private MeshRenderer meshRenderer;
    private Material[] materialesOriginales;
    private Coroutine coroutine;
    private Material instanciaDisolver;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        GuardarMaterialesOriginales();
    }

    private void GuardarMaterialesOriginales()
    {
        if (meshRendererHijos != null && meshRendererHijos.Length > 0)
        {
            materialesOriginales = meshRendererHijos[0].materials;
        }
        else if (meshRenderer != null)
        {
            materialesOriginales = meshRenderer.materials;
        }
    }
    [ContextMenu("Disolver")]
    public void Disolver()
    {
        AplicarDisolver(true);
    }
    /// <summary>
    /// Aplica el material de disolver
    /// </summary>
    /// <param name="disolverAdentro">true = disolver, false = aparecer</param>
    
    public void AplicarDisolver(bool disolverAdentro)
    {
        if (!gameObject.activeInHierarchy || materialDisolver == null)
            return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(DisolverCoroutine(disolverAdentro));
    }

    /// <summary>
    /// Restaura los materiales originales
    /// </summary>
    [ContextMenu("Restaurar")]
    public void RestaurarMateriales()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        if (meshRendererHijos != null && meshRendererHijos.Length > 0)
        {
            foreach (var r in meshRendererHijos)
            {
                r.materials = materialesOriginales;
            }
        }
        else if (meshRenderer != null)
        {
            meshRenderer.materials = materialesOriginales;
        }

        coroutine = null;
    }

    private IEnumerator DisolverCoroutine(bool disolverAdentro)
    {
        // Crear instancia del material
        instanciaDisolver = new Material(materialDisolver);
        instanciaDisolver.SetFloat("_Frecuencia", disolverAdentro ? 1f : -1f);

        Material[] nuevosMateriales = new Material[] { instanciaDisolver };

        // Asignar material
        if (meshRendererHijos != null && meshRendererHijos.Length > 0)
        {
            foreach (var r in meshRendererHijos)
            {
                r.materials = nuevosMateriales;
            }
        }
        else if (meshRenderer != null)
        {
            meshRenderer.materials = nuevosMateriales;
        }

        float tiempo = 0f;

        while (tiempo < tiempoDisolver)
        {
            float t = tiempo / tiempoDisolver;

            float valor = disolverAdentro
                ? Mathf.Lerp(1f, -1f, t)
                : Mathf.Lerp(-1f, 1f, t);

            instanciaDisolver.SetFloat("_Frecuencia", valor);

            tiempo += Time.deltaTime;
            yield return null;
        }

        instanciaDisolver.SetFloat("_Frecuencia", disolverAdentro ? -1f : 1f);

        coroutine = null;
    }
}
