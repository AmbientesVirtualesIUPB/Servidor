using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ValidaPiezasFueraMapa : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaMaximaY = 2f;
    public ValidadorPiezasMesa[] piezasMeson;

    [Header("Opciones")]
    public float intervaloValidacion = 0.5f;

    float timer;

    void Update()
    {
        if (transform.childCount == 0) return;

        timer += Time.deltaTime;

        if (timer >= intervaloValidacion)
        {
            timer = 0;
            Validar();
        }
    }

    [ContextMenu("Validar ahora")]
    public void Validar()
    {
        float yPadre = transform.position.y;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform hijo = transform.GetChild(i);

            float diferencia = Mathf.Abs(hijo.position.y - yPadre);

            if (diferencia > distanciaMaximaY)
            {
                for (int j = 0; j < piezasMeson.Length; j++)
                {
                    piezasMeson[j].ValidarYRestaurarHijoEspecifico(hijo.gameObject);
                }
                
                MoverPieza piezaActual = hijo.GetComponent<MoverPieza>();
                ManagerMinijuego.singleton.RemoverHijoInstanciado(piezaActual);
                StartCoroutine(EsperarParaDestruir(hijo.gameObject));            
            }
        }
    }

    private IEnumerator EsperarParaDestruir(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
