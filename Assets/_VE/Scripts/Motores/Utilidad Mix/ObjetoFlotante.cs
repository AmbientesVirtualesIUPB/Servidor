using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoFlotante : MonoBehaviour
{
    [Header("Configuración de Flotación")]
    public float intensidad = 0.5f;   // Altura del movimiento
    public float velocidad = 2f;      // Velocidad del movimiento

    [Header("Opcional: Rotación suave")]
    public bool rotar = false;
    public float velocidadRotacion = 30f;

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos la posición inicial
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // Movimiento senoidal (sube y baja)
        float nuevoY = Mathf.Sin(Time.time * velocidad) * intensidad;

        transform.localPosition = new Vector3(
            posicionInicial.x,
            posicionInicial.y + nuevoY,
            posicionInicial.z
        );

        // Si quieres que rote mientras flota
        if (rotar)
        {
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
        }
    }
}
