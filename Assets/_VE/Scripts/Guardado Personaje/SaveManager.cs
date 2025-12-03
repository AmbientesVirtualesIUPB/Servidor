using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{

    //Referenciamos el archivo donde guardaremos la informacion
    [SerializeField]
    private SaveSplit split;

    public EnvioDatosBD managerBD;

    
    private void Start()
    {
        // Busca el objeto por nombre para buscar la referencia al objeto que administra la base de datos, ya que este pasar� entre escenas
        GameObject obj = GameObject.Find("EnvioBD");

        if (obj != null)
        {
            managerBD = obj.GetComponent<EnvioDatosBD>(); // Si encuentra el objeto lo almacenamos en la variable
        }
        else
        {
            managerBD = null;
        }
    }

    /// <summary>
    /// Metodo para el guardado de archivos JSON
    /// </summary>
    public void Save()
    {
        //Conovertimos el objeto a formato Json
        string splitJson = JsonUtility.ToJson(split);
        //Generamos una ubicacion en disco, persistente para que funcione en cualquier plataforma
        string path = Path.Combine(Application.persistentDataPath, "splitData.data");

        // Imprimir ruta del archivo guardado
        // Debug.Log(path);

        //Guardamos el archibo json
        File.WriteAllText(path, splitJson);
    }

    /// <summary>
    /// Metodo para la carga de archivos JSON
    /// </summary>
    public void Load()
    {
        //Traemos la ruta del archivo
        string path = Path.Combine(Application.persistentDataPath, "splitData.data");

        //Validamos si ya existe un archivo de guardado actual
        if (File.Exists(path))
        {
            //leemos el archivo Json
            string splitJson = File.ReadAllText(path);
            //Convertimos el archivo Json a objeto unity
            SaveSplit splitLoad = JsonUtility.FromJson<SaveSplit>(splitJson);

            split.posiciones = splitLoad.posiciones;
            split.colores = splitLoad.colores;
            split.furtivos = splitLoad.furtivos;
        }
        // Sino existe creamos uno por defecto
        else
        {
            Save();
        }
    }

    /// <summary>
    /// Metodo invocado desde el scrip de personalización, para grabar los datos de las posiciones
    /// </summary>
    /// <param name="texto"> Parametro de texto con las posiciones </param>
    public void PersonalizacionPersonaje(string texto)
    {
        split.posiciones = texto;
        //Guardamos
        Save();
    }

    /// <summary>
    /// Metodo invocado desde el scrip de personalización, para grabar los datos de las posiciones de los colores
    /// </summary>
    /// <param name="texto"> Parametro de texto con las posiciones </param>
    public void PersonalizacionColores(string texto)
    {
        split.colores = texto;
        //Guardamos
        Save();
    }


    /// <summary>
    /// Metodo invocado desde el script de personalización y personalizacionFurtivo, en el Awake
    /// </summary>
    public void CargarDatos()
    {
        //Cargamos
        Load();
    
    }
}


