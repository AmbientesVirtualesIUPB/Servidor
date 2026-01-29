using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForzarTeleportacion : MonoBehaviour
{
    public static ForzarTeleportacion singleton;

    private void Start()
    {
        singleton = this;
    }

    public void Teletransportar(Vector3 pos)
    {
        transform.position = pos;
    }
}
