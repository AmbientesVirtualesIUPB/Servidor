using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SueloInteractivo : MonoBehaviour
{
    [Header("Referencias Obligatorias")]
    public GameObject canvasWorldSpace; // Hace referencia al canvas que nos indica que tecla oprimir
    public GameObject canvasPrincipal; // Hace referencia al canvas principal del escenario
    public Button btnSalir; // Referencia al btnSalir del canvas
    public CamaraOrbital camaraPrincipal; // Camara orbital / principal
    public Transform posicionObjetivoCamara; // Posicion a la que deseamos llevar la camara
    public float velocidadPosCamara = 1; // Velocidad de desplazamiento de la camara
    public Collider[] piezasMeson; // Piezas sobre la mesa

    [Header("Referencias Opcionales")]
    public GameObject canvasSecundario; // Hace referencia al canvas secundario a activar
    public bool esRestaurable;
    public EscaladorUI escaladorUI;
    public MoverObjeto moverObjeto;
    public Button btnBajarPlataforma; // Referencia al btnBajarPlataforma del canvas

    [Header("Booleanos ID")]
    public bool btnCambiarMotor; // Para validar si quiero habilitar el boton de cambio de motor
    public bool mesaArmadoMotor; // Para validar si es el suelo interactivo de la mesa de armado, deberia ir activa en el sueloInteractivoArmadoMotor
    public bool mesaHerramientas; // Para validar si es el suelo interactivo de la mesa de herramientas, deberia ir activa en el SueloInteractivo Porta Herramientas
    public bool mesaDinamometro;

    public bool puedoInteractuarInicialmente;
    private MovimientoJugador movimientoJugador; // Para guardar la referencia del movimiento del jugador
    private Camera camera; // Para guardar referencia a nuestra camara
    private int playerLayer; // Para guardar el numero de layer
    private Vector3 posicionOriginal; // para guardar la posicion original
    private Quaternion rotacionOriginal; // para guardar la rotacion original
    private bool botonesSalvavidasEjecutados;
    public bool interactuar; // Para validar si estoy interactuando
    [HideInInspector]
    public bool salirInteraccion; // Para validar si salgo de la interaccion
    [HideInInspector]
    public bool plataformaAbajo; // Para validar si salgo bajando la plataforma
    private Coroutine coroutine;
    
    private void Awake()
    {
        camaraPrincipal = CamaraOrbital.singleton;
        camera = Camera.main;// Obtenemos el componenete de la camara
    }

    private void Start()
    {
        //StartCoroutine(CargaFantasmaCanvas()); // Cargamos los componentes de canvas rapidamente al inicio
        playerLayer = LayerMask.NameToLayer("Player"); // Obtener el número de layer correspondiente al nombre "Player"
        plataformaAbajo = true; // indicamos que inicialmente la plataforma se encuentra abajo
    }

    private void Update()
    {
        if (interactuar)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("SueloInteractivo2", 1f);

                DesactivarMovimientoJugador(movimientoJugador); // Desactivamos el movimiento del jugador que interactua

                posicionOriginal = camaraPrincipal.transform.position; // Guardamos la posicion original de mi camara orbital antes de interactuar
                rotacionOriginal = camaraPrincipal.transform.rotation; // Guardamos la rotacion original de mi camara orbital antes de interactuar

                camaraPrincipal.CursorVisible(); // Habilitamos la vista del cursor
                camaraPrincipal.enabled = false; // Deshabilitamos el script de la camara orbital

                if (mesaArmadoMotor && ManagerMinijuego.singleton.minijuegoActivo)
                {
                    if (plataformaAbajo)
                    {
                        EntornoMecanica.singleton.AbrirCompuerta(ManagerMinijuego.singleton.posicionMinijuegoActual);
                        plataformaAbajo = false;
                    }
                    else
                    {
                        InicializarMovimientoCamara(ManagerMinijuego.singleton.posicionMinijuegoActual);
                    }
                    MesaMotor.singleton.estoyArmando = true;
                }
                else if (mesaArmadoMotor)
                {
                    ControlCamaraMotor.singleton.ReestablecerPosicionCamara(); // Reiniciamos el indice para que la posicion de la camara sea correcta
                    MesaMotor.singleton.estoyArmando = true;

                    if (plataformaAbajo)
                    {
                        GestionMensajesServidor.singeton.EnviarMensaje("MS01", " Subiendo plataforma");
                        ServidorMotores.singleton.plataformaIniciada = true;
                        EntornoMecanica.singleton.AbrirCompuerta(posicionObjetivoCamara);
                        plataformaAbajo = false;                       
                    }
                    else
                    {
                        InicializarMovimientoCamara(posicionObjetivoCamara);
                    }            
                }
                else
                {
                    InicializarMovimientoCamara(posicionObjetivoCamara);
                }

                if (mesaDinamometro)
                {
                    if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.ActivarSonidoDinamometro(); // Detenemos el sonido loop
                }

                ControlCamaraMotor.singleton.CambiarNearCamara(0.01f);
                camera.cullingMask &= ~(1 << playerLayer); // Desactivamos la layer "PLayer" de la camara para que no se vea nuestro personaje
                canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual       
                btnSalir.onClick.AddListener(SalirInteraccion); // Agregamos el evento actual al boton

                ManagerMinijuego.singleton.btnDesplazarMotor.onClick.AddListener(SalirInteraccion); // Agregamos el evento actual al boton

                // Si tenemos referenciado el boton lo activamos
                if (btnBajarPlataforma != null) btnBajarPlataforma.onClick.AddListener(BajarPlataforma);

                // Si tenemos referenciado el script, ejecutamos
                if (moverObjeto != null) moverObjeto.IniciarDesplazamientoObjeto();

                // Si tenemos almenos una pieza para interactuar
                if (piezasMeson.Length > 0) ActivarPiezas();

                interactuar = false; // indicamos que ya no podemos interactuar
            }
        }

        if (mesaArmadoMotor && MesaMotor.singleton.estoyEnMesa && MesaMotor.singleton.estoyArmando && MesaMotor.singleton.mesaMotorActiva && !interactuar)
        {
            ManagerCanvas.singleton.btnSalir.gameObject.SetActive(true);
            if (ServidorMotores.singleton.esMecanico)
            {
                ManagerCanvas.singleton.HabilitarBtnBajarPlataforma();
            }
            else
            {
                ManagerCanvas.singleton.DeshabilitarBtnBajarPlataforma();
            }
        }
    }

    public void InicializarMovimientoCamara(Transform posicionObjetivo)
    {
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(MoverCamara(posicionObjetivo.position, posicionObjetivo.rotation, velocidadPosCamara)); // Movemos la camara 
    }

    private void OnTriggerEnter(Collider other)
    {

        if (puedoInteractuarInicialmente)
        {
            if (other.CompareTag("Player"))
            {
                if (!ValidarOwner(other)) return;

                interactuar = true; // Indicamos que podemos interactuar
                canvasWorldSpace.SetActive(true); // Activamos canvas visual

                if (mesaArmadoMotor) MesaMotor.singleton.estoyEnMesa = true;
            }
        }

        if (btnCambiarMotor)
        {
            if (other.CompareTag("Player"))
            {
                if (!ValidarOwner(other)) return;
                ManagerCanvas.singleton.DesactivarBTNEleccionMotor();
            }
        }   
    }

    public bool ValidarOwner(Collider other)
    {
        MorionID morionID = other.GetComponent<MorionID>();

        if (morionID != null && !morionID.isOwner)
        {
            return false;
        }
        else
        {
            morionID = other.GetComponentInChildren<MorionID>();
            if (morionID != null && !morionID.isOwner) return false;
        }
        return true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!ValidarOwner(other)) return;
            movimientoJugador = other.GetComponent<MovimientoJugador>();  // Obtenemos una referencia al movimiento del jugador que interactua
        }     
    }

    private void OnTriggerExit(Collider other)
    {
        movimientoJugador = null;  // Eliminamos la referencia al movimiento del jugador que interactua

        if (puedoInteractuarInicialmente)
        {
            if (other.CompareTag("Player"))
            {
                if (!ValidarOwner(other)) return;

                interactuar = false; // Indicamos que no podemos interactuar
                canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual

                if (mesaArmadoMotor)
                {
                    MesaMotor.singleton.estoyEnMesa = false;
                    MesaMotor.singleton.estoyArmando = false;
                    botonesSalvavidasEjecutados = false;
                }                                
            }
        }

        if (btnCambiarMotor)
        {
            if (other.CompareTag("Player"))
            {
                if (!ValidarOwner(other)) return;
                ManagerCanvas.singleton.ActivarBTNEleccionMotor();

            }
        }     
    }

    public void TrigerExit()
    {
        interactuar = false; // Indicamos que no podemos interactuar
        canvasWorldSpace.SetActive(false);  // Desactivamos canvas visual
        movimientoJugador = null;  // Eliminamos la referencia al movimiento del jugador que interactua    
    }

    /// <summary>
    /// Currutina encargada del movimiento de la pieza suavizado
    /// </summary>
    /// <param name="posicionDeseada"> La posicion a la cual queremos moder la camara </param>
    /// <param name="duracion"> Tiempo del movimiento de la pieza </param>

    IEnumerator MoverCamara(Vector3 destinoPos, Quaternion destinoRot, float duracion)
    {
        Vector3 posicionInicio = camaraPrincipal.transform.position; //  Guardamos la posicion de inicio
        Quaternion rotacionInicio = camaraPrincipal.transform.rotation; //  Guardamos la rotacion de inicio

        float tiempo = 0f; // Damos un tiempo para la interpolacion

        while (tiempo < duracion)
        {
            // Asignamos la posicion y rotacion de la camara, con interpolacion lineal
            camaraPrincipal.transform.position = Vector3.Lerp(posicionInicio, destinoPos, tiempo / duracion);
            camaraPrincipal.transform.rotation = Quaternion.Lerp(rotacionInicio, destinoRot, tiempo / duracion);

            tiempo += Time.deltaTime;
            yield return null;
        }

        camaraPrincipal.transform.position = destinoPos; // Aseguramos la posición final
        camaraPrincipal.transform.rotation = destinoRot; // Aseguramos la rotacion final

        IngresandoInteraccion();
    }

    public void IngresandoInteraccion()
    {
        if (salirInteraccion) // Si salimos de interaccion
        {
            camaraPrincipal.enabled = true; // Habilitamos nuevamente la camara orbital
            camaraPrincipal.CursorInvisible(); // Habilitamos la vista del cursor
            ActivarMovimientoJugador(movimientoJugador); // Activamos el movimiento del jugador que interactua        
            if (movimientoJugador != null) interactuar = true; // Indicamos que nuevamente puede interactura aun sin salir del trigger 
            salirInteraccion = false; // Indicamos que ya no estamos interactuando
        }
        else
        {
            btnSalir.gameObject.SetActive(true); // Habilitamos el boton de salir
            if (canvasPrincipal != null) canvasPrincipal.SetActive(true);  // Activamos canvas informativo
            if (canvasSecundario != null) canvasSecundario.SetActive(true);  // Activamos canvas informativo

            if (btnBajarPlataforma != null) btnBajarPlataforma.gameObject.SetActive(true);// Si tenemos referenciado el boton lo activamos

            if (mesaArmadoMotor && !plataformaAbajo)
            {
                // Si el miijuego esta activo lo desactivamos al momento de salir de la interaccion de la mesa de armado
                if (ManagerMinijuego.singleton.minijuegoActivo) 
                {
                    ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(true);
                    ManagerMinijuego.singleton.herramientasRotatorias.SetActive(true);

                    if (InventarioUI.singleton.tamanoHerramienta == 1)
                    {
                        ManagerMinijuego.singleton.prensaValvulas.SetActive(true);
                    }
                } 

                if (EntornoMecanica.singleton != null)
                {
                    EntornoMecanica.singleton.BajarIntensidadLuzPrincipal();
                } 

                if (ControlCamaraMotor.singleton != null) ControlCamaraMotor.singleton.enabled = true;

                MesaMotor.singleton.mesaMotorActiva = true;
            }      
        }
    }

    /// <summary>
    /// Metodo utilizado al momento de salir de la interacccion
    /// </summary>
    public void SalirInteraccion()
    {
        StartCoroutine(SalirInteraccionCurrutina());
    }

    IEnumerator SalirInteraccionCurrutina()
    {
        if (mesaHerramientas && InventarioHerramientas.singleton.herramientasTomadas.Count > 0)
        {
            if (InventarioHerramientas.singleton != null) { InventarioHerramientas.singleton.ReactivarHerramientasTomadas(); }
            yield return new WaitForSeconds(0.5f);          
        }
        else
        {
            yield return new WaitForSeconds(0.01f);
        }

        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("SueloInteractivo2", 1f);
        // Indicamos que estamos saliendo de la interacion
        salirInteraccion = true;

        if (moverObjeto != null) moverObjeto.RetornarPosicionOriginal();

        // Si tenemos almenos una pieza para interactuar
        if (piezasMeson.Length > 0) DesactivarPiezas();

        if (!plataformaAbajo && mesaArmadoMotor)
        {
            if (ManagerMinijuego.singleton.minijuegoActivo)
            {
                ManagerMinijuego.singleton.miniJuegoAtornillar.SetActive(false);
                ManagerMinijuego.singleton.herramientasRotatorias.SetActive(false);

                if (InventarioUI.singleton.tamanoHerramienta == 1 || ManagerMinijuego.singleton.sizeHerramienta == 1)
                {
                    ManagerMinijuego.singleton.prensaValvulas.SetActive(false);
                }
            }

            if (ManagerBrazos.singleton != null) ManagerBrazos.singleton.RetornarBrazos();

            if (EntornoMecanica.singleton != null)
            {
                EntornoMecanica.singleton.SubirIntensidadLuzPrincipal();
            } 

            if (ControlCamaraMotor.singleton != null) ControlCamaraMotor.singleton.enabled = false;

            MesaMotor.singleton.mesaMotorActiva = false;
            MesaMotor.singleton.estoyArmando = false;

            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(MoverCamara(posicionOriginal, rotacionOriginal, velocidadPosCamara)); // Retornamos la camara principal a la posicion original 
            HabilitarInfoMesaArmado();
        }
        else if (!mesaArmadoMotor)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(MoverCamara(posicionOriginal, rotacionOriginal, velocidadPosCamara)); // Retornamos la camara principal a la posicion original 
            HabilitarInfoMesaArmado();
        }

        if (mesaDinamometro)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerSonidoDinamometro(); // Detenemos el sonido loop
        }

        camera.cullingMask |= (1 << playerLayer); // Activamos de nuevo la layer "Player" para que nuestro personaje se vea     
        ControlCamaraMotor.singleton.CambiarNearCamara(1f);
        if (canvasPrincipal != null && !esRestaurable) canvasPrincipal.SetActive(false);  // Desactivamos canvas principal   
        if (escaladorUI != null) escaladorUI.Restaurar();  // Restauramos 

        if (canvasSecundario != null)
        {
            canvasSecundario.SetActive(false);  // Activamos canvas informativo
            ManagerCanvas.singleton.ActualizarInformacionPieza("", ""); //Borramos la informacion del cavas
        }

        btnSalir.gameObject.SetActive(false); // Deshabilitamos el boton de salir   
        btnSalir.onClick.RemoveListener(SalirInteraccion); // Retiramos el evento actual del boton
        ManagerMinijuego.singleton.btnDesplazarMotor.onClick.RemoveListener(SalirInteraccion); // Retiramos el evento actual al boton

        if (btnBajarPlataforma != null)
        {
            btnBajarPlataforma.onClick.RemoveListener(BajarPlataforma); // Retiramos el evento actual del boton
            btnBajarPlataforma.gameObject.SetActive(false);
        }
    }

    public void BajarPlataforma()
    {
        if (EntornoMecanica.singleton != null)
        {
            plataformaAbajo = true;
            SalirInteraccion();
            GestionMensajesServidor.singeton.EnviarMensaje("MS02", " Bajando plataforma");
            ServidorMotores.singleton.plataformaIniciada = false;
            EntornoMecanica.singleton.CerrarCompuerta();       
            btnBajarPlataforma.gameObject.SetActive(false);       
        }  
    }

    public void HabilitarInfoMesaArmado()
    {
        canvasWorldSpace.SetActive(true); // Activamos canvas visual
    }

    /// <summary>
    /// Metodo utilizado para activar el movimiento del jugador que interactura
    /// </summary>
    /// <param name="movimiento"> script de movimiento </param>
    public void ActivarMovimientoJugador(MovimientoJugador movimiento)
    {
        if (movimiento != null)
        {
            movimiento.enabled = true;
        }
        else
        {
            ManagerCanvas.singleton.movimientoJugador.enabled = true;
            ManagerCanvas.singleton.movimientoJugador.HabilitarJugador();
            TrigerExit();
        }
    }

    /// <summary>
    /// Metodo utilizado para desactivar el movimiento del jugador que interactura
    /// </summary>
    /// <param name="movimiento"> script de movimiento </param>
    public void DesactivarMovimientoJugador(MovimientoJugador movimiento)
    {
        if (movimiento != null)
        {
            movimiento.enabled = false;
        }
        else
        {
            ManagerCanvas.singleton.movimientoJugador.enabled = false;
            ManagerCanvas.singleton.movimientoJugador.DeneterJugador();
        }    
    }

    /// <summary>
    /// Metodo para habilitar los collider de las piezas
    /// </summary>
    public void ActivarPiezas()
    {
        StartCoroutine(ActivarPiezasmeson());
    }

    IEnumerator ActivarPiezasmeson()
    {
        if (mesaHerramientas)
        {
            yield return new WaitForSeconds(2f);

            for (int i = 0; i < piezasMeson.Length; i++)
            {
                // Validamos que las piezas no sean nulas y procedemos a activar los colliders
                if (piezasMeson[i] != null)
                {
                    piezasMeson[i].enabled = true;
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(0.01f);
            for (int i = 0; i < piezasMeson.Length; i++)
            {
                // Validamos que las piezas no sean nulas y procedemos a activar los colliders
                if (piezasMeson[i] != null)
                {
                    piezasMeson[i].enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Metodo para inhabilitar los collider de las piezas
    /// </summary>
    public void DesactivarPiezas()
    {
        for (int i = 0; i < piezasMeson.Length; i++)
        {
            // Validamos que las piezas no sean nulas y procedemos a desactivar los colliders
            if (piezasMeson[i] != null)
            {
                piezasMeson[i].enabled = false;
            }          
        }
    }

    IEnumerator CargaFantasmaCanvas()
    {
        canvasWorldSpace.SetActive(true);
        canvasPrincipal.SetActive(true);
        btnSalir.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        canvasWorldSpace.SetActive(false);
        canvasPrincipal.SetActive(false);
        btnSalir.gameObject.SetActive(false);
    }
}
