using System;
using System.Collections.Generic;
using UnityEngine;

public class ControladorBancoDePeces : MonoBehaviour
{
    [Header("Rango de Aparicion")]
    public Vector3 puntoMinimoSpawn = new Vector3(-10f, -2f, -10f);
    public Vector3 puntoMaximoSpawn = new Vector3(10f, 2f, 10f);

    [Header("Prefab y Cantidad")]
    public GameObject prefabPez;
    [Min(1)] public int cantidadPeces = 20;

    [Header("Lugares Objetivo")]
    public Transform[] lugaresObjetivo;

    [Header("Movimiento")]
    public float velocidadMovimiento = 2.5f;
    public float fuerzaDireccion = 4.0f;
    public float radioMerodeoObjetivo = 1.5f;
    public float distanciaLlegada = 0.75f;

    [Header("Movimiento Tipo Agua")]
    public float amplitudOndulacion = 0.25f;
    public float frecuenciaOndulacion = 1.2f;

    [Header("Cambio de Objetivo")]
    public Vector2 tiempoCambioObjetivo = new Vector2(4f, 10f);
    [Range(0f, 1f)] public float probabilidadCambioObjetivo = 0.35f;

    [Header("Rotacion")]
    public bool rotarHaciaMovimiento = true;
    public float velocidadRotacion = 6f;

    // -----------------------------

    [Serializable]
    public class DatosPez
    {
        public GameObject pez;
        public int idObjetivo;

        public Vector3 offsetLocal;
        public Vector3 velocidadActual;
        public float proximoCambio;
        public float semillaRuido;
    }

    [Header("Lista de peces")]
    public List<DatosPez> listaPeces = new List<DatosPez>();

    // -----------------------------

    void Start()
    {
        if (prefabPez == null || lugaresObjetivo.Length == 0)
        {
            Debug.LogError("Faltan referencias en ControladorBancoDePeces");
            enabled = false;
            return;
        }

        GenerarPeces();
    }

    void GenerarPeces()
    {
        listaPeces.Clear();

        for (int i = 0; i < cantidadPeces; i++)
        {
            Vector3 posicion = transform.position + PuntoAleatorioCaja(puntoMinimoSpawn, puntoMaximoSpawn);
            GameObject nuevoPez = Instantiate(prefabPez, posicion, Quaternion.identity, transform);

            DatosPez datos = new DatosPez();
            datos.pez = nuevoPez;
            datos.idObjetivo = UnityEngine.Random.Range(0, lugaresObjetivo.Length);
            datos.offsetLocal = UnityEngine.Random.insideUnitSphere * radioMerodeoObjetivo;
            datos.velocidadActual = Vector3.zero;
            datos.semillaRuido = UnityEngine.Random.Range(0f, 1000f);
            datos.proximoCambio = Time.time + UnityEngine.Random.Range(
                tiempoCambioObjetivo.x,
                tiempoCambioObjetivo.y
            );

            listaPeces.Add(datos);
        }
    }

    // -----------------------------

    void Update()
    {
        float dt = Time.deltaTime;

        for (int i = listaPeces.Count - 1; i >= 0; i--)
        {
            DatosPez pez = listaPeces[i];

            if (pez.pez == null)
            {
                listaPeces.RemoveAt(i);
                continue;
            }

            if (pez.idObjetivo < 0 || pez.idObjetivo >= lugaresObjetivo.Length)
                pez.idObjetivo = UnityEngine.Random.Range(0, lugaresObjetivo.Length);

            Transform objetivo = lugaresObjetivo[pez.idObjetivo];

            Vector3 posicionObjetivo = objetivo.position + pez.offsetLocal;

            float onda = Mathf.Sin((Time.time + pez.semillaRuido) * frecuenciaOndulacion) * amplitudOndulacion;
            posicionObjetivo.y += onda;

            Vector3 posicionActual = pez.pez.transform.position;
            Vector3 direccion = posicionObjetivo - posicionActual;

            if (direccion.magnitude < distanciaLlegada)
            {
                pez.offsetLocal = UnityEngine.Random.insideUnitSphere * radioMerodeoObjetivo;
            }

            Vector3 velocidadDeseada = direccion.normalized * velocidadMovimiento;
            pez.velocidadActual = Vector3.Lerp(
                pez.velocidadActual,
                velocidadDeseada,
                1f - Mathf.Exp(-fuerzaDireccion * dt)
            );

            pez.pez.transform.position += pez.velocidadActual * dt;

            if (rotarHaciaMovimiento && pez.velocidadActual.sqrMagnitude > 0.001f)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(pez.velocidadActual.normalized);

                pez.pez.transform.rotation = Quaternion.Slerp(
                    pez.pez.transform.rotation,
                    rotacionObjetivo,
                    1f - Mathf.Exp(-velocidadRotacion * dt)
                );
            }

            // Cambio ocasional de objetivo
            if (Time.time >= pez.proximoCambio)
            {
                pez.proximoCambio = Time.time + UnityEngine.Random.Range(
                    tiempoCambioObjetivo.x,
                    tiempoCambioObjetivo.y
                );

                if (UnityEngine.Random.value < probabilidadCambioObjetivo &&
                    lugaresObjetivo.Length > 1)
                {
                    int nuevo = pez.idObjetivo;

                    while (nuevo == pez.idObjetivo)
                        nuevo = UnityEngine.Random.Range(0, lugaresObjetivo.Length);

                    pez.idObjetivo = nuevo;
                    pez.offsetLocal = UnityEngine.Random.insideUnitSphere * radioMerodeoObjetivo;
                }
            }
        }
    }

    // -----------------------------

    Vector3 PuntoAleatorioCaja(Vector3 a, Vector3 b)
    {
        return new Vector3(
            UnityEngine.Random.Range(Mathf.Min(a.x, b.x), Mathf.Max(a.x, b.x)),
            UnityEngine.Random.Range(Mathf.Min(a.y, b.y), Mathf.Max(a.y, b.y)),
            UnityEngine.Random.Range(Mathf.Min(a.z, b.z), Mathf.Max(a.z, b.z))
        );
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 centro = transform.position + (puntoMinimoSpawn + puntoMaximoSpawn) * 0.5f;
        Vector3 tamano = new Vector3(
            Mathf.Abs(puntoMaximoSpawn.x - puntoMinimoSpawn.x),
            Mathf.Abs(puntoMaximoSpawn.y - puntoMinimoSpawn.y),
            Mathf.Abs(puntoMaximoSpawn.z - puntoMinimoSpawn.z)
        );

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(centro, tamano);
    }
#endif
}
