using UnityEngine;
using static TamanoItem;

public class EstanteriaSlot : MonoBehaviour
{
    public ItemTamano tamanoSlot;

    private bool estaOcupado;
    private ObjetoAlmacenable objetoAlmacenado;

    public void IntentarColocarItem()
    {
        if (estaOcupado)
        {
            Debug.Log("Slot ocupado");
            return;
        }

        if (InventarioManager.Singleton.ItemSeleccionado == null)
        {
            Debug.Log("No hay objeto seleccionado");
            return;
        }

        ObjetoAlmacenable prefab = InventarioManager.Singleton.ItemSeleccionado;

        if (prefab.itemEspacio > tamanoSlot)
        {
            Debug.Log("El objeto es demasiado grande para este slot");
            return;
        }

        ColocarItem(prefab);
    }

    void ColocarItem(ObjetoAlmacenable prefab)
    {
        GameObject obj = Instantiate(prefab.gameObject, transform.position, transform.rotation);
        obj.transform.SetParent(transform);

        objetoAlmacenado = obj.GetComponent<ObjetoAlmacenable>();
        estaOcupado = true;

        InventarioManager.Singleton.LimpiarSeleccion();
    }
}
