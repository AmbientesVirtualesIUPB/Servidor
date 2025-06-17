using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionAvatar : MonoBehaviour
{
    private bool rotando = false; // Para rastrear si el clic est� sostenido
    public float velocidadRotacion = 70f; // Velocidad de rotaci�n ajustable
    private Vector3 utilmaPosMouse; // �ltima posici�n del mouse

    void Update()
    {
        // Detecta si el clic izquierdo del mouse comenz�
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Comprueba si el rayo golpea este objeto
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                rotando = true; // Comienza la rotaci�n
                utilmaPosMouse = Input.mousePosition; // Registra la posici�n inicial del mouse
            }
        }

        // Detecta si se suelta el clic izquierdo del mouse
        if (Input.GetMouseButtonUp(0))
        {
            rotando = false; // Detiene la rotaci�n
        }

        // Si est� en modo rotaci�n, calcula el desplazamiento del mouse
        if (rotando)
        {
            Vector3 actualPosicion = Input.mousePosition;
            float deltaX = actualPosicion.x - utilmaPosMouse.x; // Cambio horizontal del mouse

            // Gira el objeto en funci�n del desplazamiento
            transform.Rotate(Vector3.up, -deltaX * velocidadRotacion * Time.deltaTime);

            // Actualiza la posici�n del mouse para la pr�xima iteraci�n
            utilmaPosMouse = actualPosicion;
        }
    }

    /// <summary>
    /// Metodo invocado desde btnBack para reestablecer la posicion del avatar
    /// </summary>
    public void ReestablecerPosicion()
    {
        transform.eulerAngles = new Vector3(0f, 0f, 0f); //Establecemos las rotaciones en cero
    }
}
