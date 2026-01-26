using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServidorMotores : MonoBehaviour
{
    [Header("MANIPULACION DE LAS PIEZAS")]
    public List<MoverPieza> partes;
    public List<GameObject> partesInstanciadas;
    public Transform[] padresInstancia;

    [Header("MANIPULACION BRAZOS")]
    public AgregarDisolver esferaDisolver;
    public Transform targetBrazo;

    [Header("MANIPULACION MESA ARMADO")]
    public SueloInteractivo sueloMesaArmado;
    public Canvas oprimirTecla;
    public bool plataformaIniciada;

    [Header("VALIDACION MINIJUEGO")]
    public int Torque;
    public int Aceite;

    [Header("VALIDACION MECANICO")]
    public bool esMecanico;
    public GameObject btnElegirMotor;

    public bool esArmador = false;
    public static ServidorMotores singleton;

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
    // Start is called before the first frame update
    void Start()
    {
        GestionMensajesServidor.singeton.RegistrarAccion("MS00", InstanciaPiezaServidor);
        GestionMensajesServidor.singeton.RegistrarAccion("MS01", SubirMesaArmado);
        GestionMensajesServidor.singeton.RegistrarAccion("MS02", BajarMesaArmado);
        GestionMensajesServidor.singeton.RegistrarAccion("MS03", ManipularBrazoDerecho);
        GestionMensajesServidor.singeton.RegistrarAccion("MS04", ManipularBrazoIzquierdo);
        GestionMensajesServidor.singeton.RegistrarAccion("MS05", RegresarBrazosMecanicos);
        GestionMensajesServidor.singeton.RegistrarAccion("MS06", ValidarMinijuego);
        GestionMensajesServidor.singeton.RegistrarAccion("MS07", AsignarMotorActivo);
        GestionMensajesServidor.singeton.RegistrarAccion("MS08", ParteColocada);
        GestionMensajesServidor.singeton.RegistrarAccion("MS09", ActivarMecanico);
    }

    public void InstanciaPiezaServidor(string msj)
    {
        PartesMotores parteMotor = JsonUtility.FromJson<PartesMotores>(msj);
        InstanciarPieza(parteMotor);
    }


    public void InstanciarPieza(PartesMotores parte)
    {
        for (int i = 0; i < partes.Count; i++)
        {
            if (parte.id == partes[i].id)
            {
                GameObject pieza = Instantiate(partes[i].gameObject,parte.pos,Quaternion.identity);
                pieza.transform.parent = padresInstancia[pieza.GetComponent<MoverPieza>().piezaExterna ? 0 : 1];
                pieza.GetComponent<MorionID>().SetID(parte.idServidor);
                pieza.GetComponent<Collider>().enabled = false; // Desactivamos los colliders de las piezas

                partesInstanciadas.Add(pieza);

                //Buscamos los hijos Snap
                foreach (Transform hijo in pieza.GetComponentsInChildren<Transform>(true))
                {
                    // Evita evaluarse a sí mismo
                    if (hijo == pieza.transform)
                        continue;

                    if (hijo.name.StartsWith("SP"))
                    {
                        hijo.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void SubirMesaArmado(string msj)
    {
        plataformaIniciada = true;
        EntornoMecanica.singleton.noAbroYo = true;
        EntornoMecanica.singleton.AbrirCompuerta(sueloMesaArmado.posicionObjetivoCamara);
        sueloMesaArmado.plataformaAbajo = false;
        sueloMesaArmado.enabled = false;
        oprimirTecla.enabled = false;
        StartCoroutine(HabilitarMesaArmado(10f));
    }

    public void BajarMesaArmado(string msj)
    {
        plataformaIniciada = false;
        if (MesaMotor.singleton.estoyArmando) sueloMesaArmado.SalirInteraccion();
        EntornoMecanica.singleton.noAbroYo = true;
        EntornoMecanica.singleton.CerrarCompuerta();
        sueloMesaArmado.plataformaAbajo = true;
        sueloMesaArmado.enabled = false;
        oprimirTecla.enabled = false;
        StartCoroutine(HabilitarMesaArmado(8.5f));
    }

    private IEnumerator HabilitarMesaArmado(float espera)
    {
        yield return new WaitForSeconds(espera);
        EntornoMecanica.singleton.noAbroYo = false;
        sueloMesaArmado.enabled = true;
        oprimirTecla.enabled = true;
    }

    public void ManipularBrazoDerecho(string msj)
    {
        for (int i = 0; i < partesInstanciadas.Count; i++)
        {
            if (partesInstanciadas[i].GetComponent<MoverPieza>().id == int.Parse(msj))
            {
                esferaDisolver = partesInstanciadas[i].GetComponent<MoverPieza>().esferaDisolver;
                targetBrazo = partesInstanciadas[i].transform;
            }
        }

        ManagerBrazos.singleton.AsignarTargetDerecho(targetBrazo, esferaDisolver);
    }

    public void ManipularBrazoIzquierdo(string msj)
    {
        for (int i = 0; i < partesInstanciadas.Count; i++)
        {
            if (partesInstanciadas[i].GetComponent<MoverPieza>().id == int.Parse(msj))
            {
                esferaDisolver = partesInstanciadas[i].GetComponent<MoverPieza>().esferaDisolver;
                targetBrazo = partesInstanciadas[i].transform;
            }
        }

        ManagerBrazos.singleton.AsignarTargetIzquierdo(targetBrazo, esferaDisolver);
    }

    public void RegresarBrazosMecanicos(string msj)
    {
        ManagerBrazos.singleton.RetornarBrazos(); // Le asignamos este transform como target a los brazis
        ManagerBrazos.singleton.EfectoDisolverInversa(); // Le retiramos el efecto de disolver
    }

    public void ValidarMinijuego(string msj)
    {
        string[] partes = msj.Split('*');

        Torque = int.Parse(partes[0]);
        Aceite = int.Parse(partes[1]);

        ManagerMinijuego.singleton.ValidarResultado(Torque, Aceite);
    }

    public void AsignarMotorActivo(string msj)
    {
        partesInstanciadas.Clear();
        ManagerMinijuego.singleton.AsignarMotorActivo(msj);
    }

    public void ParteColocada(string msj)
    {
        ModificarPartesColocadas(int.Parse(msj));
    }

    public void ModificarPartesColocadas(int id)
    {
        for (int i = 0; i < partesInstanciadas.Count; i++)
        {
            if (partesInstanciadas[i].GetComponent<MoverPieza>().id == id)
            {
                partesInstanciadas[i].GetComponent<MoverPieza>().piezaColocada = true;
                partesInstanciadas.RemoveAt(i);
            }
        }
    }

    public void ActivarMecanico(string msj)
    {
        esMecanico = msj == ControlUsuarios.singleton.usuarioLocal.GetMorionID().ID;
        btnElegirMotor.SetActive(esMecanico);

        if (esMecanico) if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("PiezaColocada2", 0.5f); // Ejecutamos el efecto nombrado
    }
}

[System.Serializable]
public class PartesMotores
{
    public int id;
    public Vector3 pos;
    public string idServidor;
}
