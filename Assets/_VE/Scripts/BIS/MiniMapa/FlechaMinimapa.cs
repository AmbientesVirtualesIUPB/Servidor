using UnityEngine;

public class FlechaMinimapa : MonoBehaviour
{
    public Transform cameraTransform;
    public float spriteOffset = 0f; // ajustable si la flecha apunta hacia arriba

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!cameraTransform) return;

        float yaw = cameraTransform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 0, -yaw + spriteOffset);
    }
}
