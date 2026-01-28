using UnityEngine;

public class CuerpoReferenciasVR : MonoBehaviour
{
    public static CuerpoReferenciasVR singleton;

    [Header("Referencias del Cuerpo VR")]
    public GameObject manoDerecha;
    public GameObject manoIzquierda;
    public GameObject cabeza;
    public GameObject cuerpo;

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;
    }

    public GameObject GetParteCuerpo(ParteCuerpo parte)
    {
        switch (parte)
        {
            case ParteCuerpo.ManoDerecha: return manoDerecha;
            case ParteCuerpo.ManoIzquierda: return manoIzquierda;
            case ParteCuerpo.Cabeza: return cabeza;
            case ParteCuerpo.Cuerpo: return cuerpo;
            default: return null;
        }
    }
}

public enum ParteCuerpo
{
    ManoDerecha,
    ManoIzquierda,
    Cabeza,
    Cuerpo
}
