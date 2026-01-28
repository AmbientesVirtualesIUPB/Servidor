using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlGeneralFriccion : MonoBehaviour
{
    public Bloque bloque;
    public AnimationCurve curvaBien;
    public AnimationCurve curvaMal;

    public float puntoInflexion;
    public Text txtFriccion;

    public string retroalimentacion;

    [Header("Cuerdas / Renderers")]
    [Tooltip("Renderers de todas las cuerdas a las que se les modificará el offset.")]
    public Renderer[] renderersCuerdas;

    [Tooltip("Nombre de la propiedad de textura a modificar.")]
    public string nombrePropiedadTextura = "_MainTex";

    [Tooltip("Índice del material dentro del Renderer (0 si solo tiene uno).")]
    public int indiceMaterial = 0;

    [Header("Offset / Velocidad")]
    public float velOffsetCuerdasMat = 3f;

    public MaterialFriccion[] matFric;
    public Renderer cuboRenderer;

    private void Start()
    {
        CambiarMaterial(0);
    }


    public void CalcularResultado()
    {
        CreadorPerros.singleton.Generar();
        StartCoroutine(CalculandoResultado());
    }

    public void CambiarMaterial(int cual)
    {
        if(cual >=0 && cual < matFric.Length)
        {
            puntoInflexion = matFric[cual].puntoInflexion;
            cuboRenderer.material.color = matFric[cual].color;
            indiceMaterial = cual;
            txtFriccion.text = matFric[cual].friccion.ToString() ;
        }
    }

    IEnumerator CalculandoResultado()
    {
        // Obligatoria para Visual Scripting
        yield return new WaitForNextFrameUnit();

        yield return new WaitForSeconds(1);
        float fuerzaPerros = ControlPerros.singleton.CalcularFuerza();
        print(fuerzaPerros);

        float resultado = fuerzaPerros - puntoInflexion;
        string titulo = "¡Felicitaciones!";
        print(resultado);

        if (resultado >= -0.11)
        {
            if(resultado < 0.09) retroalimentacion = "Son los perros exactos, Muy bien hecho.";
            if(resultado >= 0.09 && resultado <= 1) retroalimentacion = "Son casi exactos los perros, muy cerca!";
            if(resultado >  1) retroalimentacion = "Lo lograste pero gastando más perros de los necesarios.";
            Victoria();
        }
        else
        {
            retroalimentacion = "No hay suficientes perros";
            Fallo();
            titulo = "¡Rayos!";
        }
        yield return new WaitForSeconds(8);
        MSGBox.singleton.Mensaje(titulo, retroalimentacion);
    }

    void Victoria()
    {
        StartCoroutine(Victoriando());
    }

    void Fallo()
    {
        StartCoroutine(Fallando());
    }

    public void Reiniciar()
    {
        float t = 0;
        DeslizadorControl.singleton.t = curvaMal.Evaluate(t);

        // Opcional: resetear offset de las cuerdas
        ControlPerros.singleton.CambiarAnimacion(EstadoPerro.idle);
        SetOffsetEnCuerdas(0f);
    }

    IEnumerator Fallando()
    {
        float t = 0;
        float pausas = 0.04f;

        ControlPerros.singleton.CambiarAnimacion(EstadoPerro.caminar);
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(pausas);
            t += 0.01f;

            float valorCurva = curvaMal.Evaluate(t);
            DeslizadorControl.singleton.t = valorCurva;

            // Aplicar offset a todas las cuerdas
            SetOffsetEnCuerdas(valorCurva);
        }
        ControlPerros.singleton.CambiarAnimacion(EstadoPerro.reversa);
        for (int i = 50; i < 100; i++)
        {
            yield return new WaitForSeconds(pausas);
            t += 0.01f;

            float valorCurva = curvaMal.Evaluate(t);
            DeslizadorControl.singleton.t = valorCurva;

            // Aplicar offset a todas las cuerdas
            SetOffsetEnCuerdas(valorCurva);
        }
        ControlPerros.singleton.CambiarAnimacion(EstadoPerro.cansado);
    }

    IEnumerator Victoriando()
    {
        float t = 0;
        float pausas = 0.05f;
        ControlPerros.singleton.CambiarAnimacion(EstadoPerro.caminar);
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(pausas);
            t += 0.01f;

            float valorCurva = curvaBien.Evaluate(t);
            DeslizadorControl.singleton.t = valorCurva;

            // Aplicar offset a todas las cuerdas
            SetOffsetEnCuerdas(valorCurva);
        }
        ControlPerros.singleton.CambiarAnimacion(EstadoPerro.cansado);
    }

    /// <summary>
    /// Aplica el offset en Y a todos los renderers de cuerdas.
    /// </summary>
    /// <param name="valorCurva">Valor de la curva (0–1 normalmente).</param>
    private void SetOffsetEnCuerdas(float valorCurva)
    {
        if (renderersCuerdas == null || renderersCuerdas.Length == 0)
            return;

        // Offset hacia arriba, escalado por la velocidad
        Vector2 offset = Vector2.up * valorCurva * velOffsetCuerdasMat;

        foreach (var rend in renderersCuerdas)
        {
            if (rend == null)
                continue;

            // Tomamos el arreglo de materiales para evitar problemas de índices
            Material[] mats = rend.materials;
            if (mats == null || mats.Length == 0)
                continue;

            int idx = Mathf.Clamp(indiceMaterial, 0, mats.Length - 1);
            Material mat = mats[idx];

            if (mat != null && mat.HasProperty(nombrePropiedadTextura))
            {
                mat.SetTextureOffset(nombrePropiedadTextura, offset);
            }
        }
    }

}

[System.Serializable]
public class MaterialFriccion
{
    public string nombre;
    public float puntoInflexion;
    public Color color;
    public float friccion;
}