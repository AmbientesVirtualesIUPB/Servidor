using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColocarItemsManager : MonoBehaviour
{
    [Header(" CONFIGURACION ")]

    public ObjetoCreador[] ObjetosCreadores;
    public ObjetoCreador ObjetoCreadorActivo;
    public Vector3 positionOffsetObjetoCreador;
    public LayerMask slotLayer;
    public List<GameObject> BtnObjetosPequenos;
    public List<GameObject> BtnObjetosMedianos;
    public List<GameObject> BtnObjetosGrandes;
    public List<Button> todosLosBotones;
    public List<BtnInventarioLogistica> todosLosBtnInventario;

    [HideInInspector]
    public bool noCrearManager;


    public static ColocarItemsManager singleton;
    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
    }
    void Update()
    {
        if (ObjetoCreadorActivo != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, slotLayer))
            {
                ObjetoCreadorActivo.gameObject.SetActive(true);
                ObjetoCreadorActivo.transform.position = hit.transform.position + positionOffsetObjetoCreador;
                ObjetoCreadorActivo.transform.rotation = hit.transform.rotation;

                if (noCrearManager) return;

                if (Input.GetMouseButtonDown(0))
                {  
                    ObjetoCreadorActivo.Crear();
                }
            }

            else
            {
                ObjetoCreadorActivo.gameObject.SetActive(false);
                noCrearManager = false;
            }
        }    
    }

    public void LlamadoActualizarCreador(int size)
    {
        if (size == 0)
        {
            ActualizarObjetoCreador(ObjetosCreadores[0], 0f);
        }
        else if (size == 1)
        {
            ActualizarObjetoCreador(ObjetosCreadores[1], 0f);
        }
        else if (size == 2)
        {
            ActualizarObjetoCreador(ObjetosCreadores[2], 0f);
        }
    }

    public void ActualizarObjetoCreador(ObjetoCreador objeto, float offsetZ)
    {
        ObjetoCreadorActivo = objeto;
        positionOffsetObjetoCreador.z = offsetZ;
    }

    public void EliminarCreadorActivo()
    {
        ObjetoCreadorActivo = null;

        for (int i = 0; i < todosLosBtnInventario.Count; i++)
        {
            todosLosBtnInventario[i].ReacerSeleccion();
        }
    }


    /// <summary>
    /// Para destruir los hijos de cualquier padre
    /// </summary>
    /// <param name="padre"> Padre que tiene hijos a destruir </param>
    public void DestruirHijos(Transform padre)
    {
        for (int i = padre.childCount - 1; i >= 0; i--)
        {
            Destroy(padre.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Metodo invocado desde los botones de tamaño en el canvas
    /// </summary>
    /// <param name="size"></param>
    public void AignarListas(int size)
    {
        if (size == 0)
        {
            for (int i = 0; i < BtnObjetosPequenos.Count; i++)
            {
                BtnObjetosPequenos[i].SetActive(true);
            }

            for (int i = 0; i < BtnObjetosMedianos.Count; i++)
            {
                BtnObjetosMedianos[i].SetActive(false);
            }

            for (int i = 0; i < BtnObjetosGrandes.Count; i++)
            {
                BtnObjetosGrandes[i].SetActive(false);
            }
        }
        else if (size == 1)
        {
            for (int i = 0; i < BtnObjetosPequenos.Count; i++)
            {
                BtnObjetosPequenos[i].SetActive(false);
            }

            for (int i = 0; i < BtnObjetosMedianos.Count; i++)
            {
                BtnObjetosMedianos[i].SetActive(true);
            }

            for (int i = 0; i < BtnObjetosGrandes.Count; i++)
            {
                BtnObjetosGrandes[i].SetActive(false);
            }
        }
        else if (size == 2)
        {
            for (int i = 0; i < BtnObjetosPequenos.Count; i++)
            {
                BtnObjetosPequenos[i].SetActive(false);
            }

            for (int i = 0; i < BtnObjetosMedianos.Count; i++)
            {
                BtnObjetosMedianos[i].SetActive(false);
            }

            for (int i = 0; i < BtnObjetosGrandes.Count; i++)
            {
                BtnObjetosGrandes[i].SetActive(true);
            }
        }
    }
}
