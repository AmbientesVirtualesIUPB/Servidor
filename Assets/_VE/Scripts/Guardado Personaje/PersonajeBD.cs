using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PersonajeBD : MonoBehaviour
{
    // La única instancia de la clase
    public static PersonajeBD instance;

    [SerializeField]
    private string url_consulta_p = "CRUD/Read/leer_datos_personalizacion.php";   // URL para consultar la informacion de la personalizacion
    [SerializeField]
    private string url_actualizacion = "CRUD/Update/actualizar_usuario.php"; // URL que guardla la informacion de  personalizacion al momento de ser guardada

    // Datos de usuario, extraidos del script ConsumirApi
    public bool debugEnConsola; // Gestionador de mensajes
    public string datosPersonalizacion;

    public DatosUsuario usuario
    {
        get
        {
            return EnvioDatosBD.singleton.usuario;
        }
        set
        {
            EnvioDatosBD.singleton.usuario = value;
        }
    }



    // Método que se llama al iniciar la clase
    void Awake()
    {
        // Si no hay otra instancia, esta será la principal
        if (instance == null)
        {
            instance = this;

            // Evita que se destruya cuando cambia de escena
            DontDestroyOnLoad(gameObject);
        }
        // Si ya existe otra instancia y no es esta, destruye esta para mantener solo una
        else if (instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    /// <summary>
    /// Metodo invocado desde el script de personalizacion y furtivo, este recibe los datos de la personalizacion
    /// </summary>

    [ContextMenu("Guardar P")]
    public void Guardar()
    {
        StartCoroutine(EnviarDatos());
    }

    /// <summary>
    /// Currutina que crea el formulario y lo envia en solicitud POST para guardar la infomracion de la personalizacion en la BD
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnviarDatos()
    {
        // Creación del formulario
        WWWForm form = new WWWForm();
        form.AddField("id_usuario", usuario.id_usuario);
        form.AddField("datos_json", JsonUtility.ToJson(usuario));

        string url_base = ConfiguracionGeneral.configuracionDefault.url;
        // Enviar la solicitud POST
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_actualizacion, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Almacenamos la respuesta del servidor
                string responseText = www.downloadHandler.text;
                if (debugEnConsola) print("Respuesta del servidor: " + responseText);

                // Verificar si la respuesta contiene un mensaje de error
                if (responseText.Contains("Error"))
                {
                    if (debugEnConsola) print("Respuesta Unity: " + "Error en la solicitud: " + responseText);
                    // Acciones a realizar
                }
                else if (responseText.Contains("Usuario no encontrado"))
                {
                    if (debugEnConsola) print("Respuesta Unity: " + "Usuario no encontrado en la base");
                    // Acciones a realizar
                }
                else
                {
                    if (debugEnConsola) print("Respuesta Unity: " + "Datos enviados con éxito");
                }
            }
            else
            {
                if (debugEnConsola) print("Respuesta Unity: " + "Error al enviar los datos: " + www.error);
            }
        }
    }

    /// <summary>
    ///  Metodo que puede ser invocado para traer la personalizacion que se tenga guardada en la base de datos, pasandole el numero de cedula
    /// </summary>
    /// <param name="id"></param>

    [ContextMenu("Traer Personalizacion")]
    public void TraerInformacionPersonalizacion()
    {
        // Llamamos la currutina
        StartCoroutine(ObtenerPersonalizacion(int.Parse(usuario.id_usuario)));
    }

    /// <summary>
    /// Currutina encargada de consultar la base de datos y traer la informacion del usuario especificado
    /// </summary>
    /// <param name="idUsuario"> Cedula del usuario </param>
    /// <param name="procesador"> Referencia al scrip procesador de informacion </param>
    /// <returns></returns>
    public IEnumerator ObtenerPersonalizacion(int idUsuario)
    {
        // Creación del formulario
        WWWForm form = new WWWForm();
        // Enviamos la cedula que este logueada
        form.AddField("id_usuario", idUsuario);

        string url_base = ConfiguracionGeneral.configuracionDefault.url;

        //Enviamos la solicitud Post
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_consulta_p, form))
        {
            yield return www.SendWebRequest();

            //Si la solicitud es correcta y exitosa
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Acciones a realizar
                Debug.Log("Respuesta recibida: " + www.downloadHandler.text);

                // Asignamos al furtivo que este loggeado la personalizacion guardada en la base de datos

                datosPersonalizacion = www.downloadHandler.text;
            }
            else
            {
                // Acciones a realizar
                Debug.LogError("Error al realizar la solicitud: " + www.error);
            }
        }
    }


    /// <summary>
    /// Currutina encargada de consultar la base de datos y traer la informacion del usuario especificado
    /// </summary>
    /// <param name="idUsuario"> Cedula del usuario </param>
    /// <param name="procesador"> Referencia al scrip procesador de informacion </param>
    /// <returns></returns>
    public IEnumerator ObtenerPersonalizacionExterior(string idUsuario, Personalizacion3 pSalida)
    {
        // Creación del formulario
        WWWForm form = new WWWForm();
        // Enviamos la cedula que este logueada
        form.AddField("id_usuario", idUsuario);

        string url_base = ConfiguracionGeneral.configuracionDefault.url;

        //Enviamos la solicitud Post
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_consulta_p, form))
        {
            yield return www.SendWebRequest();

            //Si la solicitud es correcta y exitosa
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Acciones a realizar
                Debug.Log("Respuesta recibida: " + www.downloadHandler.text);

                // Asignamos al furtivo que este loggeado la personalizacion guardada en la base de datos

                pSalida.CargarDesdeTexto(www.downloadHandler.text);
            }
            else
            {
                // Acciones a realizar
                Debug.LogError("Error al realizar la solicitud: " + www.error);
            }
        }
    }
}
