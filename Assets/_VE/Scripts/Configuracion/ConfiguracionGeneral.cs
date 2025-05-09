// Importa las bibliotecas necesarias
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Si se está utilizando el Editor de Unity, importa la biblioteca de UnityEditor
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Clase que representa la configuración de gráficos como un ScriptableObject.
/// </summary>
[CreateAssetMenu(menuName = "Configuracion/Configuracion general", fileName = "ConfiguracionGeneral")]
public class ConfiguracionGeneral : ScriptableObject
{
    /// <summary>
    /// Tipo de gráficos seleccionados (bajos, medios, altos).
    /// </summary>
    public TipoGraficos tipoGraficos;

    /// <summary>
    /// Plataformas seleccionadas mediante flags (Movil, Windows, VR).
    /// </summary>
    public Plataforma plataformaObjetivo;

    /// <summary>
    /// Ip del servidor al que se va a conectar.
    /// </summary>
    public string url = "10.0.100.9";

    /// <summary>
    /// Ip del servidor al que se va a conectar.
    /// </summary>
    public string ip = "127.0.0.1";

    /// <summary>
    /// URL API Artcraft.
    /// </summary>
    public string urlAPIArtcraft = "http://localhost/artcraft/api/";
    /// <summary>
    /// Notas para tener en cuenta.
    /// </summary>
    [Multiline(3)]
    public string notas = "";

    /// <summary>
    /// Obtiene un prefijo basado en el tipo de gráficos seleccionado.
    /// </summary>
    /// <returns>El prefijo correspondiente al tipo de gráficos.</returns>
    public string GetPrefijo()
    {
        switch (tipoGraficos)
        {
            case TipoGraficos.bajos:
                return "LP";
            case TipoGraficos.medios:
                return "MP";
            case TipoGraficos.altos:
                return "HP";
        }
        return "";
    }
    /// <summary>
    /// Devuelve una lista de plataformas que hayan sido seleccionadas en el enum multiseleccionable
    /// </summary>


    public static ConfiguracionGeneral configuracionDefault
    {
        get
        {
            return Resources.Load<ConfiguracionGeneral>("ConfiguracionGeneral");
        }

        set
        {
            configuracionDefault = value;
        }
    }

    public bool VerificarPlataforma(Plataformas plataformas)
    {
        string[] p = plataformas.ToString().Replace(" ", "").Split(",");
        for (int i = 0; i < p.Length; i++)
        {
            if (p[i].ToString() == plataformaObjetivo.ToString())
            {
                return true;
            }
        }
        return false;
    }
    public bool VerificarPlataforma(Plataformas plataformas, int _plataforma)
    {
        string[] p = plataformas.ToString().Replace(" ", "").Split(",");
        Plataforma plataforma = (Plataforma) _plataforma;
        for (int i = 0; i < p.Length; i++)
        {
            if (p[i] == plataforma.ToString())
            {
                return true;
            }
        }
        return false;
    }

    public string GetIP()
    {
        return PlayerPrefs.GetString("ip", ip);
    }
}

/// <summary>
/// Enumeración que representa los niveles de gráficos (bajos, medios, altos).
/// </summary>
public enum TipoGraficos
{
    bajos = 0,
    medios = 1,
    altos = 2
}

/// <summary>
/// Enumeración de Flags que representa las plataformas disponibles (Movil, Windows, VR).
/// </summary>
[System.Flags]
public enum Plataformas
{
    Nada = 0,
    Movil = 1 << 1,
    Windows = 1 << 2,
    VR = 1 << 4
}

/// <summary>
/// Enumeración de Flags que representa las plataformas disponibles (Movil, Windows, VR).
/// </summary>
public enum Plataforma
{
    Nada = 0,
    Movil = 1,
    Windows = 2,
    VR = 4
}
/// <summary>
/// Clase sellada que define un atributo de EnumFlags para ser utilizado en la interfaz del editor.
/// </summary>
public sealed class EnumFlagsAttribute : PropertyAttribute
{
    /// <summary>
    /// Constructor por defecto de EnumFlagsAttribute.
    /// </summary>
    public EnumFlagsAttribute() { }

    /// <summary>
    /// Obtiene los índices de las opciones seleccionadas en la enumeración.
    /// </summary>
    /// <typeparam name="T">Tipo de la enumeración.</typeparam>
    /// <param name="val">Valor de la enumeración con flags.</param>
    /// <returns>Lista de índices de las opciones seleccionadas.</returns>
    public static List<int> GetSelectedIndexes<T>(T val) where T : IConvertible
    {
        List<int> selectedIndexes = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(T)).Length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedIndexes.Add(i);
            }
        }
        return selectedIndexes;
    }

    /// <summary>
    /// Obtiene las cadenas de las opciones seleccionadas en la enumeración.
    /// </summary>
    /// <typeparam name="T">Tipo de la enumeración.</typeparam>
    /// <param name="val">Valor de la enumeración con flags.</param>
    /// <returns>Lista de cadenas de las opciones seleccionadas.</returns>
    public static List<string> GetSelectedStrings<T>(T val) where T : IConvertible
    {
        List<string> selectedStrings = new List<string>();
        for (int i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedStrings.Add(Enum.GetValues(typeof(T)).GetValue(i).ToString());
            }
        }
        return selectedStrings;
    }
}

// Si se está utilizando el Editor de Unity, define un PropertyDrawer para el atributo EnumFlags
#if UNITY_EDITOR
/// <summary>
/// Clase que define el dibujante personalizado para el atributo EnumFlags.
/// </summary>
[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    /// <summary>
    /// Sobrescribe el método OnGUI para personalizar la interfaz del editor.
    /// </summary>
    /// <param name="position">Rectángulo de posición del campo en el Editor.</param>
    /// <param name="property">Propiedad serializada asociada al campo.</param>
    /// <param name="label">Etiqueta del campo.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Utiliza EditorGUI.MaskField para mostrar un campo de máscara de bits en el Editor
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
    }
}
#endif
