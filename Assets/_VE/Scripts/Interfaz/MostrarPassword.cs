using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class MostrarPassword : MonoBehaviour
{
    public TMP_InputField passwordInput; // Asigna aquí tu TMP_InputField de contraseña
    public Toggle toggle; // Asigna aquí un Toggle o botón

    private string passwordOculto; // Para almacenar el password temporalmente

    void Start()
    {
        // Asegurarse de que el toggle esté desactivado al inicio
        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(delegate { TogglePassword(); });
        }
    }

    public void TogglePassword()
    {
        if (toggle.isOn)
        {
            // Mostrar la contraseña real
            passwordOculto = passwordInput.text;
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            // Ocultar la contraseña con asteriscos
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        }

        // Refrescar el campo de entrada
        passwordInput.ForceLabelUpdate();
    }
}
