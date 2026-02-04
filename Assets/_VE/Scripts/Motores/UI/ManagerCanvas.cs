using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerCanvas : MonoBehaviour
{
    [Header("ESTA ES UNA CLASE SINGLETON")]
    
    public MovimientoJugador movimientoJugador; // Referencia al movimiento jugador principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public EscaladorUI menuBienvenida; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public EscaladorUI menuEleccionMotor; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject btnEleccionMotor; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject imagenBLoqueoMotor; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject btnEleccionMecanico; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject imagenBLoqueoMecanico; // Referencia al Menu de bienvenida del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public UIAutoAnimation menuPausa; // Referencia al Menu Pausa del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnSalir; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnRotar; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnAyudaPista; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnAyudaAutomatica; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button btnBajarPlataforma; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public Button[] btnExpandir; // Referencia al boton btnSalir del canvas principal
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtTituloPieza; // Referencia al texto titulo de la pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtDescripcionPieza; // Referencia al texto descripcion para la pieza
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public GameObject mensajeAlerta;
    [InfoMessage("Este es una referencia importante, arrastrala del CanvasPrincipal", MessageTypeCustom.Warning)]
    public TextMeshProUGUI txtMensaje; // Referencia al texto que nos indica si algo esta incorrecto o el inventario esta lleno
    [TextArea(3, 4)]
    public string pistaActual;

    //[HideInInspector]
    public bool btnReutilizableHabilitado;
    [HideInInspector]
    public bool juegoPausado;
    //[HideInInspector]
    public bool mensajeAlertaActivo;
    public bool activarTutorial;
    public bool puedoPausar;
    public static ManagerCanvas singleton;

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
        movimientoJugador = ControlUsuarios.singleton.usuarioLocal.GetComponent<MovimientoJugador>();

        if (activarTutorial)
        {
            if (menuBienvenida != null)
            {
                movimientoJugador.DeneterJugador();
                if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.DeneterCamara();
            }
        }      
    }
    private void Update()
    {
        if (puedoPausar)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!juegoPausado)
                {
                    if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Menu", 0.5f); // Ejecutamos el efecto nombrado
                    juegoPausado = true;
                    menuPausa.EntranceAnimation();
                    movimientoJugador.DeneterJugador();
                    if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.DeneterCamara();
                }
                else
                {
                    if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnOmitir", 0.5f); // Ejecutamos el efecto nombrado
                    juegoPausado = false;
                    menuPausa.ExitAnimation();
                    movimientoJugador.HabilitarJugador();
                    if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.HabilitarCamara();
                }
            }
        }   
    }

    /// <summary>
    /// Metodo utilizado para actualizar la informacion de la pieza tomada de la mesa
    /// </summary>
    /// <param name="titulo"> Nombre tecnico de la pieza</param>
    /// <param name="descripcion"> Descripcion de para que sirve esa pieza </param>
    public void ActualizarInformacionPieza(string titulo, string descripcion)
    {
        txtTituloPieza.text = titulo;
        txtDescripcionPieza.text = descripcion;
    }

    /// <summary>
    /// Metodo utilizado para actualizar la pista o ayuda en cuestion segun la pieza en la que vaya actualmente
    /// </summary>
    /// <param name="pistaDada"> Descripcion de la pista de para esta pieza </param>
    public void ActualizarInformacionPista(string pistaDada)
    {
        pistaActual = pistaDada;
    }

    /// <summary>
    /// Metodo invocado desde btnAyuda en el canvas para actualizar la pista o ayuda en cuestion segun la pieza en la que vaya actualmente
    /// </summary>
    public void PistaSolicitada()
    {
        txtTituloPieza.text = "Pista";
        txtDescripcionPieza.text = pistaActual;
    }

    /// <summary>
    /// Metodo utilizaod para borrar la informacion del titulo y descripcion
    /// </summary>
    public void BorrarInformacionPieza()
    {
        txtTituloPieza.text = "";
        txtDescripcionPieza.text = "";
    }

    /// <summary>
    /// Para habilitar la notificacion de inventario lleno o cualquier otro mensaje de alerta
    /// </summary>
    public void AlertarMensaje(string texto)
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Error", 0.6f); // Ejecutamos el efecto nombrado
        txtMensaje.text = texto;
        mensajeAlerta.SetActive(true);
        mensajeAlertaActivo = true;
    }

    /// <summary>
    /// Para deshabilitar la notificacion de inventario lleno o cualquier otro mensaje de alerta
    /// </summary>
    public void DesactivarAlertarMensaje()
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnResaltar2", 0.2f); // Ejecutamos el efecto nombrado
        mensajeAlerta.SetActive(false);
        mensajeAlertaActivo = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void HabilitarBtnSalir()
    {
        btnSalir.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnSalir()
    {
        btnSalir.interactable = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void HabilitarBtnRotar()
    {
        btnRotar.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnRotar()
    {
        btnRotar.interactable = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void HabilitarBtnAyudaAutomatica()
    {
        btnAyudaAutomatica.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnAyudaAutomatica()
    {
        btnAyudaAutomatica.interactable = false;
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void ActivarBtnAyudaAutomatica()
    {
        btnAyudaAutomatica.gameObject.SetActive(true);
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DesactivarBtnAyudaAutomatica()
    {
        btnAyudaAutomatica.gameObject.SetActive(false);
    }

    /// <summary>
    /// Para habilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void ActivarBtnAyudaPista()
    {
        btnAyudaPista.gameObject.SetActive(true);
    }

    /// <summary>
    /// Para deshabilitar el interactuable del boton salir donde se requiera
    /// </summary>
    public void DesactivarBtnAyudaPista()
    {
        btnAyudaPista.gameObject.SetActive(false);
    }

    /// <summary>
    /// Para habilitar el interactuable del BtnBajarPlataforma salir donde se requiera
    /// </summary>
    public void HabilitarBtnBajarPlataforma()
    {
        btnBajarPlataforma.interactable = true;
    }

    /// <summary>
    /// Para deshabilitar el interactuable del BtnBajarPlataforma salir donde se requiera
    /// </summary>
    public void DeshabilitarBtnBajarPlataforma()
    {
        btnBajarPlataforma.interactable = false;
    }


    /// <summary>
    /// Para habilitar los botones de expandir donde se requiera
    /// </summary>
    public void HabilitarBtnExpandir()
    {
        for (int i = 0; i < btnExpandir.Length; i++)
        {
            btnExpandir[i].interactable = true;
        }
    }

    /// <summary>
    /// Para deshabilitar los botones de expandir donde se requiera
    /// </summary>
    public void DeshabilitarBtnExpandir()
    {
        for (int i = 0; i < btnExpandir.Length; i++)
        {
            btnExpandir[i].interactable = false;
        }
    }

    public void ActivarPausa()
    {
        if (puedoPausar)
        {
            if (!juegoPausado)
            {
                if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Menu", 0.5f); // Ejecutamos el efecto nombrado
                juegoPausado = true;
                menuPausa.EntranceAnimation();
                movimientoJugador.DeneterJugador();
                if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.DeneterCamara();
            }
            else
            {
                if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnOmitir", 0.5f); // Ejecutamos el efecto nombrado
                juegoPausado = false;
                menuPausa.ExitAnimation();
                movimientoJugador.HabilitarJugador();
                if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.HabilitarCamara();
            }
        }    
    }

    /// <summary>
    /// Metodo invocado desde btnElegirMotor en el menu de pausa
    /// </summary>
    public void ActivarMenuEleccionMotor()
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnElegir", 0.5f); // Ejecutamos el efecto nombrado
        menuEleccionMotor.Escalar();
    }

    /// <summary>
    /// Metodo sin utilizar aun
    /// </summary>
    public void DesactivarMenuEleccionMotor()
    {
        menuEleccionMotor.Restaurar();
    }

    /// <summary>
    /// Metodo invocado desde los suelos interactivos al momento de entrar al trigger
    /// </summary>
    public void ActivarBTNEleccionMotor()
    {
        if (ServidorMotores.singleton.esMecanico)
        {
            if (!ManagerDesplazamientoMotor.singleton.desplazamientoIniciado) imagenBLoqueoMotor.SetActive(false);
        }

        if (EnvioDatosBD.singleton.usuario.tipo_usuario == "1")
        {
            if (!ManagerDesplazamientoMotor.singleton.desplazamientoIniciado) imagenBLoqueoMecanico.SetActive(false);
        }     
    }

    /// <summary>
    /// Metodo invocado desde los suelos interactivos al momento de salir del trigger
    /// </summary>
    public void DesactivarBTNEleccionMotor()
    {
        //btnEleccionMotor.SetActive(false);

        imagenBLoqueoMotor.SetActive(true);

        if (EnvioDatosBD.singleton.usuario.tipo_usuario == "1")
        {
            imagenBLoqueoMecanico.SetActive(true);
        }    
    }

    public void PuedoPausarJuego()
    {
        puedoPausar = true;
        movimientoJugador.HabilitarJugador();
        if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.HabilitarCamara();
    }

    public void NoPuedoPausarJuego()
    {
        puedoPausar = false;
        movimientoJugador.DeneterJugador();
        if (CamaraOrbital.singleton != null) CamaraOrbital.singleton.DeneterCamara();
    }

    public void Salir()
    {
        Application.Quit();
    }

    /// <summary>
    /// Cambia de escena seg�n el nombre recibido
    /// </summary>
    /// <param name="sceneName">Nombre exacto de la escena</param>
    public void CambiarEscena(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning(" El nombre de la escena est� vac�o o es nulo");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
