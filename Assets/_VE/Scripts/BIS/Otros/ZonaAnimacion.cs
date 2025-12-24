using UnityEngine;
using System.Collections.Generic;


public class ZonaAnimacion : MonoBehaviour
{
    [Header("Animator que controla la animación")]
    public Animator animator;          // Asigna aquí el Animator en el Inspector

    [Header("Nombre del parámetro bool")]
    public string nombreBool = "Entro"; // Debe coincidir con el bool del Animator

    private void Reset()
    {
        // Si el script está en el mismo objeto que el Animator, se asigna solo
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetBool(nombreBool, true);  // Entró a la zona
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetBool(nombreBool, false); // Salió de la zona
            }
        }
    }
}
