using UnityEngine;

public class MaterialScrollURP : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Scroll Settings")] 
    [SerializeField] private bool isAccelerating = false;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Material Settings")]
    [SerializeField] private string texturePropertyName = "_BaseMap"; // Para URP
    [SerializeField] private int materialIndex = 0; // Por si tiene múltiples materiales

    private Material materialInstance;
    private float currentOffset = 0f;
    private float initialSpeed = 0f;

    private void Awake()
    {
        // Validar renderer
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                Debug.LogError("No se encontró un Renderer asignado o en el GameObject!");
                enabled = false;
                return;
            }
        }

        // Crear instancia del material para no afectar otros objetos
        CreateMaterialInstance();

        // Guardar la velocidad inicial
        initialSpeed = currentSpeed;
    }

    private void CreateMaterialInstance()
    {
        // Verificar que el renderer tenga materiales
        if (targetRenderer.sharedMaterials.Length == 0)
        {
            Debug.LogError("El Renderer no tiene materiales asignados!");
            enabled = false;
            return;
        }

        // Validar el índice del material
        if (materialIndex >= targetRenderer.sharedMaterials.Length)
        {
            Debug.LogWarning($"Material index {materialIndex} fuera de rango. Usando material 0.");
            materialIndex = 0;
        }

        // Crear instancia del material
        Material[] materials = targetRenderer.materials; // Esto crea instancias automáticamente
        materialInstance = materials[materialIndex];
    }

    private void Update()
    {
        // Si está acelerando, aumentar la velocidad
        if (isAccelerating)
        {
            currentSpeed += acceleration * Time.deltaTime;

            // Limitar la velocidad máxima si está configurada
            if (maxSpeed > 0)
            {
                currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
            }
        }

        // Actualizar el offset basado en la velocidad actual
        currentOffset += currentSpeed * Time.deltaTime;

        // Aplicar el offset al material
        ApplyOffset();
    }

    private void ApplyOffset()
    {
        if (materialInstance != null)
        {
            // Obtener el offset actual
            Vector2 offset = materialInstance.GetTextureOffset(texturePropertyName);

            // Actualizar solo el eje X
            offset.x = currentOffset;

            // Aplicar el nuevo offset
            materialInstance.SetTextureOffset(texturePropertyName, offset);
        }
    }

    [ContextMenu("Reset Speed")]
    public void ResetSpeed()
    {
        currentSpeed = initialSpeed;
        isAccelerating = false;
    }

    [ContextMenu("Start Accelerating")]
    public void StartAccelerating()
    {
        isAccelerating = true;
    }

    [ContextMenu("Stop Accelerating")]
    public void StopAccelerating()
    {
        isAccelerating = false;
    }

    // Método público para resetear completamente (velocidad y offset)
    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        currentSpeed = initialSpeed;
        currentOffset = 0f;
        isAccelerating = false;
        ApplyOffset();
    }

    // Método público para establecer la velocidad
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    // Método público para establecer la aceleración
    public void SetAcceleration(float accel)
    {
        acceleration = accel;
    }

    // Método público para obtener la velocidad actual
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    // Método público para establecer si está acelerando
    public void SetAccelerating(bool accelerating)
    {
        isAccelerating = accelerating;
    }

    // Método público para establecer el offset directamente
    public void SetOffset(float offset)
    {
        currentOffset = offset;
        ApplyOffset();
    }

    // Método público para obtener el offset actual
    public float GetCurrentOffset()
    {
        return currentOffset;
    }

    // Método público para cambiar el nombre de la propiedad de textura
    public void SetTexturePropertyName(string propertyName)
    {
        texturePropertyName = propertyName;
    }

    private void OnDestroy()
    {
        // Limpiar la instancia del material cuando se destruya el objeto
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }

    private void OnDisable()
    {
        // Opcional: resetear al deshabilitar
        // ResetAll();
    }

    // Validación en el inspector
    private void OnValidate()
    {
        // Asegurar que la aceleración no sea negativa
        if (acceleration < 0)
            acceleration = 0;

        // Asegurar que el índice del material sea válido
        if (materialIndex < 0)
            materialIndex = 0;
    }
}