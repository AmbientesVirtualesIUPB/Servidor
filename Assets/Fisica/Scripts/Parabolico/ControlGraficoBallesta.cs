using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGraficoBallesta : MonoBehaviour
{
    public Animator animaciones;
    public Transform carreteAtras;
    public float anguloCarrete = 800;
    public Transform ejeVertical;
    public Transform ejeVertical2;
    public float anguloDireccione = 90;
    public Transform carreteBase;

    float anguloDisparo;
    float direccionDisparo;
    public Transform maztil;
    public TrajectoryCalculator traCalculator;
    public void CambiarVelocidadInicial(float v)
    {
        animaciones.SetFloat("Tens", v);
        carreteAtras.localEulerAngles = Vector3.right * v * anguloCarrete;
        traCalculator.velInicialT = v;
    }

    public void CambiarDireccion(float v)
    {
        direccionDisparo = v;
        ActualizarDirecciones();
    }

    public float GetDireccion()
    {
        return direccionDisparo;
    }
    public void CambiarAnguloInicial(float v)
    {
        anguloDisparo = v;
        ActualizarDirecciones();
    }

    public void ActualizarDirecciones()
    {
        ejeVertical.localEulerAngles = Vector3.up * direccionDisparo * anguloDireccione;
        ejeVertical2.localEulerAngles = Vector3.up * direccionDisparo * anguloDireccione + Vector3.left * (90);
        maztil.localEulerAngles = (-90 - anguloDisparo) * Vector3.right + Vector3.up * direccionDisparo * anguloDireccione;
        carreteBase.localEulerAngles = Vector3.right * anguloDisparo * 50;
    }

}
