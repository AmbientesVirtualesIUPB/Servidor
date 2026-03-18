using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBotonera : MonoBehaviour
{
    public static UIBotonera singleton;

    public GameObject prBoton;
    public GameObject prMision;
    public List<UIBotonMision> botones;

    private void Awake()
    {
        singleton = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
       ActualizarMisiones();
    }

    public void VerMision(int cual)
    {
        GameObject go = Instantiate (prMision, transform.parent);
        UIMision  mis = go.GetComponent<UIMision>();
        mis.Inicializar(cual);
    }

    public void ActualizarMisiones( ) 
    {
        for (int i = 0; i < botones.Count; i++)
        {
            Destroy(botones[i].gameObject);
        }
        botones.Clear();
        for (int i = 0; i < GestioMosionesPersonales.singleton.misiones.Count; i++)
        {
            GameObject go = Instantiate(prBoton, transform);
            UIBotonMision bomi = go.GetComponent<UIBotonMision>();
            bomi.indice = i;
            bomi.btm.image.color = GestioMosionesPersonales.singleton.misiones[i].VerificaroCompletado()?Color.green:Color.white;
            botones.Add(bomi);
            go.SetActive(true);

        }
    }
}
