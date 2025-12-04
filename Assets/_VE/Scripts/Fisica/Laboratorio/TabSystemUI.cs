using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabSystemUI : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public string tabName;
        public GameObject panel;

        [HideInInspector] public Button button;
        [HideInInspector] public Text buttonText;
    }

    [Header("Tab Configuration")]
    [SerializeField] private Tab[] tabs;
    [SerializeField] private int defaultTabIndex = 0;

    [Header("Button Settings")]
    [SerializeField] private GameObject buttonPrefab; // Prefab del botón
    [SerializeField] private Transform buttonContainer; // Contenedor con Horizontal Layout Group

    [Header("Button Colors")]
    [SerializeField] private Color activeButtonColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private Color inactiveButtonColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color hoverButtonColor = new Color(0.3f, 0.7f, 1f, 1f);

    [Header("Text Colors")]
    [SerializeField] private Color activeTextColor = Color.white;
    [SerializeField] private Color inactiveTextColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Animation")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private int currentTabIndex = -1;
    private List<ColorBlock> originalColorBlocks = new List<ColorBlock>();

    private void Start()
    {
        InitializeTabs();
        SelectTab(defaultTabIndex);
    }

    private void InitializeTabs()
    {
        // Validar referencias
        if (buttonContainer == null)
        {
            Debug.LogError("Button Container no está asignado!");
            return;
        }

        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab no está asignado!");
            return;
        }

        // Crear botones para cada pestaña
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i; // Capturar el índice para el closure

            // Instanciar el botón
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError("El prefab no tiene un componente Button!");
                continue;
            }

            // Configurar el texto del botón
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = tabs[i].tabName;
                tabs[i].buttonText = buttonText;
            }
            else
            {
                Debug.LogWarning($"El botón de la pestaña '{tabs[i].tabName}' no tiene un componente Text hijo!");
            }

            // Guardar referencia del botón
            tabs[i].button = button;

            // Guardar los colores originales del botón
            originalColorBlocks.Add(button.colors);

            // Configurar el listener del botón
            button.onClick.AddListener(() => SelectTab(index));

            // Deshabilitar el panel inicialmente
            if (tabs[i].panel != null)
            {
                tabs[i].panel.SetActive(false);
            }
        }
    }

    [ContextMenu("Select First Tab")]
    public void SelectFirstTab()
    {
        SelectTab(0);
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabs.Length)
        {
            Debug.LogWarning($"Índice de pestaña inválido: {index}");
            return;
        }

        // Si ya está seleccionada, no hacer nada
        if (currentTabIndex == index)
            return;

        // Desactivar la pestaña anterior
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
        {
            if (tabs[currentTabIndex].panel != null)
                tabs[currentTabIndex].panel.SetActive(false);

            UpdateButtonVisuals(currentTabIndex, false);
        }

        // Activar la nueva pestaña
        currentTabIndex = index;

        if (tabs[index].panel != null)
        {
            if (useAnimation)
            {
                tabs[index].panel.SetActive(true);
                StartCoroutine(AnimatePanel(tabs[index].panel));
            }
            else
            {
                tabs[index].panel.SetActive(true);
            }
        }

        UpdateButtonVisuals(index, true);
    }

    private void UpdateButtonVisuals(int index, bool isActive)
    {
        if (index < 0 || index >= tabs.Length || tabs[index].button == null)
            return;

        Button button = tabs[index].button;
        Text buttonText = tabs[index].buttonText;

        // Actualizar colores del botón
        ColorBlock colors = button.colors;
        colors.normalColor = isActive ? activeButtonColor : inactiveButtonColor;
        colors.highlightedColor = isActive ? activeButtonColor : hoverButtonColor;
        colors.selectedColor = isActive ? activeButtonColor : inactiveButtonColor;
        button.colors = colors;

        // Actualizar color del texto
        if (buttonText != null)
        {
            buttonText.color = isActive ? activeTextColor : inactiveTextColor;
        }

        // Opcional: Escala del botón activo
        if (useAnimation)
        {
            float targetScale = isActive ? 1.05f : 1.0f;
            StartCoroutine(AnimateButtonScale(button.transform, targetScale));
        }
    }

    private System.Collections.IEnumerator AnimatePanel(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        canvasGroup.alpha = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            canvasGroup.alpha = animationCurve.Evaluate(t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator AnimateButtonScale(Transform buttonTransform, float targetScale)
    {
        Vector3 startScale = buttonTransform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            buttonTransform.localScale = Vector3.Lerp(startScale, endScale, animationCurve.Evaluate(t));
            yield return null;
        }

        buttonTransform.localScale = endScale;
    }

    // Método público para cambiar de pestaña por nombre
    public void SelectTabByName(string tabName)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabName == tabName)
            {
                SelectTab(i);
                return;
            }
        }

        Debug.LogWarning($"No se encontró ninguna pestaña con el nombre: {tabName}");
    }

    // Método público para obtener el índice actual
    public int GetCurrentTabIndex()
    {
        return currentTabIndex;
    }

    // Método público para obtener el nombre de la pestaña actual
    public string GetCurrentTabName()
    {
        if (currentTabIndex >= 0 && currentTabIndex < tabs.Length)
            return tabs[currentTabIndex].tabName;

        return "";
    }

    // Método público para habilitar/deshabilitar una pestaña
    public void SetTabEnabled(int index, bool enabled)
    {
        if (index < 0 || index >= tabs.Length || tabs[index].button == null)
            return;

        tabs[index].button.interactable = enabled;

        // Si se deshabilitó la pestaña activa, cambiar a otra
        if (!enabled && currentTabIndex == index)
        {
            // Buscar la primera pestaña habilitada
            for (int i = 0; i < tabs.Length; i++)
            {
                if (i != index && tabs[i].button.interactable)
                {
                    SelectTab(i);
                    break;
                }
            }
        }
    }

    // Método público para habilitar/deshabilitar una pestaña por nombre
    public void SetTabEnabledByName(string tabName, bool enabled)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabName == tabName)
            {
                SetTabEnabled(i, enabled);
                return;
            }
        }
    }

    // Método para actualizar el texto de una pestaña
    public void UpdateTabName(int index, string newName)
    {
        if (index < 0 || index >= tabs.Length)
            return;

        tabs[index].tabName = newName;
        if (tabs[index].buttonText != null)
        {
            tabs[index].buttonText.text = newName;
        }
    }

    // Método para agregar pestañas dinámicamente en runtime
    public void AddTab(string tabName, GameObject panel)
    {
        // Crear nuevo array con espacio adicional
        Tab[] newTabs = new Tab[tabs.Length + 1];
        for (int i = 0; i < tabs.Length; i++)
        {
            newTabs[i] = tabs[i];
        }

        // Agregar nueva pestaña
        Tab newTab = new Tab();
        newTab.tabName = tabName;
        newTab.panel = panel;
        newTabs[tabs.Length] = newTab;

        tabs = newTabs;

        // Crear el botón para la nueva pestaña
        int index = tabs.Length - 1;
        GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
        Button button = buttonObj.GetComponent<Button>();

        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = tabName;
            tabs[index].buttonText = buttonText;
        }

        tabs[index].button = button;
        originalColorBlocks.Add(button.colors);

        int capturedIndex = index;
        button.onClick.AddListener(() => SelectTab(capturedIndex));

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    // Contextos de menú para testing
    [ContextMenu("Select Next Tab")]
    private void SelectNextTab()
    {
        int nextIndex = (currentTabIndex + 1) % tabs.Length;
        SelectTab(nextIndex);
    }

    [ContextMenu("Select Previous Tab")]
    private void SelectPreviousTab()
    {
        int prevIndex = (currentTabIndex - 1 + tabs.Length) % tabs.Length;
        SelectTab(prevIndex);
    }
}