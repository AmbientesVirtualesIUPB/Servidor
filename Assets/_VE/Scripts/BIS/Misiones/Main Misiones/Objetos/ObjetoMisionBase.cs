using UnityEngine;

public abstract class ObjetoMisionBase : ScriptableObject
{
    public int idObjeto;
    public string nombreObjeto;
    public Sprite icono;

    [TextArea]
    public string descripcion;

    [Header("Lógica de información")]
    // ✅ Si está activado, este objeto NECESITA análisis para ver la info.
    // ✅ Si está apagado, solo con recolectarlo / interactuar se puede mostrar.
    public bool requiereAnalisisParaInfo = false;
}
