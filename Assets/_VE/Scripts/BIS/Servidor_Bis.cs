using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Servidor_Bis : MonoBehaviour
{
    [Header("Datos Microscopio")]
    private float t0Servidor;
    private float t1Servidor;
    public Miccroscopio miccroscopio;

    public static Servidor_Bis singleton;
    // Start is called before the first frame update

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else Destroy(this);
    }
    void Start()
    {
        GestionMensajesServidor.singeton.RegistrarAccion("BS01",EnviarDatosMicro);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnviarDatosMicro(string msg)
    {
        string[] valoresSeparados = msg.Split('*');
        t0Servidor = float.Parse(valoresSeparados[0]);
        t1Servidor = float.Parse(valoresSeparados[1]);

        miccroscopio.slider0.value = t0Servidor;
        miccroscopio.slider1.value = t1Servidor;
    }    
}
