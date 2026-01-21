using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanoParabolicoDeltas : MonoBehaviour
{
    public Text txtDY;
    public Text txtX;
    public TrajectoryCalculator calculador;
    public Apuntador apuntador;
    public Transform[] metas;
    private void Start()
    {
        Resanar();
    }

    public void Resanar()
    {
        Transform obj = metas[apuntador.indiceObjetivo];
        txtX.text = (calculador.mira.position - obj.position).magnitude.ToString("0.0") + "m";
        txtDY.text = Mathf.Abs(calculador.mira.position.y - obj.position.y).ToString("0.0") + "m";
    }
}
