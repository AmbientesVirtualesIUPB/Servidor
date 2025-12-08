using UnityEngine;
using TMPro;

public class UI_AlertaInfo : MonoBehaviour
{
    public static UI_AlertaInfo instancia;

    public GameObject panel;
    public TMP_Text texto;

    private void Awake()
    {
        instancia = this;
        panel.SetActive(false);
    }

    public void Mostrar(string mensaje)
    {
        StopAllCoroutines();
        texto.text = mensaje;
        panel.SetActive(true);
        StartCoroutine(Desvanecer());
    }

    private System.Collections.IEnumerator Desvanecer()
    {
        yield return new WaitForSeconds(1.5f);
        panel.SetActive(false);
    }
}
