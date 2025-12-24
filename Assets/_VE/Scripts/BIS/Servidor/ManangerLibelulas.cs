using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManangerLibelulas : MonoBehaviour
{
    public GameObject[] libelulas_Servidor;
    public List<GameObject> libelulas_Lista;
    // Start is called before the first frame update
    void Start()
    {
        GestionMensajesServidor.singeton.RegistrarAccion("BS00", CreadorLibServidor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("prender")]
    public void Ensallo()
    {
        CrearLibelulaPropia(0);
    }


    public void CrearLibelulaPropia(int cual) 
    {

        GameObject lib = Instantiate(libelulas_Servidor[cual],transform.position,transform.rotation);
        MorionID morionID = lib.GetComponent<MorionID>();
        morionID.SetID("Lib_Estandar" + Random.Range(11111,99999).ToString());   
        morionID.isOwner = true;


        DatosLibelula dl = new DatosLibelula();
        dl.idLid = morionID.GetID();
        dl.posicionLib = lib.transform.position;
        dl.indiceLib = cual;
        GestionMensajesServidor.singeton.EnviarMensaje("BS00", JsonUtility.ToJson(dl));
    }

    public void CreadorLibServidor(string msj)
    {
        DatosLibelula dl = JsonUtility.FromJson<DatosLibelula>(msj);
        GameObject lib = Instantiate(libelulas_Servidor[dl.indiceLib], transform.position, transform.rotation);
        MorionID morionID = lib.GetComponent<MorionID>();
        morionID.SetID(dl.idLid);
        lib.transform.position = dl.posicionLib;

    }
}

[System.Serializable]
public class DatosLibelula
{
    public int indiceLib;
    public string idLid;
    public Vector3 posicionLib;

}
