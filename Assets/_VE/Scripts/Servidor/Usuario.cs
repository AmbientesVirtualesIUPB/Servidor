using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MorionID))]
public class Usuario : MonoBehaviour
{
    MorionID morionID;
	public MorionTransform morionTransform;
	public bool autoGenerarID;
	public bool autoPresentacion;
    ///-public Personalizacion3 personaje;
    public GameObject camara;
	public bool debugEnConsola = false;
	public float tiempoVida = 60;

	float vida;

    ///-public Personalizacion3 personalizacion;

    private void Awake()
	{
        morionID = GetComponent<MorionID>();
		if (morionTransform == null)
		{
			morionTransform = GetComponent<MorionTransform>();
		}
		if (morionTransform == null)
		{
			morionTransform = GetComponentInChildren<MorionTransform>();
		}
        if (autoGenerarID)
        {
            morionID.GenerarID();
		}
		else
		{
			if(EnvioDatosBD.singleton != null && EnvioDatosBD.singleton.usuario != null && EnvioDatosBD.singleton.usuario.id_usuario.Length > 0)
			{
                morionID.SetID(EnvioDatosBD.singleton.usuario.id_usuario);
			}
			
		}
    }
	IEnumerator Start()
    {
		if (morionID.isOwner)
		{
			
			if (ControlUsuarios.singleton != null)
			{
				ControlUsuarios.singleton.usuarioLocal = this;
			}
			else
			{
				Debug.LogWarning("No se encuentra una instancia del control de usuarios");
			}
			if (autoPresentacion && Servidor.singleton != null)
			{
				yield return new WaitUntil(() => Servidor.singleton.conectado);
				GestionMensajesServidor.singeton.EnviarMensaje("PR00", GetPresentacion());
			}
			InvokeRepeating("NotificarVida", tiempoVida * 0.8f, tiempoVida * 0.8f);
		}
		else
		{
			camara.SetActive(false);
			if (debugEnConsola)
			{
				print("Tratando de cargar los datos de: " + morionID.GetID());
			}
		}
    }

	void NotificarVida()
	{
        if(morionID.isOwner) 
			GestionMensajesServidor.singeton.EnviarMensaje("PV00", GetMorionID().GetID());
    }

	void IrMuriendo()
	{
		vida--;
		if (vida<=0)
		{
			Morir();
		}
	}

	public void Medicar()
	{
		vida = tiempoVida;
	}

	void Morir()
	{
		///////////////////////// QUitar el personaje del registro
		///
		ControlUsuarios.singleton.EliminarUsuario(morionID.GetID());
	}

	public MorionID GetMorionID()
	{
		return morionID;
	}

	public void Inicializar(string presentacion)
	{
		Presentacion p = JsonUtility.FromJson<Presentacion>(presentacion);
		transform.position = p.posicion;
		transform.eulerAngles = p.rotacion;
		morionID.SetID(p.id_con);
		gameObject.name = "Personaje - " + p.id_con;
		
		/////////////////////////////////////////// Falta configurar la plataforma y el id usuario
		
		if (morionTransform != null)
		{
			morionTransform.posicionObjetivo = p.posicion;
			morionTransform.rotacionObjetivo = p.rotacion;
		}


		if (!morionID.isOwner)
		{
			///-StartCoroutine(PersonajeBD.instance.ObtenerPersonalizacionExterior(morionID.GetID(), personalizacion));
		}
	}

	public void Inicializar(string presentacion, bool owner)
	{
		if (debugEnConsola)
		{
			print("Inicializando el json: " + presentacion + " \n para un owner en " + owner);
		}
		morionID.isOwner = owner;
		Inicializar(presentacion);
	}


	public string GetPresentacion()
	{
		Presentacion p = new Presentacion();
		p.posicion = transform.position;
		p.rotacion = transform.eulerAngles;
		p.id_con = morionID.GetID();
		/////////////////////////////////// Pendiente configurar plataforma y ID_Usuario
		return (JsonUtility.ToJson(p));
	}
}
