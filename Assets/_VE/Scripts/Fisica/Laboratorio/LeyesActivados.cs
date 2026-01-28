using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activa un objeto específico de un arreglo de GameObjects
/// cuando recibe un mensaje del servidor con un índice.
/// </summary>
public class LeyesActivados : MonoBehaviour
{
    [Header("Objetos que pueden activarse")]
    public GameObject[] objetoActivar;

    [Header("Debug")]
    public bool debugEnConsola = false;

    /// <summary>
    /// Se ejecuta al iniciar.
    /// Registra este componente como receptor del mensaje FN00.
    /// </summary>
    private void Start()
    {
        // Validar singleton
        if (GestionMensajesServidor.singeton == null)
        {
            DebugError("GestionMensajesServidor.singeton es NULL. No se pudo registrar la acción.");
            return;
        }

        GestionMensajesServidor.singeton.RegistrarAccion("FN00", ActiServidor);
        DebugLog("Acción FN00 registrada correctamente.");
    }

    /// <summary>
    /// Método llamado desde el servidor.
    /// Convierte el mensaje recibido a entero y activa el objeto correspondiente.
    /// </summary>
    /// <param name="msj">Mensaje recibido que debe contener un número.</param>
    void ActiServidor(string msj)
    {
        if (string.IsNullOrEmpty(msj))
        {
            DebugError("Mensaje recibido vacío o nulo.");
            return;
        }

        int num;

        // Intentar convertir el mensaje
        if (!int.TryParse(msj, out num))
        {
            DebugError("No se pudo convertir el mensaje a entero: " + msj);
            return;
        }

        ActivarObjeto(num);
    }

    /// <summary>
    /// Activa un objeto del arreglo según el índice recibido.
    /// Desactiva todos los demás.
    /// </summary>
    /// <param name="cual">Índice del objeto a activar.</param>
    public void ActivarObjeto(int cual)///////////////////////////////////////// D: poner bool
    {
        if (objetoActivar == null || objetoActivar.Length == 0)
        {
            DebugError("El arreglo objetoActivar está vacío o no asignado.");
            return;
        }


        for (int i = 0; i < objetoActivar.Length; i++)
        {
            if (objetoActivar[i] == null)
            {
                DebugError("Objeto en posición " + i + " es NULL.");
                continue;
            }

            objetoActivar[i].SetActive(i == cual);
        }

        GestionMensajesServidor.singeton.EnviarMensaje("FN00", cual.ToString());

        DebugLog("Objeto activado en índice: " + cual);
    }

    /// <summary>
    /// Imprime mensajes normales si el debug está activo.
    /// </summary>
    void DebugLog(string mensaje)
    {
        if (debugEnConsola)
            Debug.Log("[LeyesActivados] " + mensaje);
    }

    /// <summary>
    /// Imprime mensajes de error si el debug está activo.
    /// </summary>
    void DebugError(string mensaje)
    {
        if (debugEnConsola)
            Debug.LogError("[LeyesActivados] " + mensaje);
    }
}
