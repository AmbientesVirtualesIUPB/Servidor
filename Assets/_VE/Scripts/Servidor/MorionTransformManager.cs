using System.Collections.Generic;
using UnityEngine;

public class MorionTransformManager : MonoBehaviour
{
    private static MorionTransformManager _singleton;
    public List<MorionTransform> morionTransforms = new List<MorionTransform>();

    /// <summary>
    /// Singleton para que sea solo un manager, procurar que esté en un GameObject llamado "Transformers"
    /// </summary>
    public static MorionTransformManager singleton
    {
        get
        {
            if (_singleton == null)
            {
                // Intentar encontrar el objeto por nombre
                GameObject obj = GameObject.Find("Transformers");
                if (obj != null)
                {
                    _singleton = obj.GetComponent<MorionTransformManager>();
                }

                // Si no se encuentra, mostrar advertencia o crear un nuevo objeto
                if (_singleton == null)
                {
                    Debug.LogWarning("MorionTransformManager no encontrado, creando nuevo objeto.");
                    GameObject newObj = new GameObject("Transformers");
                    _singleton = newObj.AddComponent<MorionTransformManager>();
                }
            }

            return _singleton;
        }
    }

    private void Awake()
    {
        // Asegurar singleton (opcional)
        if (_singleton == null)
        {
            _singleton = this;
        }
        else if (_singleton != this)
        {
            DestroyImmediate(gameObject); // Prevenir duplicados
        }

    }

    private void Start()
    {
        GestionMensajesServidor.singeton.RegistrarAccion("AT01", GestionarMensaje);
    }

    public void GestionarMensaje(string msj)
    {
        DatosActualizablesServidor datos = JsonUtility.FromJson<DatosActualizablesServidor>(msj);
        for (int i = 0; i < morionTransforms.Count; i++)
        {
            if (morionTransforms[i].morionID.GetID().Equals(datos.datosPropios.id) && !morionTransforms[i].morionID.GetOwner())
            {
                morionTransforms[i].datosActualizables = datos;
            }
        }
    }
}
