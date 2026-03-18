using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestioMosionesPersonales : MonoBehaviour
{
    public List<Mision> misiones;
    public static GestioMosionesPersonales singleton;
    public UnityEvent eventoCompletaObjetivo;
    public UnityEvent eventoCompletaMision;

    private void Awake()
    {
        singleton = this;
    }

    public void AdquirirMision(int id)
    {
        if (!ValidarMisionPorID(id))
        {
            misiones.Add(MorionMisiones.singleton.misiones[id]);
            for (int i = 0; i < MorionMisiones.singleton.misiones[id].subMisions.Length; i++)
            {
                PuntoMisionMiniMapa.singleton.AgregarMision(MorionMisiones.singleton.misiones[id].subMisions[i].puntoObjetivo);
                UIBotonera.singleton.ActualizarMisiones();
            }
        }
    }
    
    public bool ValidarMisionPorNombre(string misiono)
    {
        for (int i = 0; i < misiones.Count; i++)
        {
            if (misiones[i].nombre.Equals(misiono))
                return true;
        }
        return false;
    }

    public bool ValidarMisionPorID(int id)
    {
        for (int i = 0; i < misiones.Count; i++)
        {
            if (misiones[i].nombre.Equals(MorionMisiones.singleton.misiones[id].nombre))
                return true;
        }
        return false;
    }

    public bool CompletarSubMision(int _mision, int _submision)
    {
        for (int i = 0; i < misiones.Count; i++)
        {
            if (misiones[i].nombre.Equals(MorionMisiones.singleton.misiones[_mision].nombre))
            {
                if(!misiones[i].subMisions[_submision].rescatada)
                {
                    misiones[i].subMisions[_submision].rescatada = true;
                    PuntoMisionMiniMapa.singleton.EliminarMision(MorionMisiones.singleton.misiones[_mision].subMisions[_submision].puntoObjetivo);
                    print("COMPLETADA LA MISION " + _mision + " Y LA SUBMISION " + _submision + " CON EL PUNTO " + MorionMisiones.singleton.misiones[_mision].subMisions[_submision].puntoObjetivo.ToString());
                    if (VerificarMisionCompleta(_mision))
                    {
                        UIBotonera.singleton.ActualizarMisiones();
                        print("PASO POR AQUI ");
                        eventoCompletaMision.Invoke();
                        MorionMisiones.singleton.misiones[_mision].misionCumplida.Invoke();
                    }
                    else
                    {
                        eventoCompletaObjetivo.Invoke();
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public bool VerificarMisionCompleta(int _mision)
    {
        bool exacto = true;
        for (int i = 0; i < misiones[_mision].subMisions.Length; i++)
        {
            exacto = exacto && misiones[_mision].subMisions[i].rescatada;
        }
        return exacto;
    }
}
