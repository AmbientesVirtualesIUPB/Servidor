using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class EnvioDatosBD : MonoBehaviour
{
    //URL's
    [SerializeField]
    private string url_creacion_furtivo = "CRUD/Create/insertar_datos_personalizacion_furtivo.php"; // URL que guardla la informacion de  personalizacion al momento de ser guardada
    [SerializeField]
    private string url_creacion_usuario = "CRUD/Create/insertar_datos_usuario.php"; // URL para guardar la informacion de los usuarios
    [SerializeField]
    private string url_lectura_usuario = "CRUD/Read/login.php";
    [SerializeField]
    private string url_consulta_personalizacion = "CRUD/Read/leer_datos_personalizacion.php";   // URL para consultar la informacion de la personalizacion
    [SerializeField]
    private string url_actualizar_personalizacion = "CRUD/Update/actualizar_usuario.php"; // URL que guardla la informacion de  personalizacion al momento de ser guardada

    public string correo;
    public string datosPersonalizacion;
    public bool debugEnConsola; // Gestionador de mensajes
    public string datosFurtivo;

    public static EnvioDatosBD singleton;
    public DatosUsuario usuario;
    public string escenaACargar = "";

    void Awake()
    {
        // Si la instancia ya existe y no es esta, destruir la nueva
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Asignar la instancia a esta y asegurarse de que no se destruya
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    /// <summary>
    /// Metodo invocado desde el script de personalizacion y furtivo, este recibe los datos de la personalizacion
    /// </summary>
    public void EnviarDatosFurtivo(string f)
    {
        StartCoroutine(EnviarDatosPersonalizacionFurtivo(f));
    }

    /// <summary>
    /// Metodo invocado desde el script de consumirAPi, este recibe los datos del usuari y lo crea en la BD
    /// </summary>
    public void EnviarDatosUsuario()
    {
        StartCoroutine(EnviarDatosU());
    }

    /// <summary>
    /// Currutina que crea el formulario y lo envia en solicitud POST para guardar la infomracion de la personalizacion en la BD
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnviarDatosPersonalizacionFurtivo(string f)
    {
        // Creación del formulario
        WWWForm form = new WWWForm();
        form.AddField("furtivos", f);

        string url_base = ConfiguracionGeneral.configuracionDefault.url;
        // Enviar la solicitud POST
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_creacion_furtivo, form))
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
                else
                {
                    if (debugEnConsola) print("Respuesta Unity: " + "Datos enviados con éxito");
                }
            }
            else
            {
                if (debugEnConsola) print("Respuesta Unity: " + "Error al enviar los datos: " + www.error);
            }
            //tengo una tabla en sql con un campo llamado furtivo, necesito enviarle un string, separarlo como una cadena separada con este caracter | y que lo inserte en ese campo ya tengo un codigo php, modifica el codigo para este proceso 
        }
    }

    /// <summary>
    /// Currutina que crea el formulario y lo envia en solicitud POST para guardar la infomracion del usuario en la BD
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnviarDatosU()
    {
        // Creación del formulario
        WWWForm form = new WWWForm();
        form.AddField("id_usuario", usuario.id_usuario);
        form.AddField($"personalizacion", "{\"genero\":0,\"color1\":17,\"color2\":17,\"cuerpo\":0,\"colorPiel\":1,\"cabeza\":5,\"cabello\":2,\"colorCabello\":1,\"cejas\":2,\"zapatos\":10,\"sombrero\":0,\"accesorios\":0}");
        form.AddField("tiempo_uso", 0);
        form.AddField("num_conexiones", 0);
        form.AddField("nombre", usuario.nombre);
        form.AddField("tipo_usuario", usuario.tipo_usuario);
        form.AddField("programa", usuario.programa);
        form.AddField("facultad", usuario.facultad);

        string url_base = ConfiguracionGeneral.configuracionDefault.url;
        // Enviar la solicitud POST
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_creacion_usuario, form))
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
                else if (responseText.Contains("El usuario ya existe en el sistema"))
                {
                    if (debugEnConsola) print("Respuesta Unity: " + "El usuario ya esta creado");
                    // Acciones a realizar
                }
                else
                {
                    if (debugEnConsola) print("Respuesta Unity: " + "Datos enviados con éxito");
                    print(responseText);
                }
            }
            else
            {
                if (debugEnConsola) print("Respuesta Unity: " + "Error al enviar los datos: " + www.error);
            }
        }

        // Creación del formulario
        WWWForm form2 = new WWWForm();
        form2.AddField("id_usuario", usuario.id_usuario);

        string url_base2 = ConfiguracionGeneral.configuracionDefault.url;
        // Enviar la solicitud POST
        using (UnityWebRequest www = UnityWebRequest.Post(url_base2 + url_lectura_usuario, form2))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Almacenamos la respuesta del servidor
                string responseText = www.downloadHandler.text;
                if (debugEnConsola) print("Respuesta del servidor: " + responseText);

                if (www.downloadHandler.text != "[]")
                {
                    Debug.Log(responseText);
                    usuario = JsonUtility.FromJson<DatosUsuario>(responseText);
                    usuario.correo = correo;
                }

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
                    if (debugEnConsola) print("Respuesta Unity: Datos enviados con éxito");
                    CambioScena();
                }
            }
            else
            {
                if (debugEnConsola) print("Respuesta Unity: " + "Error al enviar los datos: " + www.error);
            }
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
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_actualizar_personalizacion, form))
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
    /// Currutina encargada de consultar la base de datos y traer la informacion del usuario especificado
    /// </summary>
    /// <param name="idUsuario"> Cedula del usuario </param>
    /// <param name="procesador"> Referencia al scrip procesador de informacion </param>
    /// <returns></returns>
    public IEnumerator ObtenerPersonalizacionExterior(string idUsuario/*, Personalizacion3 pSalida*/)
    {
        // Creación del formulario
        WWWForm form = new WWWForm();
        // Enviamos la cedula que este logueada
        form.AddField("id_usuario", idUsuario);

        string url_base = ConfiguracionGeneral.configuracionDefault.url;

        //Enviamos la solicitud Post
        using (UnityWebRequest www = UnityWebRequest.Post(url_base + url_consulta_personalizacion, form))
        {
            yield return www.SendWebRequest();

            //Si la solicitud es correcta y exitosa
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Acciones a realizar
                Debug.Log("Respuesta recibida: " + www.downloadHandler.text);

                // Asignamos al furtivo que este loggeado la personalizacion guardada en la base de datos

                //pSalida.CargarDesdeTexto(www.downloadHandler.text);
            }
            else
            {
                // Acciones a realizar
                Debug.LogError("Error al realizar la solicitud: " + www.error);
            }
        }
    }

    public void CambioScena()
    {
        StartCoroutine(CambiarEscena(escenaACargar));
    }

    public void SalirScena()
    {
        Application.Quit();
    }

    public IEnumerator CambiarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
        yield return new WaitForSeconds(1f);
    }
}

[Serializable]
// Clase que Representa los datos anidados dentro de la respuesta
public class DatosUsuario
{
    public string id_usuario;
    public string personalizacion;
    public string tiempo_uso;
    public string num_conexiones;
    public string nombre;
    public string tipo_usuario;
    public string programa;
    public string facultad;
    public string correo;
}