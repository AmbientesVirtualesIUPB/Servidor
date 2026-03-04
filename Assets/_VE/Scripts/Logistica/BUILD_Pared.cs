using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUILD_Pared : MonoBehaviour
{
    public GameObject tapaSuperior;
    public float altura;
    public int indice;

    public GameObject[] objetosPosibles;
    public int indiceActual;

    private void Update()
    {
        for (int i = 0; i < objetosPosibles.Length; i++)
        {
            objetosPosibles[i].SetActive(i == indiceActual);
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
}
