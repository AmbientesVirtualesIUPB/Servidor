using System.Collections.Generic;
using UnityEngine;

public class ServidorMotores : MonoBehaviour
{
    public List<MoverPieza> partes;
    public Transform[] padresInstancia;
    public bool esArmador = false;

    public static ServidorMotores singleton;
    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GestionMensajesServidor.singeton.RegistrarAccion("MS00", InstanciaPiezaServidor);
    }

    public void InstanciaPiezaServidor(string msj)
    {
        PartesMotores parteMotor = JsonUtility.FromJson<PartesMotores>(msj);
        InstanciarPieza(parteMotor);
    }


    public void InstanciarPieza(PartesMotores parte)
    {
        for (int i = 0; i < partes.Count; i++)
        {
            if (parte.id == partes[i].id)
            {
                GameObject pieza = Instantiate(partes[i].gameObject,parte.pos,Quaternion.identity);
                pieza.transform.parent = padresInstancia[pieza.GetComponent<MoverPieza>().piezaExterna ? 0 : 1];
            }
        }
    }

}

[System.Serializable]
public class PartesMotores
{
    public int id;
    public Vector3 pos;
    public string idServidor;
}
