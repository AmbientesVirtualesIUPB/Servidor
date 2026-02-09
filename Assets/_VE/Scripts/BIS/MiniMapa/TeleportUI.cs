using UnityEngine.UI;
using UnityEngine;

public class TeleportUI : MonoBehaviour
{
    public Transform player;
    public PuntoTeleport teleportPoint;
    public RectTransform miniMapRect;
    public float mapMundoRadio = 50f;
    public float bordeMargen = 4f;

    RectTransform rect;
    Button boton;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        boton = GetComponent<Button>();
        boton.onClick.AddListener(Teleportar);
    }

    void Start()
    {
        player = ControlUsuarios.singleton.usuarioLocal.GetComponent<Transform>();
    }

    void Update()
    {
        if (!player || !teleportPoint) return;

        Vector3 offset = teleportPoint.transform.position - player.position;
        Vector2 offset2D = new Vector2(offset.x, offset.z);

        float distancia = offset2D.magnitude;
        float radioMapa = miniMapRect.rect.width * 0.5f;
        float radioIcono = rect.rect.width * 0.5f;

        Vector2 pos = (offset2D / mapMundoRadio) * radioMapa;

        if (distancia > mapMundoRadio)
        {
            pos = pos.normalized * (radioMapa - radioIcono - bordeMargen);
        }

        rect.anchoredPosition = pos;
    }

    void Teleportar()
    {
        // SOLO permitir si el mapa está expandido
        //if (!ZoomMiniMapa.singleton.estaExpandido) return;
        ZoomMiniMapa.singleton.ToggleMiniMap();
        player.position = teleportPoint.transform.position;
    }
}
