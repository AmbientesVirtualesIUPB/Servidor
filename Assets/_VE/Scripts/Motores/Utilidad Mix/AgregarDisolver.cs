using System.Collections;
using UnityEngine;

public class AgregarDisolver : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public Material materialDisolver;
    public float tiempoDisolver = 1f;

    [Header("OPCIONAL")]
    public MeshRenderer[] meshRendererHijos;

    private bool disolucionAplicada;
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
        if (!disolucionAplicada)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Burbuja", 1f); // Ejecutamos el efecto nombrado 
            disolucionAplicada = true;
            AplicarDisolver(true);
        }   
    }

    [ContextMenu("Restaurar")]
    public void Restaurar()
    {
        if (disolucionAplicada)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Burbuja", 1f); // Ejecutamos el efecto nombrado 
            AplicarDisolver(false);
        }  
    }

    /// <summary>
    /// true = disolver (1 → -1)
    /// false = aparecer (-1 → 1 y restaurar)
    /// </summary>
    public void AplicarDisolver(bool disolverAdentro)
    {
        if (!gameObject.activeInHierarchy || materialDisolver == null)
            return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(DisolverCoroutine(disolverAdentro));
    }

    private IEnumerator DisolverCoroutine(bool disolverAdentro)
    {
        // Crear instancia del material de disolver
        instanciaDisolver = new Material(materialDisolver);

        float valorInicial = disolverAdentro ? 1f : -1f;
        float valorFinal = disolverAdentro ? -1f : 1f;

        instanciaDisolver.SetFloat("_Frecuencia", valorInicial);

        Material[] nuevosMateriales = new Material[] { instanciaDisolver };

        // Asignar material de disolver
        if (meshRendererHijos != null && meshRendererHijos.Length > 0)
        {
            foreach (var r in meshRendererHijos)
                r.materials = nuevosMateriales;
        }
        else if (meshRenderer != null)
        {
            meshRenderer.materials = nuevosMateriales;
        }

        float tiempo = 0f;

        while (tiempo < tiempoDisolver)
        {
            float t = tiempo / tiempoDisolver;
            float valor = Mathf.Lerp(valorInicial, valorFinal, t);

            instanciaDisolver.SetFloat("_Frecuencia", valor);

            tiempo += Time.deltaTime;
            yield return null;
        }

        instanciaDisolver.SetFloat("_Frecuencia", valorFinal);

        // SOLO si estamos restaurando, devolvemos los materiales originales
        if (!disolverAdentro)
        {
            RestaurarMaterialesInstantaneo();
        }

        coroutine = null;
    }

    private void RestaurarMaterialesInstantaneo()
    {
        if (meshRendererHijos != null && meshRendererHijos.Length > 0)
        {
            foreach (var r in meshRendererHijos)
                r.materials = materialesOriginales;
        }
        else if (meshRenderer != null)
        {
            meshRenderer.materials = materialesOriginales;
        }

        disolucionAplicada = false;
    }
}
