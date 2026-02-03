using System.Collections.Generic;
using UnityEngine;

public class ListaUsuariosMotores : MonoBehaviour
{
    public BtnUsuarioLista btnMuestra;
    public UIAutoAnimation autoAnimation;
    public GameObject btnMostrarLista;
    public bool visible;
    List<GameObject> listaUsuarios;


    public static ListaUsuariosMotores singleton;

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
        listaUsuarios = new List<GameObject>();
        btnMuestra.gameObject.SetActive(false);
    }

    public void Mostrar()
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnElegir", 0.5f); // Ejecutamos el efecto nombrado
        visible = true;
        for (int i = 0; i < listaUsuarios.Count; i++)
        {
            Destroy(listaUsuarios[i]);
        }

        listaUsuarios.Clear();

        btnMuestra.gameObject.SetActive(true);

        //CrearBoton(EnvioDatosBD.singleton.usuario.id_usuario); //// Cambiar eventualmente
        //for (int i = 0; i < ControlUsuarios.singleton.usuarios.Count; i++)
        //{
        //    string nombre = ControlUsuarios.singleton.usuarios[i].gameObject.name.Substring(12);
        //    CrearBoton(nombre);
        //}
        //void CrearBoton(string _nombre)
        //{
        //    btnMuestra.Inicializar(_nombre, _nombre);
        //    listaUsuarios.Add(Instantiate(btnMuestra.gameObject, btnMuestra.transform.parent) as GameObject);
        //}
        btnMuestra.gameObject.SetActive(false);
        autoAnimation.EntranceAnimation();
    }

    public void Ocultar()
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("btnOmitir", 0.5f); // Ejecutamos el efecto nombrado
        visible = false;
        autoAnimation.ExitAnimation();
    }

    public void Invertir()
    {
        if (visible)
        {
            Ocultar();
        }
        else
        {
            Mostrar();
        }
    }

    public void MostrarBotonListaUsuarios()
    {
        btnMostrarLista.SetActive(EnvioDatosBD.singleton.usuario.tipo_usuario == "1");
        ServidorMotores.singleton.esMecanico = EnvioDatosBD.singleton.usuario.tipo_usuario == "1";
        ServidorMotores.singleton.sueloInteractivoCollider.enabled = EnvioDatosBD.singleton.usuario.tipo_usuario == "1";
    }
}
