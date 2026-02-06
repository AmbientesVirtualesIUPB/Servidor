using UnityEngine;

public class SeguimientoMinimapa : MonoBehaviour
{

    public Transform player;
    public float height = 50f;

    // Start is called before the first frame update
    void Start()
    {
        player = ControlUsuarios.singleton.usuarioLocal.GetComponent<Transform>();
    }

    void LateUpdate()
    {
        if (!player) return;

        transform.position = new Vector3(player.position.x, height, player.position.z);
    }
}
