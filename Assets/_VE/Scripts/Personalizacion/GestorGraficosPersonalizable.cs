using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorGraficosPersonalizable : MonoBehaviour
{
    public Personaje personaje;
    public ManagerPanels managerPanels;

    public GameObject prBotonMenu;
    List<GameObject> lista = new List<GameObject>();

    public void CrearBotonera(Transform padre, int indice)
	{
		if (lista.Count >0)
		{
			for (int i = 0; i < lista.Count; i++)
			{
				Destroy(lista[i].gameObject);
			}
			lista.Clear();
		}

		for (int i = 0; i < personaje.elementosPersonalizables[indice].GetNumeroElementos(); i++)
		{
			int c = i;
			GameObject go = Instantiate(prBotonMenu, padre);
			go.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => CambiarGrafico(indice, c));
			lista.Add(go);
		}
	}

	public void CambiarGrafico(int indicePersonalizable, int indiceObjeto)
	{
		personaje.elementosPersonalizables[indicePersonalizable].Iniciar(indiceObjeto);
		print("Cambio grafico de " + indicePersonalizable + " por el de la pos " + indiceObjeto);
	}
}
