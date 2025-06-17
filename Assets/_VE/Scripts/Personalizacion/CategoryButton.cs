using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] private Image icono;
    [SerializeField] private Button button;
    [SerializeField] private Color colorSeleccionado;
    [SerializeField] private Color colorDefecto;

    public Sprite imagenDefecto;  // Imagen por defecto
    public Sprite imagenSeleccion; // Imagen seleccionada

    private static CategoryButton botonActualSeleccionado; // Botón seleccionado actualmente

    private void Start()
    {
        button.onClick.AddListener(presionarBoton);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(presionarBoton);
    }

    private void presionarBoton()
    {
        // Si este botón ya está seleccionado, no hace nada
        if (botonActualSeleccionado == this) return;

        // Cambia el color del botón previamente seleccionado al predeterminado
        if (botonActualSeleccionado != null)
        {
            botonActualSeleccionado.ConfigurarBoton();
        }

        // Marca este botón como seleccionado y actualiza el color
        botonActualSeleccionado = this;
        ConfigurarPorDefecto();
    }

    /// <summary>
    /// Metodo encargado de dejar las configuraciones del boton por defecto
    /// </summary>
    public void ConfigurarBoton()
    {
        icono.color = colorDefecto; // Cambiar al color por defecto cuando se suelta el botó
        icono.sprite = imagenDefecto; // Cambiar a la imagen por defecto cuando se suelta el botón
    }

    /// <summary>
    /// Metodo encargado demodificar las configuraciones del boton por defecto
    /// </summary>
    public void ConfigurarPorDefecto()
    {
        icono.color = colorSeleccionado; // Cambiar al color seleccionado cuando se presiona el botón
        icono.sprite = imagenSeleccion; // Cambiar a la imagen seleccionada cuando se presiona el botón
    }
}
