using UnityEngine;
using UnityEngine.UI;
public class PuntoMisionMiniMapa : MonoBehaviour
{
    public Transform player;
    public Transform objetivoMision;
    public RectTransform miniMapRect;
    public float mapMundoRadio = 50f;
    public float bordeMargen = 4f;

    RectTransform rect;
    Image imagenPunto;

    public static PuntoMisionMiniMapa singleton;

    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }

        rect = GetComponent<RectTransform>();
        imagenPunto = GetComponent<Image>();
    }

    void Start()
    {
        player = ControlUsuarios.singleton.usuarioLocal.GetComponent<Transform>();
    }

    void Update()
    {
        if (!player || !objetivoMision || !miniMapRect)
        {
            imagenPunto.enabled = false;
            return;
        }

        imagenPunto.enabled = true;

        // Offset mundo
        Vector3 offset = objetivoMision.position - player.position;
        Vector2 offset2D = new Vector2(offset.x, offset.z);

        float distancia = offset2D.magnitude;

        // 🔹 Radio REAL del minimapa (no sizeDelta)
        float radioMapa = Mathf.Min(miniMapRect.rect.width, miniMapRect.rect.height) * 0.5f;

        // 🔹 Radio visual del icono
        float radioIcono = rect.rect.width * 0.5f;

        // Posición dentro del mapa
        Vector2 pos = (offset2D / mapMundoRadio) * radioMapa;

        // Limitar al borde
        if (distancia > mapMundoRadio)
        {
            float radioSeguro = radioMapa - radioIcono - bordeMargen;
            pos = pos.normalized * radioSeguro;
        }

        rect.anchoredPosition = pos;
    }

    // Cambiar misión
    public void AsignarNuevaMision(Transform nuevoObjetivo)
    {
        objetivoMision = nuevoObjetivo;
    }
}
