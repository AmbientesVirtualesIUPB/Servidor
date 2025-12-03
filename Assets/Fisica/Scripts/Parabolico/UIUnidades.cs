using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnidades : MonoBehaviour
{
    public TrajectoryCalculator trajeCalculator;
    public Text txtGrados;
    public Text txtVelInicial;

    public GameObject lineaVertical;
    public GameObject lineaHorizontal;
    public GameObject lineaTrayectoria;

    public void VerLineas(bool b)
    {
        lineaHorizontal.SetActive(b);
        lineaVertical.SetActive(b);
    }
    public void VerTrayectoria(bool b)
    {
        lineaTrayectoria.SetActive(b);
    }

    // Update is called once per frame
    void Update()
    {
        txtGrados.text = trajeCalculator.AnguloEnGrados.ToString("0.0") + "º";
        txtVelInicial.text = trajeCalculator.velocidadInicial().ToString("0.0") + " m/s";
    }
}
