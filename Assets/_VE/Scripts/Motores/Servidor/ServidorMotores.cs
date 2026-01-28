using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    public Collider sueloInteractivoCollider;
    public bool plataformaIniciada;

    [Header("VALIDACION MINIJUEGO")]
    public int torque;
    public int aceite;
    public string motorActivo;

    [Header("VALIDACION MECANICO")]
    public bool esMecanico;
    public GameObject btnElegirMotor;

    private Coroutine coroutine;
    //public bool esArmador = false;
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
    IEnumerator Start()
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
        GestionMensajesServidor.singeton.RegistrarAccion("MS10", AbrirCompuertaOnline);
        GestionMensajesServidor.singeton.RegistrarAccion("PR01", IndicacionCompuerta);

        yield return new WaitUntil(() => Servidor.singleton.conectado);
        GestionMensajesServidor.singeton.EnviarMensaje("PR01"," ");
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

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(HabilitarMesaArmado(10f));
    }

    public void AbrirCompuertaOnline(string abrir)
    {
        Debug.Log("compuerta abrieta");
        if (!plataformaIniciada && abrir == "abierto")
        {
            SubirMesaArmado("");
        }
        else if (plataformaIniciada && abrir == "cerrado")
        {
            BajarMesaArmado("");
        }
    }

    public void IndicacionCompuerta(string dato)
    {
        Debug.Log("indicador antes");
        if (esMecanico)
        {
            Debug.Log("ya indico");
            GestionMensajesServidor.singeton.EnviarMensaje("MS10", plataformaIniciada?"abierto":"cerrado");
        }
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

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(HabilitarMesaArmado(8.5f));
    }

    private IEnumerator HabilitarMesaArmado(float espera)
    {
        yield return new WaitForSeconds(espera);
        EntornoMecanica.singleton.noAbroYo = false;

        if (esMecanico && !plataformaIniciada)
        {
            sueloMesaArmado.enabled = true;
            sueloInteractivoCollider.enabled = true;
            oprimirTecla.enabled = true;
        }

        if (!esMecanico && !plataformaIniciada)
        {
            sueloMesaArmado.enabled = false;
            sueloInteractivoCollider.enabled = false;
            oprimirTecla.enabled = false;
            ListaUsuariosMotores.singleton.btnMostrarLista.SetActive(EnvioDatosBD.singleton.usuario.tipo_usuario == "1");
        }

        if (plataformaIniciada)
        {
            sueloMesaArmado.enabled = true;
            sueloInteractivoCollider.enabled = true;
            oprimirTecla.enabled = true;
        }
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

        torque = int.Parse(partes[0]);
        aceite = int.Parse(partes[1]);

        Debug.Log(torque);
        Debug.Log(aceite);
        Debug.Log(motorActivo);
        ManagerMinijuego.singleton.ValidarResultado(torque, aceite, motorActivo);
    }

    public void AsignarMotorActivo(string msj)
    {
        motorActivo = msj;

        if (motorActivo == "Diesel")
        {
            ManagerMinijuego.singleton.motorAnimadoActivo = ManagerMinijuego.singleton.motoresAnimados[0];   
        }
        else if (motorActivo == "Nissan")
        {
            ManagerMinijuego.singleton.motorAnimadoActivo = ManagerMinijuego.singleton.motoresAnimados[1];
        }
        partesInstanciadas.Clear();
        ManagerMinijuego.singleton.motorAnimadoActivo.SetActive(false);
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

        partesInstanciadas.Clear();
        ManagerMinijuego.singleton.AsignarMotorActivo(msj);

        if (!MesaMotor.singleton.estoyArmando && !MesaMotor.singleton.estoyEnMesa)
        {
            btnElegirMotor.SetActive(esMecanico);
        }
        
        if (esMecanico)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("PiezaColocada2", 0.5f); // Ejecutamos el efecto nombrado
            sueloMesaArmado.enabled = true;
            oprimirTecla.enabled = true;

            if (MesaMotor.singleton.estoyEnMesa)
            {
                oprimirTecla.gameObject.SetActive(true);
            }
            else
            {
                oprimirTecla.gameObject.SetActive(false);
            }

            if (MesaMotor.singleton.estoyArmando)
            {
                oprimirTecla.gameObject.SetActive(false);
            }

            sueloInteractivoCollider.enabled = true;
            ManagerCanvas.singleton.HabilitarBtnBajarPlataforma();
            ManagerCanvas.singleton.HabilitarBtnExpandir();
        }
        else
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnOmitir", 0.5f); // Ejecutamos el efecto nombrado

            if (!plataformaIniciada && !esMecanico)
            {
                sueloMesaArmado.enabled = false;
                sueloInteractivoCollider.enabled = false;
                oprimirTecla.enabled = false;
            }

            if (plataformaIniciada && !esMecanico)
            {
                sueloMesaArmado.enabled = true;
                sueloInteractivoCollider.enabled = true;
                oprimirTecla.enabled = true;
                oprimirTecla.gameObject.SetActive(false);
            }

            if (!esMecanico)
            {
                ManagerCanvas.singleton.DeshabilitarBtnBajarPlataforma();
                ManagerCanvas.singleton.DeshabilitarBtnExpandir();
                ManagerCanvas.singleton.HabilitarBtnRotar();
                ManagerCanvas.singleton.DesactivarBtnAyudaPista();
                ManagerCanvas.singleton.DesactivarBtnAyudaAutomatica();
            }
        }
    }
}

[System.Serializable]
public class PartesMotores
{
    public int id;
    public Vector3 pos;
    public string idServidor;
}
