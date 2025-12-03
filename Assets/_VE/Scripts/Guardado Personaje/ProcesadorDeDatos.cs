
using UnityEngine;

public class ProcesadorDeDatos : MonoBehaviour
{

    //Referenciamos el archivo donde guardaremos la informacion
    public int[] pos = new int[15];

    // Clase para representar los datos recibidos desde el servidor
    [System.Serializable]
    public class PersonalizacionData
    {
        public int id_personalizacion;
        public int id_usuario;
        public int genero;
        public int maleta;
        public int cuerpo;
        public int cabeza;
        public int cejas;
        public int cabello;
        public int reloj;
        public int sombrero;
        public int zapatos;
        public int tamano;
        public int color1;
        public int color2;
        public int color3;
        public int color4;
        public int color5;
        public int carroceria;
        public int aleron;
        public int silla;
        public int volante;
        public int llanta;
        public int bateria;
    }

    // Método para procesar la respuesta JSON
    public int[] RespuestaProcesada(string jsonResponse)
    {
        //Convertimos la informacion de json a entero y pasamos a personalizar
        PersonalizacionData data = JsonUtility.FromJson<PersonalizacionData>(jsonResponse);
        int[] valores = new int[] {
            data.genero, data.maleta, data.cuerpo, data.cabeza, data.cejas, data.cabello, data.reloj,
            data.sombrero, data.zapatos, data.tamano, data.color1, data.color2, data.color3, data.color4,
            data.color5, data.carroceria, data.aleron, data.silla, data.volante, data.llanta, data.bateria
        };

        return valores;
    }

}
