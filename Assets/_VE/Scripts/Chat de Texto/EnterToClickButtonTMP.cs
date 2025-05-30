using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class EnterToClickButtonTMP : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button targetButton;

    void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void OnSubmit(string text)
    {
        targetButton.onClick.Invoke();
        // Reasignar foco después de un pequeño retraso para que no se cancele por el submit
        StartCoroutine(ReFocusInput());
    }

    IEnumerator ReFocusInput()
    {
        yield return new WaitForEndOfFrame(); // Espera un frame para que el "unfocus" termine
        inputField.ActivateInputField();      // Vuelve a enfocar y permite seguir escribiendo
    }
}
