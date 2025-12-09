using UnityEngine;

public abstract class ObjetoMisionBase : ScriptableObject
{
    public int idObjeto;
    public string nombreObjeto;
    public Sprite icono;

    [TextArea]
    public string descripcion;
}
