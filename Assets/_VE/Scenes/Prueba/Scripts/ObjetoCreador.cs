using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjetoCreador : MonoBehaviour
{
    public Material matBien, matMal;
    public GameObject objetoCrearBase;
    public Vector3 positionOffsetObjetoCreado;
    public ObjetoAlmacenable[] objetosCrear;

    [HideInInspector]
    public bool noCrear;

    int idActivo;
    Renderer mirenderer;
    Coroutine coroutine;

    private void Awake()
    {
        mirenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        for (int i = 0; i < objetosCrear.Length; i++)
        {
            GameObject nuevoBoton = Instantiate(InventarioManager.singleton.buttonPrefab, InventarioManager.singleton.contentPanel);// Instanciamos el boton en el inventario

            TextMeshProUGUI textoBoton = nuevoBoton.GetComponentInChildren<TextMeshProUGUI>(); // Obtenemos el componente texto
            textoBoton.text = objetosCrear[i].itemNombre; // Asignamos el texto al boton

            BtnInventarioLogistica btnInventario = nuevoBoton.GetComponent<BtnInventarioLogistica>();
            btnInventario.imagenIcono.sprite = objetosCrear[i].icono;
            btnInventario.id = objetosCrear[i].itemID;

            if (objetosCrear[i].itemEspacio == ItemTamano.Pequeno)
            {
                btnInventario.size = 0;
                ColocarItemsManager.singleton.BtnObjetosPequenos.Add(nuevoBoton);
            }
            else if (objetosCrear[i].itemEspacio == ItemTamano.Mediano)
            {
                btnInventario.size = 1;
                ColocarItemsManager.singleton.BtnObjetosMedianos.Add(nuevoBoton);
            }
            else if (objetosCrear[i].itemEspacio == ItemTamano.Grande)
            {
                btnInventario.size = 2;
                ColocarItemsManager.singleton.BtnObjetosGrandes.Add(nuevoBoton);
            }

            Button btn = nuevoBoton.GetComponent<Button>(); // Obtenemos el componenete button
            btn.onClick.AddListener(btnInventario.InstanciarObjeto); // Agregamos la acción al botón
            ColocarItemsManager.singleton.todosLosBotones.Add(btn);
            ColocarItemsManager.singleton.todosLosBtnInventario.Add(btnInventario);
        }

        this.gameObject.SetActive(false);
    }

    public void Crear()
    {
        GameObject obj = Instantiate(objetoCrearBase.gameObject, transform.position, transform.rotation);
        obj.transform.SetParent(ColocarItemsManager.singleton.transform);

        for (int i = 0; i < objetosCrear.Length; i++)
        {
            if (idActivo == objetosCrear[i].itemID)
            {
                GameObject obj2 = Instantiate(objetosCrear[i].gameObject, transform.position + positionOffsetObjetoCreado, transform.rotation);
                obj2.transform.SetParent(InventarioManager.singleton.transform, true);
                InventarioManager.singleton.objetosAlmacenados.Add(obj2.GetComponent<ObjetoAlmacenable>());
                break;
            }
        }
    }

    public void AsignarIDActivo(int id)
    {
        idActivo = id;
    }

    private void Update()
    {
        mirenderer.material = noCrear ? matMal : matBien;
    }

    private void OnDisable()
    {
        noCrear = false;
        ColocarItemsManager.singleton.noCrearManager = false;
    }

    private void OnEnable()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(CorrutinaOnEnable());
    }

    IEnumerator CorrutinaOnEnable()
    {
        ColocarItemsManager.singleton.noCrearManager = true;
        yield return new WaitForSeconds(0.05f);
        ColocarItemsManager.singleton.noCrearManager = false;
    }
}
