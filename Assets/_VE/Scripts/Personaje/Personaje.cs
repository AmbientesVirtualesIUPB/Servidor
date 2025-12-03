using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Personaje : MonoBehaviour
{
    public Genero genero;
    public ListaPersonalizable[] elementosPersonalizables;
    public Materializador materiales;
    public MorionID morionID;
    [SerializeField]
    private string url_consulta_p = "CRUD/Read/leer_datos_personalizacion.php";
    public string datosPersonalizacion;
    public GameObject camara;


    private void Start()
	{
        materiales.Inicializar(gameObject);
		for (int i = 0; i < elementosPersonalizables.Length; i++)
		{
            elementosPersonalizables[i].IniciarAleatorio(genero);
		}
        materiales.AleatorizarTPR();
		if (morionID.isOwner && PersonajeBD.instance != null)
		{
			if (PersonajeBD.instance.usuario != null)
			{
                Cargar(PersonajeBD.instance.usuario.personalizacion);
			}
		}
		else
		{
            TraerInformacionPersonalizacion();
		}
	}

    public void ActualizarGraficos()
    {
        for (int i = 0; i < elementosPersonalizables.Length; i++)
        {
            elementosPersonalizables[i].Iniciar(genero);
        }
    }

    [ContextMenu("Guardar")]
    public void Guardar()
	{
        string json = ConvertirATexto();
        if(PersonajeBD.instance != null)
		{
            PersonajeBD.instance.usuario.personalizacion = json;
            PersonajeBD.instance.Guardar();
		}
        PlayerPrefs.SetString("personaje", json);
        print(json);
    }


    public string ConvertirATexto()
	{
        PersonajeGuardable pg = new PersonajeGuardable();
        pg.genero = genero;
        pg.indices = new int[elementosPersonalizables.Length];
        for (int i = 0; i < elementosPersonalizables.Length; i++)
        {
            pg.indices[i] = elementosPersonalizables[i].indice;
        }
        return JsonUtility.ToJson(pg);
    }
    public void Cargar(string json)
    {
		try
		{
            PersonajeGuardable pg = JsonUtility.FromJson<PersonajeGuardable>(json);
            genero = pg.genero;
            for (int i = 0; i < elementosPersonalizables.Length; i++)
            {
                elementosPersonalizables[i].Iniciar(pg.indices[i], genero);
            }
        }
		catch (System.Exception)
		{
            Debug.LogError("Error intentando cargar los datos, probablemente el JSon no tiene la forma esperada: " + json);
			throw;
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
        StartCoroutine(ObtenerPersonalizacion());
    }

    /// <summary>
    /// Currutina encargada de consultar la base de datos y traer la informacion del usuario especificado
    /// </summary>
    /// <param name="idUsuario"> Cedula del usuario </param>
    /// <param name="procesador"> Referencia al scrip procesador de informacion </param>
    /// <returns></returns>
    public IEnumerator ObtenerPersonalizacion()
    {
        yield return new WaitForSeconds(0.5f);
        string idUsuario = morionID.GetID();
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
                Cargar(datosPersonalizacion);
                camara.SetActive(morionID.isOwner);
            }
            else
            {
                // Acciones a realizar
                Debug.LogError("Error al realizar la solicitud: " + www.error);
            }
        }
    }


    [ContextMenu("Cargar")]
    public void Cargar()
    {
        string json = PlayerPrefs.GetString("personaje");
        Cargar(json);
    }

    public void GenerarMasculino()
	{
        genero = Genero.masculino;
        ActualizarGraficos();
    }
    public void GenerarFemenino()
    {
        genero = Genero.femenino;
        ActualizarGraficos();
    }
}

[System.Serializable]
public class PersonajeGuardable
{
    public Genero genero;
    public int[] indices;
}

[System.Serializable]
public class ListaPersonalizable
{
    public string nombre;
    public int indice;
    public GameObject[] objMasculinos;
    public GameObject[] objFemeninos;
    public GameObject[] objNeutro;
    public Genero genero;
    public void Iniciar(Genero g)
	{
        genero = g;
        for (int i = 0; i < objMasculinos.Length; i++)
        {
            objMasculinos[i].SetActive(false);
        }
        for (int i = 0; i < objFemeninos.Length; i++)
        {
            objFemeninos[i].SetActive(false);
        }
        for (int i = 0; i < objNeutro.Length; i++)
        {
            objNeutro[i].SetActive(false);
        }
        switch (g)
		{
			case Genero.masculino:
				if (objMasculinos.Length > 0)
				{
                    objMasculinos[Mathf.Clamp(indice, 0, objMasculinos.Length - 1)].SetActive(true);
				}
				else if (objNeutro.Length > 0)
                {
                    objNeutro[Mathf.Clamp(indice, 0, objNeutro.Length - 1)].SetActive(true);
                }
                break;
			case Genero.femenino:
                if (objFemeninos.Length > 0)
                {
                    objFemeninos[Mathf.Clamp(indice, 0, objFemeninos.Length - 1)].SetActive(true);
                }
                else if (objNeutro.Length > 0)
                {
                    objNeutro[Mathf.Clamp(indice, 0, objNeutro.Length - 1)].SetActive(true);
                }
                
                break;
			case Genero.neutro:
                if (objNeutro.Length > 0)
                {
                    objNeutro[Mathf.Clamp(indice, 0, objNeutro.Length - 1)].SetActive(true);
                }
                break;
			default:
				break;
		}
	}

