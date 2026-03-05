using UnityEngine;

public class BotonInventario : MonoBehaviour
{
    public ObjetoAlmacenable itemPrefab;

    public void OnClick()
    {
        InventarioManager.Singleton.SeleccionarItem(itemPrefab);
    }
}
