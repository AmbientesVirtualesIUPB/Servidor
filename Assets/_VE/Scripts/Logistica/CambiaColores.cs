using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambiaColores : MonoBehaviour
{
    public Material matCambiado;
    public List<MatRen> renderMateriales;

    private void Start()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            MatRen mr = new MatRen();
            mr.Inicializar(rends[i]);
            renderMateriales.Add(mr);
        }
    }
    [ContextMenu("Cambiar color")]
    public void CambiarMaterial()
    {
        for (int i = 0; i < renderMateriales.Count; i++)
        {
            renderMateriales[i].CambiarMaterial(matCambiado);
        }
    }

    [ContextMenu("Restaurar color")]
    public void RestaurarMaterial()
    {
        for (int i = 0; i < renderMateriales.Count; i++)
        {
            renderMateriales[i].RestaurarMaterial();
        }
    }


}

[System.Serializable]
public class MatRen
{
    public Renderer renderer;
    public Material[] mats;
    public Material mat;

    public void Inicializar(Renderer r)
    {
        renderer = r;

        // Guardar materiales originales
        mats = renderer.materials;

        // Guardar el primer material por compatibilidad con tu variable mat
        if (mats.Length > 0)
            mat = mats[0];
    }

    public void CambiarMaterial(Material m)
    {
        if (renderer == null) return;

        Material[] nuevos = new Material[renderer.materials.Length];

        for (int i = 0; i < nuevos.Length; i++)
        {
            nuevos[i] = m;
        }

        renderer.materials = nuevos;
    }

    public void RestaurarMaterial()
    {
        if (renderer == null || mats == null) return;

        renderer.materials = mats;
    }
}
