using System.Collections.Generic;
using UnityEngine;

public class BlendShapeController : MonoBehaviour
{
    [Header("Objetos con posibles SkinnedMeshRenderer")]
    public GameObject[] targets;

    [Header("Valor inicial del blendshape (0‑100)")]
    [Range(0f, 100f)]
    public float blendShapeValue = 0f;

    // Interno: lista de renderers y su índice válido de blendshape
    private List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();
    private Dictionary<SkinnedMeshRenderer, int> blendShapeIndex = new Dictionary<SkinnedMeshRenderer, int>();

    void Start()
    {
        foreach (var go in targets)
        {
            if (go == null) continue;

            // Buscar SkinnedMeshRenderer en el objeto o hijos
            foreach (var smr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Mesh mesh = smr.sharedMesh;
                if (mesh != null && mesh.blendShapeCount > 0)
                {
                    skinnedRenderers.Add(smr);
                    blendShapeIndex[smr] = 0; // usar el primer blendshape
                    smr.SetBlendShapeWeight(0, blendShapeValue);
                }
            }
        }
    }

    void Update()
    {
        // Aplicar valor actual a todos los renderers válidos
        foreach (var smr in skinnedRenderers)
        {
            int idx = blendShapeIndex[smr];
            if (idx >= 0)
                smr.SetBlendShapeWeight(idx, blendShapeValue);
        }
    }

    /// <summary>
    /// Ajusta el valor del blendshape dinámicamente.
    /// </summary>
    public void SetBlendShapeValue(float value)
    {
        blendShapeValue = Mathf.Clamp(value, 0f, 100f);

        foreach (var smr in skinnedRenderers)
        {
            int idx = blendShapeIndex[smr];
            if (idx >= 0)
                smr.SetBlendShapeWeight(idx, blendShapeValue);
        }
    }
}
