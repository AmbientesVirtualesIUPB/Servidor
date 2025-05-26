using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MorionID))]
[AddComponentMenu("Morion Servidor/Morion Tranform/Morion Sub-Tranform")]
public class MorionSubTransform : MonoBehaviour
{
    public bool debugEnConsola = false;
    [InfoMessage("Este componente es obligatorio.", MessageTypeCustom.Warning)]
    public MorionTransform mTransform;

    private float _toleranciaPosicion;
    private float _toleranciaRotacion;
    
    [HideInInspector]
    public MorionID morionID;
    private Vector3 posAnterior;
    private Vector3 rotAnterior;

    private void Awake()
    {
        morionID = GetComponent<MorionID>();
    }

    private void Start()
    {
        if (mTransform == null)
        {
            this.enabled = false;
            if (debugEnConsola)
            {
                Debug.LogError("No se encontró el MorionTransformManager para el objeto " + gameObject.name);
            }
            return;
        }
        posAnterior = transform.localPosition;
        rotAnterior = transform.localEulerAngles;
        _toleranciaPosicion = mTransform.toleranciaPosicion * mTransform.toleranciaPosicion;
        _toleranciaRotacion = mTransform.toleranciaRotacion * mTransform.toleranciaRotacion;
        mTransform.Registrar(this);
    }

    private void FixedUpdate()
    {
        if (mTransform == null)
        {
            return;
        }
        if (morionID.GetOwner())
        {
            if (((posAnterior - transform.localPosition).sqrMagnitude > _toleranciaPosicion ||
            (rotAnterior - transform.localEulerAngles).sqrMagnitude > _toleranciaRotacion))
            {
                DatoActualizableTransform dat = new DatoActualizableTransform();
                dat.id = morionID.GetID();
                dat.pos = transform.localPosition;
                dat.rot = transform.localEulerAngles;
                mTransform.NotificarCambioSubTransform(dat);
                posAnterior = transform.localPosition;
                rotAnterior = transform.localEulerAngles;
            }
        }

    }
}
