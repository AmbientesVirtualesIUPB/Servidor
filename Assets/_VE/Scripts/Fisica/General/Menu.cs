
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class Menu : MonoBehaviour
{
    public InputActionProperty botonActivacion;
    public Animator animator;
    public UnityEvent evendoBoton;
    public bool visible;

    // Start is called before the first frame update
    void Start()
    {
        botonActivacion.action.Enable();
        botonActivacion.action.performed += BotonAccion;
    }

    void BotonAccion(InputAction.CallbackContext contexto)
    {
        visible = !visible;
        evendoBoton.Invoke();
        animator.SetBool("visible", visible);
        ControlGenericoCamara.singleton.ActivarDesactivar(!visible);
    }
}
