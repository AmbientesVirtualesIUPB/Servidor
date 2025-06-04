using System.Collections;
using UnityEngine;
using TMPro;

public class TextoCargando : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float tamano;
    public float velocidadAumento;

    IEnumerator AumentarPuntos()
    {
        while (true)
        {
            textMeshPro.text = $"Cargando <size={tamano}>.</size>..";

            yield return new WaitForSeconds(velocidadAumento);

            textMeshPro.text = $"Cargando .<size={tamano}>.</size>.";

            yield return new WaitForSeconds(velocidadAumento);

            textMeshPro.text = $"Cargando ..<size={tamano}>.</size>";

            yield return new WaitForSeconds(velocidadAumento);
        }
    }


    /// <summary>
    /// Al desactivar el objeto
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(AumentarPuntos());
    }

    /// <summary>
    /// Al activar el objeto
    /// </summary>
    private void OnDisable()
    {
        StopCoroutine(AumentarPuntos());
    }
}
