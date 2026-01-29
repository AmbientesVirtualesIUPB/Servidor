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

    [Header("Seguimiento")]
    [Range(0.01f, 20f)]
    public float suavizado = 5f;
    public bool seguirRotacion = true;

    [Header("Offsets")]
    public Vector3 offsetPosicion;
    public Vector3 offsetRotacion;

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
            transform.localPosition = offsetPosicion;
            transform.localRotation = Quaternion.Euler(offsetRotacion);
        }
    }

    private void LateUpdate()
    {
        if (modo != ModoAtado.Seguir || objetivo == null)
            return;

        Vector3 targetPos = objetivo.TransformPoint(offsetPosicion);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * suavizado
        );

        if (seguirRotacion)
        {
            Quaternion targetRot =
                objetivo.rotation * Quaternion.Euler(offsetRotacion);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * suavizado
            );
        }
    }
}
