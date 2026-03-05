using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BUILD_Pared : MonoBehaviour
{
    public GameObject tapaSuperior;
    public float altura;
    public int indice;
    public CambiaColores cambiaColores;

    public GameObject[] objetosPosibles;
    public int indiceActual;
    public int indiceTemporal;



    private void FixedUpdate()
    {
        for (int i = 0; i < objetosPosibles.Length; i++)
        {
            int ids = (indiceTemporal != -1) ? indiceTemporal : indiceActual;
            objetosPosibles[i].SetActive(i == ids);
        }
    }

    public void Inicializar(int _indice, float _altura)
    {
        indice = _indice;
        altura = _altura;
        tapaSuperior.transform.localScale = new Vector3
            (
                tapaSuperior.transform.localScale.x,
                altura,
                tapaSuperior.transform.localScale.y 
            );
    }

    public void VerificarCambio()
    {
        if (BUIL_Bodega.singleton.modo == ModosConstruccionBodega.emparedando)
        {
            indiceTemporal = BUIL_Bodega.singleton.cualPared;
            cambiaColores.CambiarMaterial();
        }
    }

    public void DesVerificar()
    {
        indiceTemporal = -1;
        cambiaColores.RestaurarMaterial();
    }

    public void ConstanteVerificamiento()
    {
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject() &&
            BUIL_Bodega.singleton.modo == ModosConstruccionBodega.emparedando &&
            indiceTemporal != -1)
        {
            CrearNuevoObjeto();
        }
    }

    void CrearNuevoObjeto()
    {
        indiceActual = indiceTemporal;
    }

}
