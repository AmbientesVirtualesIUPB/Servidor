using System.Collections.Generic;
using UnityEngine;

public class GestorObjetos : MonoBehaviour
{
    public static GestorObjetos instancia;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------------------------------------------------
    //  Obtener o crear registro del objeto
    // ---------------------------------------------------------
    private ObjetoGuardado ObtenerRegistro(string id)
    {
        foreach (var obj in SistemaGuardado.instancia.Datos.objetos.objetos)
        {
            if (obj.idObjeto == id)
                return obj;
        }

        // No existe → crearlo
        ObjetoGuardado nuevo = new ObjetoGuardado();
        nuevo.idObjeto = id;
        nuevo.descubierto = false;
        nuevo.infoDesbloqueada = false;
        nuevo.cantidad = 0;

        SistemaGuardado.instancia.Datos.objetos.objetos.Add(nuevo);
        return nuevo;
    }

    // ---------------------------------------------------------
    //  Marcar como descubierto
    // ---------------------------------------------------------
    public void MarcarDescubierto(ObjetoMisionBase obj)
    {
        ObjetoGuardado registro = ObtenerRegistro(obj.idObjeto.ToString());

        if (!registro.descubierto)
        {
            registro.descubierto = true;
            SistemaGuardado.instancia.GuardarDatos();

            UI_AlertaInfo.instancia.Mostrar(
                "Nuevo objeto descubierto: " + obj.nombreObjeto);
        }
    }

    // ---------------------------------------------------------
    //  Desbloquear información
    // ---------------------------------------------------------
    public void DesbloquearInformacion(ObjetoMisionBase obj)
    {
        ObjetoGuardado registro = ObtenerRegistro(obj.idObjeto.ToString());

        if (!registro.infoDesbloqueada)
        {
            registro.infoDesbloqueada = true;
            SistemaGuardado.instancia.GuardarDatos();

            UI_AlertaInfo.instancia.Mostrar(
                "Nueva información desbloqueada:\n" + obj.nombreObjeto);
        }
    }

    // ---------------------------------------------------------
    //  Sumar cantidad (recolección)
    // ---------------------------------------------------------
    public void SumarCantidad(ObjetoMisionBase obj)
    {
        ObjetoGuardado registro = ObtenerRegistro(obj.idObjeto.ToString());
        registro.cantidad++;
        SistemaGuardado.instancia.GuardarDatos();
    }

    // ---------------------------------------------------------
    //  Consultas
    // ---------------------------------------------------------
    public bool EstaDescubierto(ObjetoMisionBase obj)
    {
        return ObtenerRegistro(obj.idObjeto.ToString()).descubierto;
    }

    public bool InfoDesbloqueada(ObjetoMisionBase obj)
    {
        return ObtenerRegistro(obj.idObjeto.ToString()).infoDesbloqueada;
    }
    public void DesbloquearInmediatamente(ObjetoMisionBase obj)
    {
        MarcarDescubierto(obj);
        DesbloquearInformacion(obj);
    }
}
