using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstructorEscenas : MonoBehaviour
{
    public EscenaGraficos[] graficos;
    public EscenaPlataformas[] plataformas;
    public bool debugEnConsola;
    public void Awake()
    {
        ///////////////////// Cargar gráficos ////////////////////
        if (graficos.Length > 0)
        {
            TipoGraficos tg = ConfiguracionGeneral.configuracionDefault.tipoGraficos;
            string escena = graficos[0].escena;
            for (int i = 0; i < graficos.Length; i++)
            {
                if (tg == graficos[i].graficos)
                {
                    escena = graficos[i].escena;
                }
            }
            AgregarEscenaSiNoEstaCargada(escena);
        }

        ///////////////////// Cargar Interactivos ////////////////////
        if (plataformas.Length > 0)
        {
            Plataforma p = ConfiguracionGeneral.configuracionDefault.plataformaObjetivo;
            string escena = plataformas[0].escena;
            for (int i = 0; i < plataformas.Length; i++)
            {
                if (p == plataformas[i].plataforma)
                {
                    escena = plataformas[i].escena;
                }
            }
            AgregarEscenaSiNoEstaCargada(escena);
        }
    }

    public void AgregarEscenaSiNoEstaCargada(string escena)
    {
        // Verificar si la escena ya está cargada
        if (IsSceneLoaded(escena))
        {
            if(debugEnConsola) Debug.Log($"La escena '{escena}' ya está cargada, no se volverá a cargar.");
            return;
        }

        // Cargar la escena de manera aditiva si no está cargada
        SceneManager.LoadScene(escena, LoadSceneMode.Additive);
    }

    private bool IsSceneLoaded(string sceneName)
    {
        // Iterar sobre las escenas actualmente cargadas
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class EscenaGraficos
{
    public TipoGraficos graficos;
    public string escena;
}

[System.Serializable]
public class EscenaPlataformas
{
    public Plataforma plataforma;
    public string escena;
}