    public void Iniciar(int _indice)
	{
        Iniciar(_indice, genero);
	}

    public int GetNumeroElementos(Genero g)
    {
        if ((objMasculinos.Length + objFemeninos.Length) == 0 && objNeutro.Length > 0)
            return objNeutro.Length;
        if (g == Genero.femenino)
		{
            return objFemeninos.Length;
		}
		if (g == Genero.masculino)
		{
            return objMasculinos.Length;
		}
        return objNeutro.Length;
	}

    public int GetNumeroElementos()
    {
        return GetNumeroElementos(genero);
    }
    public void Iniciar(int _indice, Genero g)
    {
        indice = _indice;
        Iniciar(g);
    }

    public void IniciarAleatorio(Genero g)
    {
        int max = g == Genero.femenino ? objFemeninos.Length: objMasculinos.Length;
        indice = Random.Range(0,max);
        Iniciar(g);
    }
}


[System.Serializable]
public class Materializador
{
    public List<Material> ACC, FRR, TPR, SKN;  //    ACC = 0,      FRR = 1,      TPR = 2
    public List<int> indices;
    public Gradient colores;

    public void Inicializar(GameObject go)
	{
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        
		for (int i = 0; i < renderers.Length; i++)
		{
			for (int j = 0; j < renderers[i].materials.Length; j++)
			{
                Material m = renderers[i].materials[j];
                //Debug.Log(m.name.Substring(0,3));
				if (m.name.Substring(0,3) == "ACC")
				{
                    renderers[i].materials[j] = m;
                    ACC.Add(m);
				}else if (m.name.Substring(0, 3) == "FRR")
                {
                    renderers[i].materials[j] = m;
                    FRR.Add(m);
                }
                else if (m.name.Substring(0, 3) == "TPR")
                {
                    renderers[i].materials[j] = m;
                    TPR.Add(m);
                }
                else if (m.name.Substring(0, 3) == "SKN")
                {
                    renderers[i].materials[j] = m;
                    SKN.Add(m);
                }
            }
		}

        if (indices.Count < 5)
        {
            indices = new List<int>();
            for (int k = 0; k < 5; k++)
            {
                indices.Add(0);
            }
        }
    }

    public void AleatorizarTPR()
	{
		for (int i = 0; i < TPR.Count; i++)
		{
            TPR[i].SetFloat("_ColorIndice1", Random.Range(0f, 12f));
            TPR[i].SetFloat("_ColorIndice2", Random.Range(0f, 12f));
        }
	}


    public void CambiarColorTPR(int indice, int c)
    {
        for (int i = 0; i < TPR.Count; i++)
        {
            string nombre = "_ColorIndice" + indice;
            TPR[i].SetFloat(nombre, c);
        }
    }

    public void CambiarColorACC(int indice, int c)
    {
        for (int i = 0; i < ACC.Count; i++)
        {
            string nombre = "_ColorIndice" + indice;
            ACC[i].SetFloat(nombre, c);
        }
    }

    public void CambiarColorFRR(int indice, int c)
    {
        for (int i = 0; i < FRR.Count; i++)
        {
            string nombre = "_ColorIndice" + indice;
            FRR[i].SetFloat(nombre, c);
        }
    }

    public void CambiarColorSKN(int indice, int c)
    {
        for (int i = 0; i < SKN.Count; i++)
        {
            string nombre = "_ColorIndice" + indice;
            SKN[i].SetFloat(nombre, c);
            Debug.Log(i + " - Cambiando " + indice + " por " + c);
        }
        Debug.Log("Cambiando " + indice + " por " + c);
    } 

    public void CambiarColor(int opcionPersonalizando, int indice, int color)
    {
        switch (opcionPersonalizando)
        {
            case 0:
                CambiarColorACC(indice, color);
                break;
            case 1:
                CambiarColorFRR(indice, color);
                break;
            case 2:
                CambiarColorTPR(indice, color);
                break;
            case 3:
                CambiarColorSKN(indice, color);
                break;
            default:
                break;
        }
    }
}