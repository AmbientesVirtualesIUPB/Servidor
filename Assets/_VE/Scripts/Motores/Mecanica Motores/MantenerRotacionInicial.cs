using UnityEngine;

public class MantenerRotacionInicial : MonoBehaviour
{
    public Vector3 rot;
    public bool validarRotacionInicial;

    public float velocidad = 5f;
    public float tolerancia = 0.1f; // grados

    void Update()
    {
        if (!validarRotacionInicial) return;

        Quaternion rotObjetivo = Quaternion.Euler(rot);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotObjetivo, Time.deltaTime * velocidad);

        // Validar si ya llego
        if (Quaternion.Angle(transform.rotation, rotObjetivo) <= tolerancia)
        {
            transform.rotation = rotObjetivo; // asegurar valor exacto
            validarRotacionInicial = false;
            RotacionFinalizada();
        }
    }

    void RotacionFinalizada()
    {
        Debug.Log("aqui");
        validarRotacionInicial = false;
    }
}
