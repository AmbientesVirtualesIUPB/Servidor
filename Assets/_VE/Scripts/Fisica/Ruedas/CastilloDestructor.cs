using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastilloDestructor : MonoBehaviour
{
    // Instancia estática para el Singleton
    public static CastilloDestructor instance;

    [Header("Configuración de Objetos")]
    public GameObject castilloCompleto;
    public GameObject castilloPartido;
    public Rigidbody[] piezasCastillo; // La lista de rigidbodies de las piezas

    [Header("Ajustes de Explosión")]
    public float fuerzaMinima = 2f;
    public float fuerzaMaxima = 5f;
    public float torqueIntensidad = 10f;

    void Awake()
    {
        // Configuración del Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Desactiva el castillo sano, activa el destruido y aplica física a las piezas.
    /// </summary>
    public void Destruir()
    {
        // 1. Intercambio de GameObjects
        if (castilloCompleto != null) castilloCompleto.SetActive(false);
        if (castilloPartido != null) castilloPartido.SetActive(true);

        // 2. Aplicar física a cada pieza
        if (piezasCastillo != null && piezasCastillo.Length > 0)
        {
            foreach (Rigidbody rb in piezasCastillo)
            {
                if (rb != null)
                {
                    // Fuerza leve en una dirección aleatoria (como una pequeña explosión interna)
                    Vector3 direccionFuerza = Random.insideUnitSphere;
                    float intensidad = Random.Range(fuerzaMinima, fuerzaMaxima);
                    rb.AddForce(direccionFuerza * intensidad, ForceMode.Impulse);

                    // Torque aleatorio para que las piezas giren al caer
                    Vector3 torqueAleatorio = new Vector3(
                        Random.Range(-torqueIntensidad, torqueIntensidad),
                        Random.Range(-torqueIntensidad, torqueIntensidad),
                        Random.Range(-torqueIntensidad, torqueIntensidad)
                    );
                    rb.AddTorque(torqueAleatorio, ForceMode.Impulse);
                }
            }
        }
    }
}
