using UnityEngine;

[CreateAssetMenu(fileName = "NuevaMision", menuName = "Misiones/Mision")]
public class DatosDeMision : ScriptableObject
{
    [Header("Info general")]
    public int IdMision;
    public int IdNombreMision;
    public int IdDescripcionMision;

    [Header("Fases en orden")]
    public FaseBase[] fases;
}
