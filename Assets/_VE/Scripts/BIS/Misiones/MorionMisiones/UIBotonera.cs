using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBotonera : MonoBehaviour
{
    public static UIBotonera singleton;

    public GameObject prBoton;
    public GameObject prMision;

    private void Awake()
    {
        singleton = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        for (int i = 0; i < GestioMosionesPersonales.singleton.misiones.Count; i++)
        {
            GameObject go = Instantiate(prBoton, transform);
            UIBotonMision bomi = go.GetComponent<UIBotonMision>();
            bomi.indice = i;
            go.SetActive(true);

        }
    }

    public void VerMision(int cual)
    {
        GameObject go = Instantiate (prMision, transform.parent);
        UIMision  mis = go.GetComponent<UIMision>();
        mis.Inicializar(cual);
    }
}
