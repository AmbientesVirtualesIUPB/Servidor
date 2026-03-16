using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[System.Serializable]
public class OpcionEstanteria
{
    public GameObject objetoEscena;
    public GameObject prefabAInstanciar;
}

public class RayoEstanterias : MonoBehaviour
{
    [Header("Configuracion")]
    public string tagObjetivo = "Estanteria";
    public LayerMask capasDetectables = ~0;
    public Camera camara;
    public Transform objetoMover;
    public InputActionProperty eventoCrear;

    [Header("Opciones de construccion")]
    public List<OpcionEstanteria> opciones = new List<OpcionEstanteria>();
    public int indiceActivo = 0;

    private RaycastHit ultimoHitValido;
    private bool hayHitValido;

    private void OnEnable()
    {
        camara = Camera.main;

        if (eventoCrear.action != null)
        {
            eventoCrear.action.Enable();
            eventoCrear.action.performed += Crear;
        }

        ActualizarIndiceActivo();
    }

    private void OnDisable()
    {
        if (eventoCrear.action != null)
        {
            eventoCrear.action.performed -= Crear;
            eventoCrear.action.Disable();
        }
    }

    private void OnValidate()
    {
        if (indiceActivo < 0)
            indiceActivo = 0;

        ActualizarIndiceActivo();
    }

    void Update()
    {
        if (camara == null)
            camara = Camera.main;

        hayHitValido = false;

        if (camara == null)
            return;

        Vector2 posicionMouse = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        Ray rayo = camara.ScreenPointToRay(posicionMouse);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, Mathf.Infinity, capasDetectables))
        {
            if (hit.collider.CompareTag(tagObjetivo))
            {
                hayHitValido = true;
                ultimoHitValido = hit;

                Vector3 punto = hit.point;

                float x = Mathf.Round(punto.x);
                float z = Mathf.Round(punto.z);
                float y = punto.y;

                if (objetoMover != null)
                {
                    objetoMover.position = new Vector3(x, y, z);
                }
            }
        }
    }

    public void Crear(InputAction.CallbackContext c)
    {
        if (!hayHitValido)
            return;

        if (indiceActivo < 0 || indiceActivo >= opciones.Count)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        OpcionEstanteria opcionActual = opciones[indiceActivo];

        if (opcionActual == null || opcionActual.prefabAInstanciar == null)
            return;

        Vector3 punto = ultimoHitValido.point;

        float x = Mathf.Round(punto.x);
        float z = Mathf.Round(punto.z);
        float y = punto.y;

        Vector3 posicionInstancia = opciones[indiceActivo].objetoEscena.transform.position;
        Quaternion rotInstancia = opciones[indiceActivo].objetoEscena.transform.rotation;

        Instantiate(opcionActual.prefabAInstanciar, posicionInstancia, rotInstancia);
    }

    [ContextMenu("Actualizar indice activo")]
    public void ActualizarIndiceActivo()
    {
        if (opciones == null || opciones.Count == 0)
            return;

        if (indiceActivo < 0)
            indiceActivo = 0;

        if (indiceActivo >= opciones.Count)
            indiceActivo = opciones.Count - 1;

        for (int i = 0; i < opciones.Count; i++)
        {
            if (opciones[i] != null && opciones[i].objetoEscena != null)
            {
                opciones[i].objetoEscena.SetActive(i == indiceActivo);
            }
        }
    }

    public void CambiarIndiceActivo(int nuevoIndice)
    {
        indiceActivo = nuevoIndice;
        ActualizarIndiceActivo();
    }
}