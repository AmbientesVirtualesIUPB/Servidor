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
            mr.renderer = rends[i];
            mr.mat = rends[i].material;
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
    public Material mat;

    public void CambiarMaterial(Material m)
    {
        renderer.material = m;
    }

    public void RestaurarMaterial()
    {
        renderer.material = mat;
    }
}
