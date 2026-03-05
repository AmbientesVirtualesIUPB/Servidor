using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUIL_Bodega : MonoBehaviour
{
    public static BUIL_Bodega singleton;

    public PlanoInicial planoInicial;
    public float offsetParedes;
    public GameObject   pared;

    public ModosConstruccionBodega modo;

    public int cualPared;

    public List<BUILD_Pared> paredes;

    void Awake()
    {
        singleton = this;
    }
    [ContextMenu("Construir")]
    public void Construir()
    {
        float tamX = planoInicial.escalas.x + planoInicial.medidasIniciales.x;
        float tamZ = planoInicial.escalas.z + planoInicial.medidasIniciales.z;
        int indice = 0;
        InstanciarPared(tamX, 0, new Vector3(0, 0, 0), new Vector3(1, 0, 0));
        InstanciarPared(tamX, 180, new Vector3(offsetParedes, 0, tamZ * offsetParedes), new Vector3(1, 0, 0));

        InstanciarPared(tamZ, 90, new Vector3(0, 0, offsetParedes), new Vector3(0, 0, 1));
        InstanciarPared(tamZ, 270, new Vector3(tamX * offsetParedes,0, 0), new Vector3(0, 0, 1));

        void InstanciarPared(float cuantas, float anguloRotacion, Vector3 offset, Vector3 unitario)
        {
            for (int i = 0; i < cuantas; i++)
            {
                GameObject _pared = Instantiate(pared, transform);
                _pared.transform.position = unitario * offsetParedes*i;
                _pared.transform.Translate(offset);
                _pared.transform.Rotate(0, anguloRotacion, 0);
                BUILD_Pared bpared = _pared.GetComponent<BUILD_Pared>();
                bpared.Inicializar(indice, planoInicial.escalas.y);
            }
        }


    }

    public void CambiarCualPared(int cual)
    {
        cualPared = cual;
    }

    public void CambiarModoNumero(int cual)
    {
        switch (cual)
        {
            case 0:
                CambiarModo(ModosConstruccionBodega.espera);
                break;
            case 1:
                CambiarModo(ModosConstruccionBodega.emparedando);
                break;
            default:
                break;
        }
    }

    public void CambiarModo(ModosConstruccionBodega m)
    {
        modo = m;
    }
}


public enum ModosConstruccionBodega
{
    espera,
    emparedando
}