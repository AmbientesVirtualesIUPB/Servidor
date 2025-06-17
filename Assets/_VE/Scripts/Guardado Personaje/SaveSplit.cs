
using System;

//En esta clase podemos agregar todas las variables que deseemos guardar en formato Json
[Serializable]
public class SaveSplit
{
    //Variable para guardas las posiciones de la personalizacion del personaje
    public string posiciones = "";
    public string colores = "";
    public string furtivos = "";
    //Constructor con informacion por defecto
    /*
    public SaveSplit()
    {
        posiciones = "";
        colores = "";
        furtivos = "";
    }
    */
}
