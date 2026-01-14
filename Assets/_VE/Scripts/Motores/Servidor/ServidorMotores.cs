using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServidorMotores : MonoBehaviour
{
    [Header("MANIPULACION DE LAS PIEZAS")]
    public List<MoverPieza> partes;
    public Transform[] padresInstancia;

    [Header("MANIPULACION MESA ARMADO")]
    public SueloInteractivo sueloMesaArmado;
    public bool plataformaIniciada;

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
        GestionMensajesServidor.singeton.RegistrarAccion("MS01", SubirMesaArmado);
        GestionMensajesServidor.singeton.RegistrarAccion("MS02", BajarMesaArmado);
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
                pieza.GetComponent<MorionID>().SetID(parte.idServidor);
                pieza.GetComponent<Collider>().enabled = false; // Desactivamos los colliders de las piezas

                //Buscamos los hijos Snap
                foreach (Transform hijo in pieza.GetComponentsInChildren<Transform>(true))
                {
                    // Evita evaluarse a sí mismo
                    if (hijo == pieza.transform)
                        continue;

                    if (hijo.name.StartsWith("SP"))
                    {
                        hijo.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void SubirMesaArmado(string msj)
    {
        plataformaIniciada = true;
        EntornoMecanica.singleton.AbrirCompuerta(sueloMesaArmado.posicionObjetivoCamara);
        sueloMesaArmado.plataformaAbajo = false;
        sueloMesaArmado.enabled = false; 
        StartCoroutine(HabilitarMesaArmado(10f));
    }

    private IEnumerator HabilitarMesaArmado(float espera)
    {
        yield return new WaitForSeconds(espera);
        sueloMesaArmado.enabled = true;
    }

    public void BajarMesaArmado(string msj)
    {
        plataformaIniciada = false;
        sueloMesaArmado.SalirInteraccion();
        EntornoMecanica.singleton.CerrarCompuerta();
        sueloMesaArmado.plataformaAbajo = true;
        sueloMesaArmado.enabled = false;
        StartCoroutine(HabilitarMesaArmado(8.5f));
    }
}

[System.Serializable]
public class PartesMotores
{
    public int id;
    public Vector3 pos;
    public string idServidor;
}
