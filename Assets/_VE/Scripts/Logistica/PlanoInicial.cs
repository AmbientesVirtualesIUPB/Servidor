using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanoInicial : MonoBehaviour
{
    public RectTransform imSuperior;
    public RectTransform imInferior;
    public Vector3 medidasIniciales;
    public float valorUnidad;
    public Vector3 escalas;

    public Vector3 inicialesUI;
    public float escalasUI;

    public Text txtX;
    public Text txtY;
    public Text txtZ;

    // Start is called before the first frame update
    void Start()
    {
        ActualizarDatos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CambiarX(float c)
    {
        escalas.x = c;
        ActualizarDatos();
    }

    public void CambiarY(float c)
    {
        escalas.y = c;
        ActualizarDatos();
    }

    public void CambiarZ(float c)
    {
        escalas.z = c;
        ActualizarDatos();
    }

    void ActualizarDatos()
    {
        Vector2 tam = imSuperior.sizeDelta;
        tam.y = inicialesUI.z + escalas.z * escalasUI;
        imSuperior.sizeDelta = tam;
        tam = imInferior.sizeDelta;
        tam.y = inicialesUI.y + escalas.y * escalasUI;
        tam.x = inicialesUI.x + escalas.x * escalasUI;
        imInferior.sizeDelta = tam;

        txtX.text = Mathf.RoundToInt(medidasIniciales.x + escalas.x * valorUnidad).ToString() + " m";
        txtY.text = Mathf.RoundToInt(medidasIniciales.y + escalas.y * valorUnidad).ToString() + " m";
        txtZ.text = Mathf.RoundToInt(medidasIniciales.z + escalas.z * valorUnidad).ToString() + " m";
    }
}
