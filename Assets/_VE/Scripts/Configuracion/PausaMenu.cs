using TMPro;
using UnityEngine;

public class PausaMenu : MonoBehaviour
{
    public static PausaMenu Instance; // Singleton

    public EnvioDatosBD envioDatosBD;
    public TextMeshProUGUI txtID;
    public bool juegoPausado = false;

    private Animator menuPausa;

    private void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        menuPausa = GetComponent<Animator>();
        // Busca el objeto por nombre para buscar la referencia al objeto que administra la base de datos, ya que este pasar� entre escenas
        GameObject obj = GameObject.Find("EnvioBD");
        
        if (obj != null)
        {
            envioDatosBD = obj.GetComponent<EnvioDatosBD>(); // Si encuentra el objeto lo almacenamos en la variable
            txtID.text = envioDatosBD.correo;
            
        }
        else
        {
            envioDatosBD = null;
        }
    }

    void Update()
    {
        if (!juegoPausado)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuPausa.SetBool("mostrar", true);
                juegoPausado = true;
                AudioManager.Instance.PlayEfect(0);
            }
        }     
    }

    public void Despausar()
    {
        AudioManager.Instance.PlayEfect(0);
        juegoPausado = false;
        menuPausa.SetBool("mostrar", false);
    }
}
