using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ListaUsuariosMotores : MonoBehaviour
{
    public BtnUsuarioLista btnMuestra;
    public UIAutoAnimation autoAnimation;
    public GameObject btnMostrarLista;
    public bool visible;
    List<GameObject> listaUsuarios;

    private IEnumerator Start()
    {
        listaUsuarios = new List<GameObject>();
        btnMuestra.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        if (EnvioDatosBD.singleton != null)
        {
            btnMostrarLista.SetActive(EnvioDatosBD.singleton.usuario.tipo_usuario == "1");        
        }
    }


    public void Mostrar()
    {
        visible = true;
        for (int i = 0; i < listaUsuarios.Count; i++)
        {
            Destroy(listaUsuarios[i]);
        }

        listaUsuarios.Clear();

        btnMuestra.gameObject.SetActive(true);
        for (int i = 0; i < ControlUsuarios.singleton.usuarios.Count; i++)
        {
            string nombre = ControlUsuarios.singleton.usuarios[i].gameObject.name.Substring(12);
            btnMuestra.Inicializar(nombre,nombre);
            listaUsuarios.Add(Instantiate(btnMuestra.gameObject, btnMuestra.transform.parent) as GameObject);
        }
        btnMuestra.gameObject.SetActive(false);
        autoAnimation.EntranceAnimation();
    }

    public void Ocultar()
    {
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
}
