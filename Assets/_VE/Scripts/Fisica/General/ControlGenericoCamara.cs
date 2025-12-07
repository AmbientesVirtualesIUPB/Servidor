using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGenericoCamara : MonoBehaviour
{
    public static ControlGenericoCamara singleton;
    public MonoBehaviour camara;

    private void Awake()
    {
        singleton = this;
    }

    public void ActivarDesactivar(bool que)
    {
        camara.enabled = que;
    }
}
