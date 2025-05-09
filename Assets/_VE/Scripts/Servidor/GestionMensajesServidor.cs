using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Morion Servidor/Gestion Mensajes Servidor")]
public class GestionMensajesServidor : MonoBehaviour
{
    public static GestionMensajesServidor singeton;
	public bool debugEnConsola = false;
	private Dictionary<string, Action<string>> acciones = new Dictionary<string, Action<string>>();

	private void Awake()
	{
		if (singeton != null)
		{
			DestroyImmediate(this);
		}
		else
		{
			singeton = this;
		}
	}

	public void RecibirMensaje(string mensaje)
	{
		if (debugEnConsola) print("MENSAJE:" + mensaje);
		string codigo = mensaje.Substring(0, 4);
		string msj = mensaje.Substring(4);
		EjecutarAccion(codigo, msj);
	}
	public void EnviarMensaje(string msj)
	{
		Servidor.singleton.EnviarMensaje(msj);
	}

	public void EnviarMensaje(string codigo, string msj)
	{
		Servidor.singleton.EnviarMensaje(codigo + msj);
	}


	// M�todo para registrar una acci�n
	public void RegistrarAccion(string palabraClave, Action<string> accion)
	{
		if (!acciones.ContainsKey(palabraClave))
		{
			acciones.Add(palabraClave, accion);
			if (debugEnConsola) Debug.Log($"Acci�n registrada: {palabraClave}");
		}
		else
		{
			Debug.LogWarning($"La palabra clave '{palabraClave}' ya tiene una acci�n registrada.");
		}
	}

	// M�todo para ejecutar una acci�n basada en una palabra clave
	public void EjecutarAccion(string palabraClave, string parametro)
	{
		if (acciones.TryGetValue(palabraClave, out Action<string> accion))
		{
			accion.Invoke(parametro); // Llama al m�todo registrado con el par�metro dado
		}
		else
		{
			Debug.LogWarning($"No se encontr� ninguna acci�n registrada para la palabra clave '{palabraClave}'.");
		}
	}

}
