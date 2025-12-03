using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ConstructorEscenas))]
public class ConstructorEscenasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Activar Todas"))
        {
            ConstructorEscenas ce = (ConstructorEscenas)target;
            EscenaGraficos[] graficos = ce.graficos;
            EscenaPlataformas[] plataformas = ce.plataformas;

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
                    }
                }
                CargarEscenaEnEditor(escena);
            }
        }
    }

    private void CargarEscenaEnEditor(string escenaNombre)
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
