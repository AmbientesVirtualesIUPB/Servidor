using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisicaCamara : MonoBehaviour
{
    public Transform target;
    public float velocidad;
    public float velRotacion;
    public Transform pivote;
    public Vector2 rotHorizontalLimites = new Vector2(-20,20);
    float rotHorizontal;
    public Camera camara;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, velocidad * Time.deltaTime);
        if (Input.GetMouseButton(2))
        {
            transform.Rotate(Vector3.up * velRotacion * Time.deltaTime * Input.GetAxis("Mouse X"));
            rotHorizontal = Mathf.Clamp(
                rotHorizontal + velRotacion * Time.deltaTime * -Input.GetAxis("Mouse Y"),
                rotHorizontalLimites.x,
                rotHorizontalLimites.y
                );
            pivote.localEulerAngles = Vector3.forward * rotHorizontal;
        }
        camara.fieldOfView = Mathf.Clamp(camara.fieldOfView - Input.mouseScrollDelta.y, 17, 55);
    }
}
