using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MorionID))]
[AddComponentMenu("Morion Servidor/Morion Tranform/Morion Transform")]
public class MorionTransform : MonoBehaviour
{

    [InfoMessage("Este componente (en la escena) es obligatorio.", MessageTypeCustom.Warning)]
    public MorionTransformManager mTransformManager;

    public bool debugEnConsola = false;
    public bool opcionesAvanzadas = false;

    [ConditionalHide("opcionesAvanzadas", true)]
    public Vector3 posicionObjetivo;
    [ConditionalHide("opcionesAvanzadas", true)]
    public Vector3 rotacionObjetivo;

    [ConditionalHide("opcionesAvanzadas", true)]
    public float toleranciaPosicion = 0.2f;
    [ConditionalHide("opcionesAvanzadas", true)]
    public float toleranciaRotacion = 10f;
    [ConditionalHide("opcionesAvanzadas", true)]
    public float velRotacion = 5f;
    [ConditionalHide("opcionesAvanzadas", true)]
    public float velTranslacion = 10f;

    [ConditionalHide("opcionesAvanzadas", true)]
    public float periodoEsperas = 0.2f;

    private float _toleranciaPosicion;
    private float _toleranciaRotacion;

    [HideInInspector]
    public MorionID morionID;
    private Vector3 posAnterior;
    private Vector3 rotAnterior;

    public DatosActualizablesServidor datosActualizables;
    private List<MorionSubTransform> subTransforms = new List<MorionSubTransform>();
    bool actualizar = false;

    private void Awake()
    {
        morionID = GetComponent<MorionID>();
    }
    private void Start()
    {
        MorionTransformManager.singleton.morionTransforms.Add(this);
        datosActualizables.datosPropios.id = morionID.GetID();
        datosActualizables.datosPropios.pos = transform.localPosition;
        datosActualizables.datosPropios.rot = transform.localEulerAngles;
        StartCoroutine(VerificarDatosAEnviar());
    }
    public IEnumerator VerificarDatosAEnviar()
    {
        while (true)
        {
            yield return new WaitForSeconds(periodoEsperas);
            if (Servidor.singleton != null && (datosActualizables.datosActualizables.Count > 0 || actualizar))
            {
                actualizar = false;
                GestionMensajesServidor.singeton.EnviarMensaje("AT01", JsonUtility.ToJson(datosActualizables));
                datosActualizables.datosActualizables.Clear();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mTransformManager == null)
        {
            mTransformManager = MorionTransformManager.singleton;
        }
    }

    public void NotificarCambioSubTransform(DatoActualizableTransform dato)
    {
        if (debugEnConsola)
        {
            print("Notificado un cambio: " + dato.ToString());
        }
        bool estaba = false;
        for (int i = 0; i < datosActualizables.datosActualizables.Count; i++)
        {
            if (datosActualizables.datosActualizables[i].id.Equals(dato.id))
            {
                datosActualizables.datosActualizables[i].pos = dato.pos;
                datosActualizables.datosActualizables[i].rot = dato.rot;
                estaba = true;
                break;
            }
        }
        if (!estaba)
        {
            datosActualizables.datosActualizables.Add(dato);
        }
    }

    public void Registrar(MorionSubTransform mst)
    {
        subTransforms.Add(mst);
    }


    private void FixedUpdate()
    {
        if (morionID.GetOwner())
        {
            if (((posAnterior - transform.localPosition).sqrMagnitude > _toleranciaPosicion ||
            (rotAnterior - transform.localEulerAngles).sqrMagnitude > _toleranciaRotacion))
            {
                DatoActualizableTransform dat = new DatoActualizableTransform();
                dat.id = morionID.GetID();
                dat.pos = transform.localPosition;
                dat.rot = transform.localEulerAngles;

                datosActualizables.datosPropios.pos = transform.localPosition;
                datosActualizables.datosPropios.rot = transform.localEulerAngles;
                actualizar = true;

                posAnterior = transform.localPosition;
                rotAnterior = transform.localEulerAngles;
            }
        }
        else
        {
            ActualizarPosicionRotacion();
        }

    }


    public void ActualizarPosicionRotacion()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, datosActualizables.datosPropios.pos, 0.2f);
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, datosActualizables.datosPropios.rot, 0.2f);

        for (int i = 0; i < datosActualizables.datosActualizables.Count; i++)
        {
            for (int j = 0; j < subTransforms.Count; j++)
            {
                if (datosActualizables.datosActualizables[i].id.Equals(subTransforms[j].morionID.GetID()))
                {
                    subTransforms[j].ActualizarPosicionRotacion(datosActualizables.datosActualizables[i]);
                }
            }
        }
    }

    public void RecibirMensaje(DatosActualizablesServidor datos)
    {
        if (!morionID.GetOwner())
        {
            datosActualizables = datos;
        }
    }

}
[System.Serializable]
public class DatoActualizableTransform
{
    public string id;
    public Vector3 pos;
    public Vector3 rot;
}

[System.Serializable]
public class DatosActualizablesServidor
{
    public DatoActualizableTransform datosPropios = new DatoActualizableTransform();
    public List<DatoActualizableTransform> datosActualizables = new List<DatoActualizableTransform>();
}