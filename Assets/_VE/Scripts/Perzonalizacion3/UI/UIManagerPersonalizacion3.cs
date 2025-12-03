using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerPersonalizacion3 : MonoBehaviour
{
    [InfoMessage("Este es un campo OBLIGATORIO para que funcione, asegúrate de configurarlo correctamente.", MessageTypeCustom.Error)]
    public Personalizacion3 basePersonalizacion;
    public Transform    padreColoresTPR1;
    public Transform    padreColoresTPR2;
    public Transform    padreColoresSKN;

    [InfoMessage("Este es un campo importante, asegúrate de configurarlo correctamente.", MessageTypeCustom.Warning)]
    public GameObject   prBoton;

    public Transform padreCuerposHombre;
    public Transform padreCuerposMujer;

    public Transform padreCabezasHombre;
    public Transform padreCabezasMujer;

    public Transform padreColoresFRR;
    public Transform padreCabellosHombre;
    public Transform padreCabellosMujer;

    public Transform padreCejasHombre;
    public Transform padreCejasMujer;

    public Transform padreZapatos;
    public Transform padreSombreros;
    public Transform padreAccesorios;

    public GameObject[] panelesMasculinos;
    public GameObject[] panelesFemeninos;
    public Button[] botonesMasculino;
    public Button[] botonesFemenino;
    public Color colorActivo;
    public Color colorInactivo;

    public ListadoDesactivable[] listasDesactivables;

    IEnumerator Start()
    {
        Inicializar();
        yield return null;
        Cargar();
        ActivarEspecifico(0);
    }


    void Inicializar()
	{
		for (int i = 0; i < basePersonalizacion.colores1.Length; i++)
		{
            Button b = Instantiate(prBoton, padreColoresTPR1).GetComponent<Button>();
            b.image.color = basePersonalizacion.colores1[i];
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarColor1(nI));
        }

        for (int i = 0; i < basePersonalizacion.colores2.Length; i++)
        {
            Button b = Instantiate(prBoton, padreColoresTPR2).GetComponent<Button>();
            b.image.color = basePersonalizacion.colores2[i];
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarColor2(nI));
        }

        for (int i = 0; i < basePersonalizacion.coloresPiel.Length; i++)
        {
            Button b = Instantiate(prBoton, padreColoresSKN).GetComponent<Button>();
            b.image.color = basePersonalizacion.coloresPiel[i];
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarColorPiel(nI));
        }

        for (int i = 0; i < basePersonalizacion.coloresCabello.Length; i++)
        {
            Button b = Instantiate(prBoton, padreColoresFRR).GetComponent<Button>();
            b.image.color = basePersonalizacion.coloresCabello[i];
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarColorCabello(nI));
        }

        for (int i = 0; i < basePersonalizacion.cuerposHombre.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCuerposHombre).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cuerposHombre[i].sprite;
            int nI = i;
            Genero g = Genero.masculino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCuerpo(nI));
        }
        for (int i = 0; i < basePersonalizacion.cuerposMujer.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCuerposMujer).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cuerposMujer[i].sprite;
            int nI = i;
            Genero g = Genero.femenino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCuerpo(nI));
        }

        for (int i = 0; i < basePersonalizacion.cuerposHombre.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCuerposHombre).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cuerposHombre[i].sprite;
            int nI = i;
            Genero g = Genero.masculino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCuerpo(nI));
        }

        for (int i = 0; i < basePersonalizacion.cabezasHombre.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCabezasHombre).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cabezasHombre[i].sprite;
            int nI = i;
            Genero g = Genero.masculino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCabeza(nI));
        }

        for (int i = 0; i < basePersonalizacion.cabezasMujer.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCabezasMujer).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cabezasMujer[i].sprite;
            int nI = i;
            Genero g = Genero.femenino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCabeza(nI));
        }

        for (int i = 0; i < basePersonalizacion.cabellosHombre.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCabellosHombre).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cabellosHombre[i].sprite;
            int nI = i;
            Genero g = Genero.masculino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCabello(nI));
        }

        for (int i = 0; i < basePersonalizacion.cabellosMujer.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCabellosMujer).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cabellosMujer[i].sprite;
            int nI = i;
            Genero g = Genero.femenino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCabello(nI));
        }

        for (int i = 0; i < basePersonalizacion.cejasHombre.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCejasHombre).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cejasHombre[i].sprite;
            int nI = i;
            Genero g = Genero.masculino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCejas(nI));
        }

        for (int i = 0; i < basePersonalizacion.cejasMujer.Length; i++)
        {
            Button b = Instantiate(prBoton, padreCejasMujer).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.cejasMujer[i].sprite;
            int nI = i;
            Genero g = Genero.femenino;
            b.onClick.AddListener(() => basePersonalizacion.CambiarGenero(g));
            b.onClick.AddListener(() => basePersonalizacion.AsignarCejas(nI));
        }

        for (int i = 0; i < basePersonalizacion.zapatos.Length; i++)
        {
            Button b = Instantiate(prBoton, padreZapatos).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.zapatos[i].sprite;
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarZapatos(nI));
        }

        for (int i = 0; i < basePersonalizacion.sombreros.Length; i++)
        {
            Button b = Instantiate(prBoton, padreSombreros).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.sombreros[i].sprite;
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarSombrero(nI));
        }

        for (int i = 0; i < basePersonalizacion.accesorios.Length; i++)
        {
            Button b = Instantiate(prBoton, padreAccesorios).GetComponent<Button>();
            b.image.sprite = basePersonalizacion.accesorios[i].sprite;
            int nI = i;
            b.onClick.AddListener(() => basePersonalizacion.AsignarAccesorio(nI));
        }

    }


    [ContextMenu("Cargar")]
    public void Cargar()
    {
        if (EnvioDatosBD.singleton != null)
        {
            basePersonalizacion.CargarDesdeTexto(EnvioDatosBD.singleton.usuario.personalizacion);
		}
		else
        {
            basePersonalizacion.CuerpoAleatorio();
        }
    }

    public void PonerMasculino()
    {
        for (int i = 0; i < panelesMasculinos.Length; i++)
        {
            panelesMasculinos[i].SetActive(true);
        }
        for (int i = 0; i < panelesFemeninos.Length; i++)
        {
            panelesFemeninos[i].SetActive(false);
        }
        basePersonalizacion.CambiarGenero(Genero.masculino);
        basePersonalizacion.Pintar();
        for (int i = 0; i < botonesMasculino.Length; i++)
        {
            botonesMasculino[i].image.color = colorActivo;
        }
        for (int i = 0; i < botonesFemenino.Length; i++)
        {
            botonesFemenino[i].image.color = colorInactivo;
        }
    }
    public void PonerFemenino()
    {
        for (int i = 0; i < panelesMasculinos.Length; i++)
        {
            panelesMasculinos[i].SetActive(false);
        }
        for (int i = 0; i < panelesFemeninos.Length; i++)
        {
            panelesFemeninos[i].SetActive(true);
        }
        basePersonalizacion.CambiarGenero(Genero.femenino);
        basePersonalizacion.Pintar();
        for (int i = 0; i < botonesMasculino.Length; i++)
        {
            botonesMasculino[i].image.color = colorInactivo;
        }
        for (int i = 0; i < botonesFemenino.Length; i++)
        {
            botonesFemenino[i].image.color = colorActivo;
        }
    }

    public void ActivarEspecifico(int cual)
    {
        for (int i = 0; i < listasDesactivables.Length; i++)
        {
            listasDesactivables[i].Desactivar();
        }
        listasDesactivables[cual].Activar();
    }
}

[System.Serializable]
public class ListadoDesactivable
{
    public GameObject[] objetos;

    public void Activar()
    {
        ActivarDesactivar(true);
    }
    public void Desactivar()
    {
        ActivarDesactivar(false);
    }

    public void ActivarDesactivar(bool b)
	{
        for (int i = 0; i < objetos.Length; i++)
        {
            objetos[i].SetActive(b);
        }
    }
}