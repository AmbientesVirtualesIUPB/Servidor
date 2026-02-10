using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseDeTextosPorIdioma", menuName = "Configuracion/Base De Textos Por Idioma")]
public class BaseDeTextosPorIdioma : ScriptableObject
{
    [Header("Fuente CSV")]
    public TextAsset csv;

    [Header("Separador CSV")]
    public char separador = ',';

    [Header("Datos generados (solo lectura)")]
    [SerializeField] private List<string> idiomas = new List<string>();

    string idiomaDefault = "ESP";

    [Serializable]
    public class ArregloIdioma
    {
        public string idioma;
        [TextArea] public string[] textos;
    }

    [SerializeField] private List<ArregloIdioma> datosPorIdioma = new List<ArregloIdioma>();

    // Cache para busquedas rapidas
    private Dictionary<string, string[]> mapaIdiomaATextos;

    /// <summary>
    /// Parsea el CSV y genera un arreglo de textos por cada idioma.
    /// Llamalo cuando cambie el CSV o al iniciar tu juego (una vez).
    /// </summary>
    public void ConvertirCsvAArreglos()
    {
        idiomas.Clear();
        datosPorIdioma.Clear();
        mapaIdiomaATextos = null;

        if (csv == null || string.IsNullOrEmpty(csv.text))
        {
            Debug.LogWarning("BaseDeTextosPorIdiomaSO: CSV vacio o no asignado.");
            return;
        }

        List<List<string>> filas = ParsearCsv(csv.text, separador);

        if (filas.Count == 0)
        {
            Debug.LogWarning("BaseDeTextosPorIdiomaSO: No se encontraron filas en el CSV.");
            return;
        }

        List<string> cabecera = filas[0];
        if (cabecera.Count == 0)
        {
            Debug.LogWarning("BaseDeTextosPorIdiomaSO: Cabecera sin columnas.");
            return;
        }

        // Idiomas desde la cabecera
        for (int c = 0; c < cabecera.Count; c++)
        {
            string nombreIdioma = (cabecera[c] ?? "").Trim();
            if (string.IsNullOrEmpty(nombreIdioma))
                nombreIdioma = "idioma_" + c;

            idiomas.Add(nombreIdioma);
        }

        int cantidadFilasTexto = Mathf.Max(0, filas.Count - 1);

        // Preparar buffers por idioma
        var buffers = new List<List<string>>(idiomas.Count);
        for (int i = 0; i < idiomas.Count; i++)
            buffers.Add(new List<string>(cantidadFilasTexto));

        // Llenar buffers fila por fila (desde la segunda fila)
        for (int f = 1; f < filas.Count; f++)
        {
            List<string> fila = filas[f];

            for (int c = 0; c < idiomas.Count; c++)
            {
                string valor = (c < fila.Count ? fila[c] : "");
                buffers[c].Add(valor ?? "");
            }
        }

        // Convertir a arrays y guardar
        for (int i = 0; i < idiomas.Count; i++)
        {
            var item = new ArregloIdioma
            {
                idioma = idiomas[i],
                textos = buffers[i].ToArray()
            };
            datosPorIdioma.Add(item);
        }

        ConstruirCache();
    }

    /// <summary>
    /// Devuelve el texto por id (fila) y por idioma (nombre de cabecera).
    /// Si no existe, retorna string.Empty.
    /// </summary>
    public string ObtenerTexto(int id)
    {
        return (ObtenerTexto((id), ControlGeneralIdioma.singleton == null?idiomaDefault:ControlGeneralIdioma.singleton.idioma)); ///////////// Hacer configurable!
    }

    
    public string ObtenerTexto(int id, string idioma)
    {
        if (id < 0) return string.Empty;
        if (string.IsNullOrEmpty(idioma)) return string.Empty;

        if (mapaIdiomaATextos == null || mapaIdiomaATextos.Count == 0)
            ConstruirCache();

        if (mapaIdiomaATextos == null) return string.Empty;

        if (!mapaIdiomaATextos.TryGetValue(idioma.Trim(), out var arr) || arr == null)
            return string.Empty;

        if (id >= arr.Length)
            return string.Empty;

        return arr[id] ?? string.Empty;
    }

    /// <summary>
    /// Version comoda usando indice de idioma (columna).
    /// </summary>
    public string ObtenerTexto(int id, int indiceIdioma)
    {
        if (id < 0) return string.Empty;
        if (indiceIdioma < 0 || indiceIdioma >= datosPorIdioma.Count) return string.Empty;

        var arr = datosPorIdioma[indiceIdioma].textos;
        if (arr == null || id >= arr.Length) return string.Empty;

        return arr[id] ?? string.Empty;
    }

    /// <summary>
    /// Lista de idiomas detectados (segun cabecera del CSV).
    /// </summary>
    public IReadOnlyList<string> ObtenerIdiomas()
    {
        return idiomas;
    }

    // ----------------- Interno -----------------

    public static BaseDeTextosPorIdioma configuracionDefault
    {
        get
        {
            return Resources.Load<BaseDeTextosPorIdioma>("Idiomas");
        }

        set
        {
            configuracionDefault = value;
        }
    }

    void ConstruirCache()
    {
        mapaIdiomaATextos = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < datosPorIdioma.Count; i++)
        {
            var item = datosPorIdioma[i];
            if (item == null) continue;

            string key = (item.idioma ?? "").Trim();
            if (string.IsNullOrEmpty(key)) continue;

            mapaIdiomaATextos[key] = item.textos ?? Array.Empty<string>();
        }
    }

    // Parse CSV simple (maneja comillas y separador dentro de comillas)
    static List<List<string>> ParsearCsv(string contenido, char separador)
    {
        var filas = new List<List<string>>();
        var fila = new List<string>();
        var actual = new System.Text.StringBuilder();

        bool enComillas = false;

        for (int i = 0; i < contenido.Length; i++)
        {
            char ch = contenido[i];

            if (ch == '"')
            {
                // Comillas dobles dentro de campo: ""
                if (enComillas && i + 1 < contenido.Length && contenido[i + 1] == '"')
                {
                    actual.Append('"');
                    i++;
                }
                else
                {
                    enComillas = !enComillas;
                }
                continue;
            }

            if (!enComillas)
            {
                if (ch == separador)
                {
                    fila.Add(actual.ToString());
                    actual.Length = 0;
                    continue;
                }

                // Fin de linea (soporta \n y \r\n)
                if (ch == '\n')
                {
                    fila.Add(actual.ToString());
                    actual.Length = 0;

                    filas.Add(fila);
                    fila = new List<string>();
                    continue;
                }

                if (ch == '\r')
                {
                    // ignorar \r, el \n se encarga
                    continue;
                }
            }

            actual.Append(ch);
        }

        // Ultimo campo
        fila.Add(actual.ToString());
        filas.Add(fila);

        // Limpiar filas vacias al final (opcional)
        for (int j = filas.Count - 1; j >= 0; j--)
        {
            bool vacia = true;
            for (int k = 0; k < filas[j].Count; k++)
            {
                if (!string.IsNullOrEmpty(filas[j][k]))
                {
                    vacia = false;
                    break;
                }
            }
            if (vacia) filas.RemoveAt(j);
            else break;
        }

        return filas;
    }
}
