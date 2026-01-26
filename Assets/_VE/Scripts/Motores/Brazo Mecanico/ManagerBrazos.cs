using UnityEngine;

public class ManagerBrazos : MonoBehaviour
{
    public BrazoMecanico[] brazosMecanicos;
    private AgregarDisolver esferaDisolver;

    public static ManagerBrazos singleton;
    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        RetornarBrazos();
    }

    public void AsignarTargetsAmbosBrazos(Transform nuevoTarget)
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].AsignarTarget(nuevoTarget);
        }
    }

    public void RetornarBrazos()
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].RegresarAPosicionInicial();
        }
    }

    public void AsignarTargetDerecho(Transform nuevoTarget, AgregarDisolver esfera)
    {
        esferaDisolver = esfera;
        brazosMecanicos[0].AsignarTarget(nuevoTarget);
        brazosMecanicos[1].RegresarAPosicionInicial();
    }

    public void AsignarTargetIzquierdo(Transform nuevoTarget, AgregarDisolver esfera)
    {
        esferaDisolver = esfera;
        brazosMecanicos[1].AsignarTarget(nuevoTarget);
        brazosMecanicos[0].RegresarAPosicionInicial();
    }

    public void DesactivarBrazos()
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].gameObject.SetActive(false);
        }
    }

    public void ActivarBrazos()
    {
        for (int i = 0; i < brazosMecanicos.Length; i++)
        {
            brazosMecanicos[i].gameObject.SetActive(true);
        }
    }

    public void EfectoDisolver()
    {
        if (esferaDisolver != null)
        {
            esferaDisolver.Disolver();
        }      
    }

    public void EfectoDisolverInversa()
    {
        if (esferaDisolver != null)
        {
            esferaDisolver.Restaurar();
        }     
    }
}
