using UnityEngine;

public class ObjetoCreador : MonoBehaviour
{
    public Material matBien, matMal;
    public GameObject objetoCrear;

    [HideInInspector]
    public bool noCrear;
    Renderer mirenderer;

    private void Awake()
    {
        mirenderer = GetComponent<Renderer>();
    }
    public void Crear()
    {
        GameObject obj = Instantiate(objetoCrear.gameObject, transform.position, transform.rotation);
        obj.transform.SetParent(ColocarItemsManager.singleton.transform);
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
}
