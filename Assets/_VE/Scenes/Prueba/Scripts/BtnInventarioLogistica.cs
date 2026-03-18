using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnInventarioLogistica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int id;
    public int size;
    public Image imagenIcono;

    [HideInInspector]
    public ColorBlock coloresIniciales;
    ColorBlock coloresSeleccion;

    Button miBtn;

    private void Awake()
    {
        miBtn = GetComponent<Button>();
        
    }

    private void Start()
    {
        coloresIniciales = miBtn.colors;
        coloresSeleccion = miBtn.colors;
        coloresSeleccion.normalColor = coloresSeleccion.highlightedColor;
    }

    /// <summary>
    /// 
    /// </summary>
    public void InstanciarObjeto()
    {
        ColocarItemsManager.singleton.LlamadoActualizarCreador(size);
        ColocarItemsManager.singleton.ObjetoCreadorActivo.AsignarIDActivo(id);

        for (int i = 0; i < ColocarItemsManager.singleton.todosLosBotones.Count; i++)
        {
            ColocarItemsManager.singleton.todosLosBotones[i].colors = coloresIniciales;
        }

        miBtn.colors = coloresSeleccion;
    }

    public void ReacerSeleccion()
    {
        if (miBtn != null)
        {
            miBtn.colors = coloresIniciales;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
