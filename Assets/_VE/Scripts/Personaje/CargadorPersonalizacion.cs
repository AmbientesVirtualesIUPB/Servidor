using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class CargadorPersonalizacion : MonoBehaviour
{
    [InfoMessage("Este es un campo OBLIGATORIO para que funcione, asegúrate de configurarlo correctamente.", MessageTypeCustom.Error)]
    public Personalizacion3 basePersonalizacion;
    [InfoMessage("Este es un campo OBLIGATORIO para que funcione, asegúrate de configurarlo correctamente.", MessageTypeCustom.Error)]
    public MorionID morionID; 
    [SerializeField]
    private string url_consulta_p = "CRUD/Read/leer_datos_personalizacion.php";   // URL para consultar la informacion de la personalizacion

    public string datosPersonalizacion;

    private void Start()
    {

        Cargar();
    }

    [ContextMenu("Cargar")]
    public void Cargar()
    {
        if (EnvioDatosBD.singleton != null && morionID.isOwner)
        {
            basePersonalizacion.CargarDesdeTexto(EnvioDatosBD.singleton.usuario.personalizacion);
        }
        else if (EnvioDatosBD.singleton != null && !morionID.isOwner)
        {
            TraerInformacionPersonalizacion();
        }
        else
        {
            basePersonalizacion.CuerpoAleatorio();
        }
    }


    [ContextMenu("Traer Personalizacion")]
    public void TraerInformacionPersonalizacion()
    {
        // Llamamos la currutina
        StartCoroutine(ObtenerPersonalizacion(morionID.GetID()));
    }

    /// <summary>
    /// Currutina encargada de consultar la base de datos y traer la informacion del usuario especificado
    /// </summary>
    /// <param name="idUsuario"> Cedula del usuario </param>
    /// <returns></returns>
    public IEnumerator ObtenerPersonalizacion(string idUsuario)
    {
        yield return new WaitForNextFrameUnit();
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
                basePersonalizacion.CargarDesdeTexto(datosPersonalizacion);
            }
            else
            {
                // Acciones a realizar
                Debug.LogError("Error al realizar la solicitud: " + www.error);

            }
        }
    }
}
