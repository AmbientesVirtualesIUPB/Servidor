using UnityEngine;

public class CanvasOrbital : MonoBehaviour
{
    [Header("Referencias")]
    public Transform centro;   // Centro del círculo
    public Transform player;   // Player a seguir

    [Header("Configuración")]
    public float radio = 2f;   // Radio ajustable desde el inspector
    public bool soloHorizontal = true;

    [Header("Altura")]
    public float alturaY = 0f;

    [Header("Rotación fija")]
    public float rotacionX = 0f; // Rotación X fija (Inspector)

    private void Start()
    {
        player = ControlUsuarios.singleton.usuarioLocal.transform;
    }

    void LateUpdate()
    {
        if (!centro || !player) return;

        // Dirección desde el centro al jugador
        Vector3 direccion = player.position - centro.position;

        if (soloHorizontal)
            direccion.y = 0f;

        direccion.Normalize();

        // Posición sobre la circunferencia
        Vector3 posicionObjetivo = centro.position + direccion * radio;

        // Aplicar altura configurable
        posicionObjetivo.y += alturaY;

        transform.position = posicionObjetivo;

        // Mirar al jugador
        transform.LookAt(player);

        // Mantener rotación X fija
        Vector3 rot = transform.eulerAngles;
        rot.x = rotacionX;
        transform.eulerAngles = rot;

        // Corrección típica en Canvas World Space
        transform.Rotate(0, 180f, 0);
    }
}
