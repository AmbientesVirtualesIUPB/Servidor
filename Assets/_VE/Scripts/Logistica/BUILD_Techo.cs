using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUILD_Techo : MonoBehaviour
{
    public Vector3 tam;
    public Transform escalable;
    public Transform replicable;
    public float escalaZ;
    public float escalaY = 5;
    public float offsetY = 6.7f;

    public void Inicializar(float x, float y, float z)
    {
        tam = new Vector3(x, x * escalaZ, z);
        escalable.localScale = tam;
        transform.position = Vector3.up * (y * escalaY+offsetY);
        print("se movió " + y);
    }

}
