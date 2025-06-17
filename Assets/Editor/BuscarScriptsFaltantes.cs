using UnityEngine;
using UnityEditor;

public class BuscarScriptsFaltantes
{
    [MenuItem("Herramientas/Encontrar scripts faltantes en escena")]
    public static void Buscar()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int total = 0;

        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Objeto con script faltante: {go.name}", go);
                    total++;
                }
            }
        }

        Debug.Log($"Total de componentes faltantes encontrados: {total}");
    }
}
