using UnityEngine;

public class ColocarItemsManager : MonoBehaviour
{
    public ObjetoCreador[] ObjetosCreadores;
    public ObjetoCreador ObjetoCreadorActivo;
    public Vector3 positionOffsetObjetoCreador;
    public LayerMask slotLayer;

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

                if (Input.GetMouseButtonDown(0))
                {
                    if (noCrearManager) return;
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
            ActualizarObjetoCreador(ObjetosCreadores[1], 2.2f);
        }
        else if (size == 2)
        {
            ActualizarObjetoCreador(ObjetosCreadores[2], 4.44f);
        }
    }

    public void ActualizarObjetoCreador(ObjetoCreador objeto, float offsetZ)
    {
        ObjetoCreadorActivo = objeto;
        positionOffsetObjetoCreador.z = offsetZ;
    }
}
