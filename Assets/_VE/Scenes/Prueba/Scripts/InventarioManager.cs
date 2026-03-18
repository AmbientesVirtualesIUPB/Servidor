using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventarioManager : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject buttonPrefab; // Prefab del botón
    public List<ObjetoAlmacenable> objetosAlmacenados;

    public static InventarioManager singleton;
    private Coroutine coroutine; 

    private void Awake()
    {
        singleton = this;
    }

    public void DestruirInstanciados()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(DestruirInstancias());
    }

    private IEnumerator DestruirInstancias()
    {
        objetosAlmacenados.Clear(); // Vaciamos los objetos almacenados

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            MaterialDisolver[] materialDisolver = transform.GetChild(i).GetComponents<MaterialDisolver>();

            for (int j = 0; j < materialDisolver.Length; j++)
            {
                materialDisolver[j].AgregarDisolver(materialDisolver[j].tiempoDisolver, false);
            }
        }

        yield return new WaitForSeconds(1f);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
