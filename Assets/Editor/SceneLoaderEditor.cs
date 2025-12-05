using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoaderEditor : MonoBehaviour
{
    [MenuItem("Tools/Escena Login")]
    private static void LoadLoginScene()
    {
        // Ruta de la escena
        string scenePath = "Assets/_VE/Scenes/Constructores/C_Login.unity";

        // Verificar si hay cambios sin guardar
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // Cargar la escena
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    // Método alternativo sin preguntar por guardar cambios
    [MenuItem("Tools/Escena Login (Forzar)")]
    private static void LoadLoginSceneForced()
    {
        string scenePath = "Assets/_VE/Scenes/Constructores/C_Login.unity";
        EditorSceneManager.OpenScene(scenePath);
    }
}