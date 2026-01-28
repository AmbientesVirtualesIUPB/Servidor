using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabraLoca : MonoBehaviour
{
    float tiempoSonar;
    // Start is called before the first frame update
    void Cambio()
    {
        tiempoSonar = Time.time + Random.Range(2, 20);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > tiempoSonar)
        {
            AudioControlTotal.instance.ReproducirAudio("Cabra");
            Cambio();
        }
    }
}
