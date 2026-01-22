using System.Collections.Generic;
using UnityEngine;

public class AudioControlTotal : MonoBehaviour
{
    // Clase para organizar los AudioSources en el Inspector
    [System.Serializable]
    public class AudioEntry
    {
        public string nombre;
        public AudioSource source;
    }

    // Instancia Singleton
    public static AudioControlTotal instance;

    [Header("Configuración de Audio Sources")]
    [SerializeField] private List<AudioEntry> listaAudios = new List<AudioEntry>();

    private void Awake()
    {
        // Lógica de Singleton
        if (instance == null)
        {
            instance = this;
            // Opcional: Descomenta la siguiente línea si quieres que persista entre escenas
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Busca un AudioSource por nombre y lo reproduce.
    /// </summary>
    public void ReproducirAudio(string nombre)
    {
        AudioEntry entry = BuscarEntrada(nombre);

        if (entry != null && entry.source != null)
        {
            entry.source.Play();
        }
    }

    /// <summary>
    /// Sobrecarga para decidir si el AudioSource debe entrar en loop o no.
    /// </summary>
    public void ReproducirAudio(string nombre, bool enLoop)
    {
        AudioEntry entry = BuscarEntrada(nombre);

        if (entry != null && entry.source != null)
        {
            entry.source.loop = enLoop;
            entry.source.Play();
        }
    }

    /// <summary>
    /// Método para detener un audio específico por su nombre.
    /// </summary>
    public void DetenerAudio(string nombre)
    {
        AudioEntry entry = BuscarEntrada(nombre);
        if (entry != null && entry.source != null)
        {
            entry.source.Stop();
        }
    }

    // Método auxiliar para evitar repetir código de búsqueda
    private AudioEntry BuscarEntrada(string nombre)
    {
        AudioEntry entry = listaAudios.Find(a => a.nombre == nombre);

        if (entry == null)
        {
            Debug.LogWarning($"AudioControlTotal: No se encontró la entrada con el nombre '{nombre}'.");
        }

        return entry;
    }
}