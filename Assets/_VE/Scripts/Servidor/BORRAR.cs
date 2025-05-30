using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BORRAR : MonoBehaviour
{
    public float velocidad = 10;
    public MorionID mid;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!mid.GetOwner()) return;
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * velocidad * Time.deltaTime);
    }
}
