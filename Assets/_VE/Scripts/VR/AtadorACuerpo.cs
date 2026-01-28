using UnityEngine;

public class AtadorACuerpo : MonoBehaviour
{
    public enum ModoAtado
    {
        Emparentar,
        Seguir
    }

    [Header("Configuración")]
    public ModoAtado modo;
    public ParteCuerpo parteCuerpo;

    [Header("Opciones de Seguimiento")]
    [Range(0.01f, 20f)]
    public float suavizado = 5f;

    public bool seguirRotacion = true;

    private Transform objetivo;

    private void Start()
    {
        if (CuerpoReferenciasVR.singleton == null)
        {
            Debug.LogError("No existe CuerpoReferenciasVR en la escena");
            enabled = false;
            return;
        }

        GameObject obj = CuerpoReferenciasVR.singleton.GetParteCuerpo(parteCuerpo);

        if (obj == null)
        {
            Debug.LogError("La parte del cuerpo seleccionada es nula");
            enabled = false;
            return;
        }

        objetivo = obj.transform;

        if (modo == ModoAtado.Emparentar)
        {
            transform.SetParent(objetivo);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void LateUpdate()
    {
        if (modo != ModoAtado.Seguir || objetivo == null)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            objetivo.position,
            Time.deltaTime * suavizado
        );

        if (seguirRotacion)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                objetivo.rotation,
                Time.deltaTime * suavizado
            );
        }
    }
}
