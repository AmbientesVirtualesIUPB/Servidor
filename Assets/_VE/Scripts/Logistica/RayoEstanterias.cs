using UnityEngine;

public class RayoEstanterias : MonoBehaviour
{
    [Header("Configuración")]
    public string tagObjetivo = "Estanteria";
    public LayerMask capasDetectables = ~0;
    public Camera camara;
    public Transform objetoMover;

    private void Reset()
    {
        camara = Camera.main;
    }

    void Update()
    {
        if (camara == null)
            camara = Camera.main;

        Ray rayo = camara.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, Mathf.Infinity, capasDetectables))
        {
            if (hit.collider.CompareTag(tagObjetivo))
            {
                Vector3 punto = hit.point;

                float x = Mathf.Round(punto.x);
                float z = Mathf.Round(punto.z);
                float y = punto.y;

                objetoMover.position = new Vector3(x, y, z);
            }
        }
    }
}