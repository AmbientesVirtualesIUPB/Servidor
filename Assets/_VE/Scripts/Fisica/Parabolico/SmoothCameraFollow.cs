using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Y Position Limit")]
    [SerializeField] private bool useYLimit = true;
    [SerializeField] private float minYPosition = 0f;

    [Header("Look At Settings")]
    [SerializeField] private bool alwaysLookAtTarget = true;
    [SerializeField] private Vector3 lookAtOffset = Vector3.zero; // Offset para el punto de mira

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private Vector3 velocity = Vector3.zero;
    private bool isLocked = false; // Para saber si la cámara está bloqueada
    private Vector3 lockedPosition; // Posición donde se bloqueó

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No hay target asignado a la cámara!");
            return;
        }

        // Calcular la posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Verificar si el target está por debajo del límite Y
        bool targetBelowLimit = useYLimit && (desiredPosition.y < minYPosition);

        if (targetBelowLimit)
        {
            // Si acaba de pasar el límite, guardar la posición actual
            if (!isLocked)
            {
                lockedPosition = transform.position;
                isLocked = true;
            }

            // Mantener la cámara completamente quieta en la posición bloqueada
            transform.position = lockedPosition;
        }
        else
        {
            // El target está dentro del rango permitido, seguir normalmente
            isLocked = false;

            // Suavizar el movimiento usando SmoothDamp
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
            transform.position = smoothedPosition;
        }

        // Siempre mirar al target
        if (alwaysLookAtTarget)
        {
            Vector3 lookAtPoint = target.position + lookAtOffset;
            transform.LookAt(lookAtPoint);
        }
    }

    // Método público para cambiar el target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Método público para cambiar el offset
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    // Método público para cambiar la suavidad
    public void SetSmoothSpeed(float speed)
    {
        smoothSpeed = Mathf.Max(0.1f, speed);
    }

    // Método público para establecer el límite Y
    public void SetYLimit(float yLimit)
    {
        minYPosition = yLimit;
    }

    // Método público para habilitar/deshabilitar el límite Y
    public void SetYLimitEnabled(bool enabled)
    {
        useYLimit = enabled;
    }

    // Método para posicionar la cámara instantáneamente (sin suavizado)
    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Verificar si está por debajo del límite
        if (useYLimit && desiredPosition.y < minYPosition)
        {
            // No hacer snap si está por debajo del límite
            isLocked = true;
            lockedPosition = transform.position;
            return;
        }

        isLocked = false;
        transform.position = desiredPosition;
        velocity = Vector3.zero;

        if (alwaysLookAtTarget)
        {
            Vector3 lookAtPoint = target.position + lookAtOffset;
            transform.LookAt(lookAtPoint);
        }
    }

    // Método público para desbloquear la cámara manualmente
    public void UnlockCamera()
    {
        isLocked = false;
        velocity = Vector3.zero;
    }

    // Método para verificar si la cámara está bloqueada
    public bool IsLocked()
    {
        return isLocked;
    }

    // Método para obtener la posición del target
    public Transform GetTarget()
    {
        return target;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || target == null) return;

        // Dibujar línea entre la cámara y el target
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);

        // Dibujar el offset
        Gizmos.color = Color.cyan;
        Vector3 targetWithOffset = target.position + offset;
        Gizmos.DrawWireSphere(targetWithOffset, 0.5f);
        Gizmos.DrawLine(target.position, targetWithOffset);

        // Dibujar el límite Y si está activo
        if (useYLimit)
        {
            Gizmos.color = Color.red;
            Vector3 limitPosition = target.position + offset;
            limitPosition.y = minYPosition;

            // Dibujar un plano representando el límite
            float planeSize = 10f;
            Vector3 p1 = new Vector3(limitPosition.x - planeSize, minYPosition, limitPosition.z - planeSize);
            Vector3 p2 = new Vector3(limitPosition.x + planeSize, minYPosition, limitPosition.z - planeSize);
            Vector3 p3 = new Vector3(limitPosition.x + planeSize, minYPosition, limitPosition.z + planeSize);
            Vector3 p4 = new Vector3(limitPosition.x - planeSize, minYPosition, limitPosition.z + planeSize);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }

        // Dibujar punto de mira
        if (alwaysLookAtTarget)
        {
            Gizmos.color = Color.green;
            Vector3 lookAtPoint = target.position + lookAtOffset;
            Gizmos.DrawWireSphere(lookAtPoint, 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Dibujar información adicional cuando está seleccionada
        Gizmos.color = Color.white;

        Vector3 desiredPosition = target.position + offset;
        if (useYLimit)
        {
            desiredPosition.y = Mathf.Max(desiredPosition.y, minYPosition);
        }

        Gizmos.DrawWireSphere(desiredPosition, 0.3f);

        // Mostrar la distancia al target
        UnityEditor.Handles.Label(transform.position + Vector3.up,
            $"Distancia: {Vector3.Distance(transform.position, target.position):F2}m");

        if (useYLimit && target.position.y + offset.y < minYPosition)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f,
                "🔒 CÁMARA BLOQUEADA",
                new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red }, fontSize = 12, fontStyle = FontStyle.Bold });
        }
    }

    // Context Menu para testing rápido
    [ContextMenu("Snap To Target Now")]
    private void SnapToTargetContextMenu()
    {
        SnapToTarget();
    }

    [ContextMenu("Reset Velocity")]
    private void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
}