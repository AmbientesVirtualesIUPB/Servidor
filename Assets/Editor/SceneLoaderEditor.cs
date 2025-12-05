using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class QuickSceneLoader
{
    [MenuItem("Tools/Escena Login")]
    private static void LoadLoginScene()
    {
        string scenePath = "Assets/_VE/Scenes/Constructores/C_Login.unity";

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
            CargarEscenasAdicionales();
        }
    }

    [MenuItem("Tools/Escena Lobby")]
    private static void LoadLobbyScene()
    {
        string scenePath = "Assets/_VE/Scenes/Constructores/C_Lobby.unity";

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
            CargarEscenasAdicionales();
        }
    }

    private static void CargarEscenasAdicionales()
    {
        // Buscar el GameObject "Manwe" en la escena activa
        GameObject manweObject = GameObject.Find("Manwe");

        if (manweObject == null)
        {
            Debug.LogWarning("No se encontró el GameObject 'Manwe' en la escena.");
            return;
        }

        // Obtener el componente ConstructorEscenas
        ConstructorEscenas constructorEscenas = manweObject.GetComponent<ConstructorEscenas>();

        if (constructorEscenas == null)
        {
            Debug.LogWarning("No se encontró el componente 'ConstructorEscenas' en 'Manwe'.");
            return;
        }

        EscenaGraficos[] graficos = constructorEscenas.graficos;
        EscenaPlataformas[] plataformas = constructorEscenas.plataformas;

        // Cargar gráficos
        if (graficos.Length > 0)
        {
            TipoGraficos tg = ConfiguracionGeneral.configuracionDefault.tipoGraficos;
            string escena = graficos[0].escena;
            for (int i = 0; i < graficos.Length; i++)
            {
                if (tg == graficos[i].graficos)
                {
                    escena = graficos[i].escena;
                    break;
                }
            }
            CargarEscenaEnEditor(escena);
        }

        // Cargar plataformas
        if (plataformas.Length > 0)
        {
            Plataforma p = ConfiguracionGeneral.configuracionDefault.plataformaObjetivo;
            string escena = plataformas[0].escena;
            for (int i = 0; i < plataformas.Length; i++)
            {
                if (p == plataformas[i].plataforma)
                {
                    escena = plataformas[i].escena;
                    break;
                }
            }
            CargarEscenaEnEditor(escena);
        }
    }

    private static void CargarEscenaEnEditor(string escenaNombre)
    {
        // Buscar la ruta completa de la escena
        string[] guids = AssetDatabase.FindAssets($"t:Scene {escenaNombre}");
        if (guids.Length == 0)
        {
            Debug.LogWarning($"No se encontró ninguna escena llamada '{escenaNombre}'. Verifica el nombre y la ruta.");
            return;
        }

        // Intentar cargar la primera coincidencia
        string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        if (EditorSceneManager.GetSceneByPath(scenePath).isLoaded)
        {
            Debug.Log($"La escena '{escenaNombre}' ya está cargada.");
            return;
        }

        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        Debug.Log($"Escena '{escenaNombre}' cargada desde: {scenePath}");
    }
}