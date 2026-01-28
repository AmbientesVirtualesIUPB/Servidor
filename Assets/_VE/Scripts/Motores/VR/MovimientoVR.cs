using UnityEngine;

public class MovimientoVR : MonoBehaviour
{
    public static MovimientoVR singleton;

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
}
