using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedDoorSystem : MonoBehaviour
{
    [System.Serializable]
    public class DoorPart
    {
        public Transform objectToMove;
        public Transform closedPoint;
        public Transform openPoint;

        [HideInInspector] public Vector3 initialPosition;
        [HideInInspector] public Quaternion initialRotation;
        [HideInInspector] public Vector3 initialScale;
    }

    [Header("Door Parts")]
    [SerializeField] private DoorPart[] doorParts;

    [Header("Animation Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private float delayBetweenParts = 0.08f;
    [SerializeField] private bool animateSequentially = true; // Si es true, los objetos se mueven uno tras otro

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Transform Options")]
    [SerializeField] private bool animatePosition = true;
    [SerializeField] private bool animateRotation = true;
    [SerializeField] private bool animateScale = false;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private bool isAnimating = false;
    private Coroutine animationCoroutine;

    private void Start()
    {
        // Guardar las posiciones iniciales de cada parte
        foreach (DoorPart part in doorParts)
        {
            if (part.objectToMove != null)
            {
                part.initialPosition = part.objectToMove.position;
                part.initialRotation = part.objectToMove.rotation;
                part.initialScale = part.objectToMove.localScale;
            }
        }

        // Posicionar las partes según el estado inicial
        if (isOpen)
            SetDoorStateImmediate(true);
        else
            SetDoorStateImmediate(false);
    }

    [ContextMenu("Toggle Door")]
    public void ToggleDoor()
    {
        if (isAnimating) return;

        isOpen = !isOpen;
        AnimateDoor(isOpen);
    }

    [ContextMenu("Open Door")]
    public void OpenDoor()
    {
        if (isOpen || isAnimating) return;

        isOpen = true;
        AnimateDoor(true);
    }

    [ContextMenu("Close Door")]
    public void CloseDoor()
    {
        if (!isOpen || isAnimating) return;

        isOpen = false;
        AnimateDoor(false);
    }

    public void SetDoorState(bool open)
    {
        if (isAnimating) return;

        isOpen = open;
        AnimateDoor(open);
    }

    private void AnimateDoor(bool opening)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateDoorCoroutine(opening));
    }

    private IEnumerator AnimateDoorCoroutine(bool opening)
    {
        isAnimating = true;

        // Reproducir sonido
        PlaySound(opening);

        if (animateSequentially)
        {
            // Animar cada parte secuencialmente
            if (opening)
            {
                // Al abrir, animar en orden normal
                for (int i = 0; i < doorParts.Length; i++)
                {
                    if (doorParts[i].objectToMove != null)
                    {
                        StartCoroutine(AnimatePart(doorParts[i], opening));
                        yield return new WaitForSeconds(delayBetweenParts);
                    }
                }
            }
            else
            {
                // Al cerrar, animar en orden inverso
                for (int i = doorParts.Length - 1; i >= 0; i--)
                {
                    if (doorParts[i].objectToMove != null)
                    {
                        StartCoroutine(AnimatePart(doorParts[i], opening));
                        yield return new WaitForSeconds(delayBetweenParts);
                    }
                }
            }

            // Esperar a que termine la última animación
            yield return new WaitForSeconds(animationDuration);
        }
        else
        {
            // Animar todas las partes simultáneamente
            List<Coroutine> animations = new List<Coroutine>();
            foreach (DoorPart part in doorParts)
            {
                if (part.objectToMove != null)
                {
                    animations.Add(StartCoroutine(AnimatePart(part, opening)));
                }
            }

            // Esperar a que terminen todas las animaciones
            yield return new WaitForSeconds(animationDuration);
        }

        isAnimating = false;
    }

    private IEnumerator AnimatePart(DoorPart part, bool opening)
    {
        Transform obj = part.objectToMove;
        Transform startPoint = opening ? part.closedPoint : part.openPoint;
        Transform endPoint = opening ? part.openPoint : part.closedPoint;

        // Si los puntos no están asignados, usar las posiciones iniciales
        Vector3 startPos = startPoint != null ? startPoint.position : obj.position;
        Vector3 endPos = endPoint != null ? endPoint.position : obj.position;

        Quaternion startRot = startPoint != null ? startPoint.rotation : obj.rotation;
        Quaternion endRot = endPoint != null ? endPoint.rotation : obj.rotation;

        Vector3 startScale = startPoint != null ? startPoint.localScale : obj.localScale;
        Vector3 endScale = endPoint != null ? endPoint.localScale : obj.localScale;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            // Aplicar curvas de animación
            if (animatePosition)
            {
                float posT = movementCurve.Evaluate(t);
                obj.position = Vector3.Lerp(startPos, endPos, posT);
            }

            if (animateRotation)
            {
                float rotT = rotationCurve.Evaluate(t);
                obj.rotation = Quaternion.Slerp(startRot, endRot, rotT);
            }

            if (animateScale)
            {
                float scaleT = scaleCurve.Evaluate(t);
                obj.localScale = Vector3.Lerp(startScale, endScale, scaleT);
            }

            yield return null;
        }

        // Asegurar que llegue exactamente al destino
        if (animatePosition)
            obj.position = endPos;
        if (animateRotation)
            obj.rotation = endRot;
        if (animateScale)
            obj.localScale = endScale;
    }

    private void SetDoorStateImmediate(bool opening)
    {
        foreach (DoorPart part in doorParts)
        {
            if (part.objectToMove != null)
            {
                Transform targetPoint = opening ? part.openPoint : part.closedPoint;

                if (targetPoint != null)
                {
                    if (animatePosition)
                        part.objectToMove.position = targetPoint.position;
                    if (animateRotation)
                        part.objectToMove.rotation = targetPoint.rotation;
                    if (animateScale)
                        part.objectToMove.localScale = targetPoint.localScale;
                }
            }
        }
    }

    private void PlaySound(bool opening)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = opening ? openSound : closeSound;
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    // Método público para verificar si está abierta
    public bool IsOpen()
    {
        return isOpen;
    }

    // Método público para verificar si está animando
    public bool IsAnimating()
    {
        return isAnimating;
    }

    // Método para establecer el estado sin animación (público)
    public void SetDoorStateImmediatePublic(bool open)
    {
        isOpen = open;
        SetDoorStateImmediate(open);
    }

    // Método para cambiar la duración de la animación en runtime
    public void SetAnimationDuration(float duration)
    {
        animationDuration = Mathf.Max(0.1f, duration);
    }

    // Gizmos para visualizar los puntos en el editor
    private void OnDrawGizmos()
    {
        if (doorParts == null) return;

        foreach (DoorPart part in doorParts)
        {
            if (part.closedPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(part.closedPoint.position, 0.1f);
                Gizmos.DrawLine(part.closedPoint.position, part.closedPoint.position + part.closedPoint.forward * 0.3f);
            }

            if (part.openPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(part.openPoint.position, 0.1f);
                Gizmos.DrawLine(part.openPoint.position, part.openPoint.position + part.openPoint.forward * 0.3f);
            }

            // Línea entre puntos
            if (part.closedPoint != null && part.openPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(part.closedPoint.position, part.openPoint.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (doorParts == null) return;

        // Mostrar etiquetas en el editor
        foreach (DoorPart part in doorParts)
        {
            if (part.objectToMove != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(part.objectToMove.position, Vector3.one * 0.2f);
            }
        }
    }
}