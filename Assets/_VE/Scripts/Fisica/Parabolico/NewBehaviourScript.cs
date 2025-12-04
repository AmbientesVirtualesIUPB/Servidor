using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform objetivo;
    void Update()
    {
        transform.position = objetivo.position;
        transform.rotation = objetivo.rotation;
    }
}
