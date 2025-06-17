using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personalizacion3 : MonoBehaviour
{
    public PersonalizacionJSON personalizacion;
    public Color[]          colores1;
    public Color[]          colores2;
    public Color[]          coloresPiel;
    public Color[]          coloresCabello;

    [Header("Cuerpos")]
    public Shader           shaderTPR;
    private Material        matTPR;
    public ObjetoPersonalizable[] cuerposHombre;
    public ObjetoPersonalizable[] cuerposMujer;

    [Header("Cabezas")]
    public Shader           shaderSKN;
    private Material        matSKN;
    public ObjetoPersonalizable[] cabezasHombre;
    public ObjetoPersonalizable[] cabezasMujer;

    [Header("Cabello")]
    public Shader shaderFRR;
    private Material matFRR;
    public ObjetoPersonalizable[] cabellosHombre;
    public ObjetoPersonalizable[] cabellosMujer;

    [Header("Cejas")]
    public ObjetoPersonalizable[] cejasHombre;
    public ObjetoPersonalizable[] cejasMujer;

    [Header("Otros")]
    public ObjetoPersonalizable[] zapatos;
    public ObjetoPersonalizable[] sombreros;
    public ObjetoPersonalizable[] accesorios;

    public bool debugEnConsola = false;

    public MessageOnly mensajeX = new MessageOnly("Este Script de personalización aplica para una sola plataforma ", MessageTypeCustom.Info);


	private void Start()
	{
        Inicializar();
	}
	public void Inicializar()
    {
        matTPR = new Material(shaderTPR);
        matSKN = new Material(shaderSKN);
        matFRR = new Material(shaderFRR);
        Pintar();
        Materializar();
    }
    /// <summary>
    /// Esta funcion sirve para pasar por todos los Renderer de los objetos y reemplazar los materiales 
    /// por los materiales que se usarían en la personalización final
    /// </summary>
    void Materializar()
	{
        void EncontrarMateriales(ObjetoPersonalizable[] listado)
		{
            for (int i = 0; i < listado.Length; i++)
            {
                Material[] sharedMats = listado[i].renderer.sharedMaterials;
                for (int j = 0; j < listado[i].renderer.sharedMaterials.Length; j++)
                {
					if (listado[i].renderer.sharedMaterials[j] != null)
					{
                        if (listado[i].renderer.sharedMaterials[j].name.Substring(0, 3).Equals("TPR"))
                        {
                            sharedMats[j] = matTPR;
                        }
                        if (listado[i].renderer.sharedMaterials[j].name.Substring(0, 3).Equals("SKN"))
                        {
                            sharedMats[j] = matSKN;
                        }
                        if (listado[i].renderer.sharedMaterials[j].name.Substring(0, 3).Equals("FRR"))
                        {
                            sharedMats[j] = matFRR;
                        }
                    }
                }
                listado[i].renderer.sharedMaterials = sharedMats;
            }
        }

        EncontrarMateriales(cuerposHombre);
        EncontrarMateriales(cuerposMujer);
        EncontrarMateriales(cabezasHombre);
        EncontrarMateriales(cabezasMujer);
        EncontrarMateriales(cabellosMujer);
        EncontrarMateriales(cabellosHombre);
        EncontrarMateriales(cejasMujer);
        EncontrarMateriales(cejasHombre);
        //EncontrarMateriales(zapatos);


    }

    public void Pintar()
    {
        int indice;
        void Desactivador(ObjetoPersonalizable[] lista)
        {
            for (int i = 0; i < lista.Length; i++)
            {
                lista[i].renderer.gameObject.SetActive(false);
            }
        }
        Desactivador(cuerposHombre);
        Desactivador(cuerposMujer);
        Desactivador(cabezasHombre);
        Desactivador(cabezasMujer);
        Desactivador(cabellosHombre);
        Desactivador(cabellosMujer);
        Desactivador(cejasMujer);
        Desactivador(cejasHombre);
        Desactivador(zapatos);
        Desactivador(sombreros);
        Desactivador(accesorios);



        switch (personalizacion.genero)
		{
			case Genero.masculino:
                indice = personalizacion.cuerpo;
                matTPR.SetTexture("_Base", cuerposHombre[indice].txBase);
                matTPR.SetTexture("_Rugosidad", cuerposHombre[indice].txRugosidad);
                matTPR.SetTexture("_Mascaras", cuerposHombre[indice].txRGB);

                cuerposHombre[personalizacion.cuerpo].renderer.gameObject.SetActive(true);

                indice = personalizacion.cabeza;
                matSKN.SetTexture("_Base", cabezasHombre[indice].txBase);
                matSKN.SetTexture("_Rugosidad", cabezasHombre[indice].txRugosidad);

                cabezasHombre[Mathf.Clamp(personalizacion.cabeza, 0, cabezasHombre.Length - 1)].renderer.gameObject.SetActive(true);

                cabellosHombre[Mathf.Clamp(personalizacion.cabello, 0, cabellosHombre.Length - 1)].renderer.gameObject.SetActive(true);
                cejasHombre[Mathf.Clamp(personalizacion.cejas, 0, cejasHombre.Length - 1)].renderer.gameObject.SetActive(true);
                break;
			case Genero.femenino:
                indice = personalizacion.cuerpo;
                matTPR.SetTexture("_Base", cuerposMujer[indice].txBase);
                matTPR.SetTexture("_Rugosidad", cuerposMujer[indice].txRugosidad);
                matTPR.SetTexture("_Mascaras", cuerposMujer[indice].txRGB);

                cuerposMujer[personalizacion.cuerpo].renderer.gameObject.SetActive(true);

                indice = personalizacion.cabeza;
                matSKN.SetTexture("_Base", cabezasMujer[indice].txBase);
                matSKN.SetTexture("_Rugosidad", cabezasMujer[indice].txRugosidad);

                cabezasMujer[Mathf.Clamp(personalizacion.cabeza, 0, cabezasMujer.Length-1)].renderer.gameObject.SetActive(true);

                cabellosMujer[Mathf.Clamp(personalizacion.cabello, 0, cabellosMujer.Length - 1)].renderer.gameObject.SetActive(true);
                cejasMujer[Mathf.Clamp(personalizacion.cejas, 0, cejasMujer.Length - 1)].renderer.gameObject.SetActive(true);
                break;
			case Genero.neutro:
				break;
			default:
				break;
		}

        zapatos[Mathf.Clamp(personalizacion.zapatos, 0, zapatos.Length - 1)].renderer.gameObject.SetActive(true);
        sombreros[Mathf.Clamp(personalizacion.sombrero, 0, sombreros.Length - 1)].renderer.gameObject.SetActive(true);
        accesorios[Mathf.Clamp(personalizacion.accesorios, 0, accesorios.Length - 1)].renderer.gameObject.SetActive(true);

        matTPR.SetColor("_Color1", colores1[personalizacion.color1]);
        matTPR.SetColor("_Color2", colores1[personalizacion.color2]);

        matSKN.SetColor("_Color1", coloresPiel[personalizacion.colorPiel]);

        matFRR.SetColor("_Color1", coloresCabello[personalizacion.colorCabello]);

    }

    public string ConvertirATexto()
	{
        return (JsonUtility.ToJson(personalizacion));
	}

    public void CargarDesdeTexto(string t)
	{
		if (debugEnConsola)
		{
            print("Cargó el JSON: " + t);
		}
        if ("personalizacion".Equals(t))
        {
            personalizacion = new PersonalizacionJSON();
        }
        else
        {
            personalizacion = JsonUtility.FromJson<PersonalizacionJSON>(t);
        }
        Pintar();
	}


    [ContextMenu("Guardar")]
    public void Guardar()
    {
        string json = ConvertirATexto();
        if (PersonajeBD.instance != null)
        {
            PersonajeBD.instance.usuario.personalizacion = json;
            PersonajeBD.instance.Guardar();
        }
        PlayerPrefs.SetString("personaje", json);
        print(json);
    }

    public void AsignarCuerpo(int cual)
	{
        personalizacion.cuerpo = cual;
        Pintar();
    }
    public void AsignarColor1(int cual)
    {
        personalizacion.color1 = cual;
        Pintar();
    }
    public void AsignarColor2(int cual)
    {
        personalizacion.color2 = cual;
        Pintar();
    }
    public void AsignarColorPiel(int cual)
    {
        personalizacion.colorPiel = cual;
        Pintar();
    }
    public void AsignarCabeza(int cual)
    {
        personalizacion.cabeza = cual;
        Pintar();
    }
    public void AsignarCejas(int cual)
    {
        personalizacion.cejas = cual;
        Pintar();
    }
    public void AsignarColorCabello(int cual)
    {
        personalizacion.colorCabello = cual;
        Pintar();
    }
    public void AsignarCabello(int cual)
    {
        personalizacion.cabello = cual;
        Pintar();
    }
    public void AsignarZapatos(int cual)
    {
        personalizacion.zapatos = cual;
        Pintar();
    }
    public void AsignarSombrero(int cual)
    {
        personalizacion.sombrero = cual;
        Pintar();
    }
    public void AsignarAccesorio(int cual)
    {
        personalizacion.accesorios = cual;
        Pintar();
    }

    public void CambiarGenero(Genero g)
	{
        personalizacion.genero = g;
	}


    public void CuerpoAleatorio()
	{
		if (Random.Range(0,100)<50)
		{
            CambiarGenero(Genero.masculino);
		}
		else
		{
            CambiarGenero(Genero.femenino);
		}

        AsignarAccesorio(Random.Range(0,accesorios.Length));
        AsignarSombrero(Random.Range(0, sombreros.Length));
        AsignarZapatos(Random.Range(0, zapatos.Length));
        AsignarCabello(Random.Range(0, Mathf.Min(cabellosHombre.Length, cabellosMujer.Length)));
        AsignarCejas(Random.Range(0, Mathf.Min(cejasMujer.Length, cejasHombre.Length)));
        AsignarCabeza(Random.Range(0, Mathf.Min(cabezasMujer.Length, cabezasMujer.Length)));
        AsignarCuerpo(Random.Range(0,  Mathf.Min(cuerposMujer.Length, cuerposHombre.Length)));

        AsignarColor1(Random.Range(0, colores1.Length));
        AsignarColor2(Random.Range(0, colores2.Length));
        AsignarColorPiel(Random.Range(0, coloresPiel.Length));
        AsignarColorCabello(Random.Range(0, coloresCabello.Length));
    }
}

[System.Serializable]
public class ObjetoPersonalizable
{
    public Sprite sprite;
    public Renderer renderer;
    public Texture2D txBase;
    public Texture2D txRGB;
    public Texture2D txRugosidad;
}


[System.Serializable]
public class PersonalizacionJSON
{
    public Genero genero;
    public int color1;
    public int color2;
    public int cuerpo;
    public int colorPiel;
    public int cabeza;
    public int cabello;
    public int colorCabello;
    public int cejas;
    public int zapatos;
    public int sombrero;
    public int accesorios;

}
public enum Genero
{
    masculino = 0,
    femenino = 1,
    neutro = 2
}