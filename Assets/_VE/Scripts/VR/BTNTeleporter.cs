using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNTeleporter : MonoBehaviour
{
    public Transform target;
    public void Teletransportar()
    {
        ForzarTeleportacion.singleton.Teletransportar(target.position);
    }
}
