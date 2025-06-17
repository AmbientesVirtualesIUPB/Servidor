using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPanels : MonoBehaviour
{
    // Variables para el manejo de paneles
    [Header("Paneles")]
    public GameObject panelColores;
    public GameObject panelExterno;
    public GameObject panelExternoBotonera;
    public GameObject panelInterno;
    public GameObject panelInternoBotonera;
    public GameObject panelBotones;
    public GameObject panelCategorias;
    public GameObject panelCategoriaRostro;
    public GameObject panelCategoriaRopa;

    // Variables para el manejo de las interfaces
    [Header("Interfaces")]
    public GameObject interfazGenero;
    public GameObject interfazCreacion;
    public GameObject cargando;

    // Variables para el manejo de los botones
    [Header("Botones")]
    public GameObject btnBack;
    public GameObject btnGuardar;

    // Variables para el manejo de referencias adicionales
    [Header("Referencias")]
    public ZoomCamara ZoomCamara;
    [SerializeField]
    private ManagerGrid managerGrid;
    public GestorGraficosPersonalizable gestorGraficos;

    public int opcionPersonalizando = 0;


    public Personaje personaje;

    /// <summary>
    /// Metodo invocado desde el boton btnBack en el encabezado
    /// </summary>
    public void VolverInicio()
    {
        ZoomCamara.VistaCuerpo(); // Posicionamos la camara

        //true
        interfazGenero.SetActive(true);

        //fale
        interfazCreacion.SetActive(false);
        btnBack.SetActive(false);
        btnGuardar.SetActive(false);   
    }

    /// <summary>
    /// Metodo invocado desde los botones de seleccion de genero: Masculino, Femenino y Otro
    /// </summary>
    public void SeleccionGenero()
    {
        ZoomCamara.VistaRostro(); // Posicionamos la camara

        //true
        interfazCreacion.SetActive(true);
        btnBack.SetActive(true);
        btnGuardar.SetActive(true);

        //false
        //interfazGenero.SetActive(false);    
    }

    /// <summary>
    /// Metodo invocado desde los botones de las categorias para el manejo de los paneles
    /// </summary>
    public void PersonalizarCara()
    {
        ZoomCamara.VistaRostro(); // Posicionamos la camara

        //true
        panelCategorias.SetActive(true);
        panelCategoriaRostro.SetActive(true);
        panelColores.SetActive(true);

        //false
        panelCategoriaRopa.SetActive(false);
        panelInterno.SetActive(false);
        panelExterno.SetActive(false);
        
        managerGrid.LimpiarGrid(); //Limpiamos el grid de colores
    }

    /// <summary>
    /// Metodo invocado desde los botones de las categorias para el manejo de los paneles
    /// </summary>
    /// <param name="index"> Identificacion que se le da al boton en el inspector para saber que comportamiento dar</param>
    public void PersonalizarPartesCara(int index)
    {
        opcionPersonalizando = 3;
        //Validamos dependiendo del indice que se elija y generamos colores deseamos y activamos panel deseado
        if (index == 1)
        {
            managerGrid.GenerarColorPiel();
            panelExterno.SetActive(true);
            gestorGraficos.CrearBotonera(panelExternoBotonera.transform, 0);

        }
        else if (index == 2)
        {
            managerGrid.GenerarColorGeneral();
            panelExterno.SetActive(false);
        }
        else
        {
            managerGrid.GenerarColorGeneral();
            panelExterno.SetActive(true);
            gestorGraficos.CrearBotonera(panelExternoBotonera.transform, 1);
        }     
    }

    /// <summary>
    /// Metodo invocado desde los botones de las categorias para el manejo de los paneles
    /// </summary>
    public void PersonalizarCuerpo()
    {
        opcionPersonalizando = 2;
        ZoomCamara.VistaCuerpo(); // Posicionamos la camara

        //true
        panelCategorias.SetActive(true);
        panelCategoriaRopa.SetActive(true);

        //false
        panelColores.SetActive(true);
        panelInterno.SetActive(false);
        panelExterno.SetActive(false);
        
        managerGrid.GenerarColorGeneral(); // Generamos panel de color
    }

    /// <summary>
    /// Metodo invocado desde los botones de las categorias para el manejo de los paneles
    /// </summary>
    /// <param name="index"> Identificacion que se le da al boton en el inspector para saber que comportamiento dar</param>
    public void PersonalizarPartesCuerpo(int index)
    {
        if (index == 1)
        {
            ZoomCamara.VistaPies();
            gestorGraficos.CrearBotonera(panelExternoBotonera.transform, 4);
        }
        else
        {
            ZoomCamara.VistaCuerpo();
            gestorGraficos.CrearBotonera(panelExternoBotonera.transform, 3);
        } 
        panelExterno.SetActive(true);
    }

    /// <summary>
    /// Metodo invocado desde los botones de las categorias para el manejo de los paneles
    /// </summary>
    /// <param name="index"> Identificacion que se le da al boton en el inspector para saber que comportamiento dar</param>
    public void PersonalizarFrente(int index)
    {
        if (index == 1)
        {
            opcionPersonalizando = 1;
            managerGrid.GenerarColorGeneral();
            ZoomCamara.VistaRostro();
            gestorGraficos.CrearBotonera(panelInternoBotonera.transform, 2);
        }
        else if (index == 2)
        {
            managerGrid.GenerarColorPiel();
            ZoomCamara.VistaCuerpo();
        }
        else if(index == 0) // Maleta
        {
            managerGrid.GenerarColorGeneral();
            ZoomCamara.VistaCuerpo();
            gestorGraficos.CrearBotonera(panelInternoBotonera.transform, 5);
        }
        else if (index == 3) // Accesorios
        {
            managerGrid.GenerarColorGeneral();
            ZoomCamara.VistaCuerpo();
            gestorGraficos.CrearBotonera(panelInternoBotonera.transform, 6);

        }else if (index == 4) // Sombreros
        {
            managerGrid.GenerarColorGeneral();
            ZoomCamara.VistaRostro();
            gestorGraficos.CrearBotonera(panelInternoBotonera.transform, 7);
        }


        panelColores.SetActive(true);  
        panelCategorias.SetActive(false);
        panelExterno.SetActive(false);
        panelInterno.SetActive(true);

    }

    /// <summary>
    /// Meotodo invocado desde el boton btnGuardar en el encabezado para guardar el avatar
    /// </summary>
    public void GuardarAvatar()
    {
        personaje.Guardar();
        StartCoroutine(CargandoAvatar());
    }

    /// <summary>
    /// Currutina encargada de mostrar el guardado del personaje
    /// </summary>
    public IEnumerator CargandoAvatar()
    {
        cargando.SetActive(true);
        yield return new WaitForSeconds(2f);
        cargando.SetActive(false);

        // Logica adicional
    }

   public void CambiarColor(int indice, int color)
	{
        gestorGraficos.personaje.materiales.CambiarColor(opcionPersonalizando, indice, color);
        print("Cambiando el " + opcionPersonalizando + " - " + indice + " - " + color);
	}
}
