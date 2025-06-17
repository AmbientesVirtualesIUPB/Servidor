using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGrid : MonoBehaviour
{
    // Grid de colores
    [Header("Grid de Colores")]
    public GameObject colorBotonPrefab; // Prefab de la celda (botón)
    public Transform colorGridParent; // Panel con GridLayoutGroup para los colores
    public int colorGridFilas = 4; // Número de filas
    public List<Color> listaColores1 = new List<Color>(); // Lista de colores configurables desde el Inspector
    public List<Color> listaColores2 = new List<Color>(); // Lista de colores configurables desde el Inspector
    public Gradient gradiente1;
    public Gradient gradiente2;
    public Gradient gradientePiel;
    public List<Color> listaColoresPiel = new List<Color>(); // Lista de colores configurables desde el Inspector
    private Color[,] colorGrid; // Almacena los colores de las celdas

    // Grid de imágenes
    [Header("Grid de Imágenes")]
    public GameObject imagenBotonPrefab; // Prefab para las celdas de imágenes (botón)
    public Transform imagenGridParentExterno, imagenGridParentInterno; // Panel del GridLayoutGroup para imágenes
    public int imagenGridFilas = 3; // Filas del grid de imágenes
    public int imagenGridColumnas = 3; // Columnas del grid de imágenes
    public List<Sprite> listaImagenes = new List<Sprite>(); // Lista de imágenes configurables desde el Inspector

    public ManagerPanels managerPanels;


    void Start()
    {
        listaColores1       = new List<Color>();
        listaColores2       = new List<Color>();
        listaColoresPiel    = new List<Color>();

        for (int i = 0; i <= colorGridFilas; i++)
		{
            listaColores1.Add(gradiente1.Evaluate(i / (float)colorGridFilas));
            listaColores2.Add(gradiente2.Evaluate(i / (float)colorGridFilas));
            listaColoresPiel.Add(gradientePiel.Evaluate(i / (float)colorGridFilas));
        }
        GenerarImagen();
    }

    /// <summary>
    /// Metodo encargado de cargar el panel de los colores generales
    /// </summary>
    public void GenerarColorGeneral()
    {
        LimpiarGrid();


        for (int i = 0; i <= colorGridFilas; i++)
        {
            // Crear una celda a partir del prefab
            GameObject cell = Instantiate(colorBotonPrefab, colorGridParent);

            // Asignar un color desde la lista
            Color assignedColor = listaColores1[i];

            // Cambiar el color de fondo de la celda
            Button b = cell.GetComponent<Button>();
            int r = i;
            b.image.color = assignedColor;
            b.onClick.AddListener(() => ClickBotonColor(1, r));
        }

        for (int i = 0; i < 2; i++)
        {
            // Crear una celda a partir del prefab
            GameObject cell = Instantiate(colorBotonPrefab, colorGridParent);

            // Asignar un color desde la lista
            Color assignedColor = listaColores1[i];

            // Cambiar el color de fondo de la celda
            Button b = cell.GetComponent<Button>();
            int r = i;
            b.image.color = new Color(0,0,0,0);
            //b.onClick.AddListener(() => ClickBotonColor(2, r));
        }

        for (int i = 0; i <= colorGridFilas; i++)
        {
            // Crear una celda a partir del prefab
            GameObject cell = Instantiate(colorBotonPrefab, colorGridParent);

            // Asignar un color desde la lista
            Color assignedColor = listaColores2[i];

            // Cambiar el color de fondo de la celda
            Button b = cell.GetComponent<Button>();
            int r = i;
            b.image.color = assignedColor;
            b.onClick.AddListener(() => ClickBotonColor(2, r));
        }
    }

    /// <summary>
    /// Metodo encargado de cargar el panel de los colores para la piel
    /// </summary>
    public void GenerarColorPiel()
    {
        LimpiarGrid();
            
        for (int i = 0; i <= colorGridFilas; i++)
        {
            // Crear una celda a partir del prefab
            GameObject cell = Instantiate(colorBotonPrefab, colorGridParent);

            // Asignar un color desde la lista
            Color assignedColor = listaColoresPiel[i];

            // Cambiar el color de fondo de la celda
            Button b = cell.GetComponent<Button>();
            int r = i;
            b.image.color = assignedColor;
            b.onClick.AddListener(() => ClickBotonColor(3, r));
        }
    }

    /// <summary>
    /// Genera el grid de imágenes en los botones de personalizacion
    /// </summary>
    public void GenerarImagen()
    {
        if (listaImagenes.Count < imagenGridFilas * imagenGridColumnas)
        {
            Debug.LogError("La lista de imágenes no tiene suficientes elementos para llenar la cuadrícula.");
            return;
        }

        //int imageIndex = 0;

        for (int row = 0; row < imagenGridFilas; row++)
        {
            for (int col = 0; col < imagenGridColumnas; col++)
            {
                //// Instanciar celda externa y asignar imagen
                //GameObject cellExterna = Instantiate(imagenBotonPrefab, imagenGridParentExterno);
                //cellExterna.GetComponent<Image>().sprite = listaImagenes[imageIndex];
                //cellExterna.GetComponent<Image>().pixelsPerUnitMultiplier = 3f;

                //// Instanciar celda interna y asignar imagen
                //GameObject cellInterna = Instantiate(imagenBotonPrefab, imagenGridParentInterno);
                //cellInterna.GetComponent<Image>().sprite = listaImagenes[imageIndex];
                //cellInterna.GetComponent<Image>().pixelsPerUnitMultiplier = 3f;

                //// Capturar índices para usarlos en el callback
                //int r = row, c = col;
                //cellExterna.GetComponent<Button>().onClick.AddListener(() => ClickBotonImagen(r, c));
                //cellInterna.GetComponent<Button>().onClick.AddListener(() => ClickBotonImagen(r, c));

                //imageIndex++;
            }
        }
    }

    /// <summary>
    /// Meotodo invocado al momento de dar click en el boton del panel de colores
    /// </summary>
    /// <param name="row"> fila de identificacion en el panel </param>
    /// <param name="col"> columna de identificacion en el panel </param>
    void ClickBotonColor(int _indice, int _color)
    {
        managerPanels.CambiarColor(_indice, _color);
    }

    /// <summary>
    /// Meotodo invocado al momento de dar click en el boton del panel externo e interno de personalizacion
    /// </summary>
    /// <param name="row"> fila de identificacion en el panel </param>
    /// <param name="col"> columna de identificacion en el panel </param>
    void ClickBotonImagen(int row, int col)
    {
        Debug.Log($"Imagen seleccionada en: ({row}, {col})");
        // Agrega tu lógica aquí
    }


    /// <summary>
    /// Método para limpiar los grid y eliminar las instancias
    /// </summary>
    public void LimpiarGrid()
    {
        // Destruir todos los hijos del panel de colores
        foreach (Transform child in colorGridParent)
        {
            Destroy(child.gameObject);
        }
    }
}
