using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ValidadorPiezasMesa : MonoBehaviour
{
    [Header("Hijos esperados (definidos en Inspector)")]
    public List<HijoGuardado> hijos = new List<HijoGuardado>();

    [ContextMenu("Restaurar hijos faltantes")]
    public void ValidarYRestaurarHijos()
    {
        foreach (HijoGuardado dato in hijos)
        {
            // Buscamos si el hijo ya existe
            Transform hijoActual = transform.Find(dato.nombre);

            if (hijoActual == null)
            {
                if (dato.prefab == null)
                {
                    Debug.LogWarning($"Prefab nulo para el hijo '{dato.nombre}'", this);
                    continue;
                }

                GameObject nuevoHijo = Instantiate(dato.prefab, transform);
                nuevoHijo.name = dato.nombre;
                nuevoHijo.transform.localPosition = dato.posicionLocal;
                nuevoHijo.transform.localRotation = dato.rotacionLocal;
                nuevoHijo.transform.localScale = dato.escalaLocal;
            }
        }
    }

    public void ValidarYRestaurarHijoEspecifico(GameObject objeto)
    {
        if (objeto == null)
        {
            Debug.LogWarning("Objeto recibido es null", this);
            return;
        }

        string nombreBuscado = objeto.name;

        // Buscar datos guardados de ese hijo
        HijoGuardado dato = hijos.Find(h => h.nombre == nombreBuscado);

        if (dato == null)
        {
            Debug.LogWarning($"No existe configuración guardada para '{nombreBuscado}'", this);
            return;
        }

        // Verificar si ya existe en la jerarquía
        Transform hijoActual = transform.Find(nombreBuscado);

        if (hijoActual != null) return; // Ya existe, no hacer nada


        // Si falta lo restaura
        if (dato.prefab == null)
        {
            Debug.LogWarning($"Prefab nulo para '{nombreBuscado}'", this);
            return;
        }

        GameObject nuevoHijo = Instantiate(dato.prefab, transform);
        nuevoHijo.name = dato.nombre;
        nuevoHijo.transform.localPosition = dato.posicionLocal;
        nuevoHijo.transform.localRotation = dato.rotacionLocal;
        nuevoHijo.transform.localScale = dato.escalaLocal;
    }

    [ContextMenu("Desactivar MoverPieza en hijos")]
    public void DesactivarMoverPiezaEnHijos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hijo = transform.GetChild(i);

            XRGrabInteractable mover = hijo.GetComponent<XRGrabInteractable>();

            if (mover != null)
            {
                mover.enabled = false;
            }
        }
    }

    [ContextMenu("Activar MoverPieza en hijos")]
    public void ActivarMoverPiezaEnHijos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hijo = transform.GetChild(i);

            XRGrabInteractable mover = hijo.GetComponent<XRGrabInteractable>();

            if (mover != null)
            {
                mover.enabled = true;
            }
        }
    }
}

[System.Serializable]
public class HijoGuardado
{
    [Header("Identificación")]
    public string nombre;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Transform local")]
    public Vector3 posicionLocal;
    public Vector3 rotacionLocalEuler;
    public Vector3 escalaLocal = Vector3.one;

    // Helper interno
    public Quaternion rotacionLocal => Quaternion.Euler(rotacionLocalEuler);
}
