using UnityEngine;

public class InventarioManager : MonoBehaviour
{
    public static InventarioManager Singleton;

    public ObjetoAlmacenable ItemSeleccionado;

    private void Awake()
    {
        Singleton = this;
    }

    public void SeleccionarItem(ObjetoAlmacenable itemPrefab)
    {
        ItemSeleccionado = itemPrefab;
        Debug.Log("Seleccionaste: " + itemPrefab.itemNombre);
    }

    public void LimpiarSeleccion()
    {
        ItemSeleccionado = null;
    }
}
