using System.Collections;
using UnityEngine;

public class MaterialDisolver : MonoBehaviour
{
    public Material materialDisolver; // Para el material de disolucion
    public float tiempoDisolver = 1; // Para controlar el tiempo de disolver 
    public MeshRenderer[] meshRendererHijos; // Para los mesh de los hijos de este objeto a los que quiero aplicar disolver

    private float valorDisolver;
    private MeshRenderer meshRenderer;
    private Material[] materialesOriginales;
    private Coroutine coroutine;

    private void Awake()
    {
        // Obtenemos los componentes
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (meshRendererHijos.Length > 0)
        {
            // Guardamos el material original de los hijos
            materialesOriginales = meshRendererHijos[0].materials;
        }
        else
        {
            // Guardamos el material original
            materialesOriginales = meshRenderer.materials;
        }
        // Ejecutamos el efecto al iniciar
        AgregarDisolver(tiempoDisolver, true);
    }

    /// <summary>
    /// Para quitar los materiales de seleccion y solo dejar el material por defecto
    /// </summary>
    public void QuitarMateriales()
    {
        if (meshRendererHijos.Length > 0)
        {
            for (int i = 0; i < meshRendererHijos.Length; i++)
            {
                meshRendererHijos[i].materials = new Material[] { materialesOriginales[0] };
            }
        }
        else
        {
            meshRenderer.materials = new Material[] { materialesOriginales[0] };
        }
    }

    public void AgregarDisolver(float tiempo, bool disolver)
    {
        if (this.gameObject.activeInHierarchy)
        {
            tiempoDisolver = tiempo;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(AgregarMaterialDisolver(disolver));
        }
    }

    private IEnumerator AgregarMaterialDisolver(bool disolverAdentro)
    {
        if (materialDisolver != null)
        {
            // Instanciar copia para que este objeto tenga su propio material
            Material instanciaMaterial = new Material(materialDisolver);
            instanciaMaterial.SetFloat("_Frecuencia", 1f);

            Material[] nuevosMateriales = new Material[1];
            nuevosMateriales[0] = instanciaMaterial;

            // Asignar material a los renderers
            if (meshRendererHijos.Length > 0)
            {
                for (int i = 0; i < meshRendererHijos.Length; i++)
                {
                    meshRendererHijos[i].materials = nuevosMateriales;
                }
            }
            else
            {
                meshRenderer.materials = nuevosMateriales;
            }

            // Interpolar la propiedad "_Frecuencia"
            float tiempo = 0f;
            while (tiempo < tiempoDisolver)
            {
                float t = tiempo / tiempoDisolver;

                if (disolverAdentro)
                {
                    valorDisolver = Mathf.Lerp(1f, -1f, t);
                }
                else
                {
                    valorDisolver = Mathf.Lerp(-1f, 1f, t);
                }

                instanciaMaterial.SetFloat("_Frecuencia", valorDisolver);

                tiempo += Time.deltaTime;
                yield return null;
            }

            // Asegurar valor final
            instanciaMaterial.SetFloat("_Frecuencia", -1f);
            coroutine = null;
            QuitarMateriales();
        }
    }
}
