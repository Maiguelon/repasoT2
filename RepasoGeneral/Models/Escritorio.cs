namespace alquilerlaboral;

public class Escritorio : Espacio
{
    public enum ubicacion_escritorio
    {
        Ventana = 1,
        Pasillo = 2,
        Rincon = 3
    }
    public bool EsDePie {get; set;}
    public ubicacion_escritorio Ubicacion {get; set;}

    public bool Parado()
    {
        return EsDePie;
    }

    public ubicacion_escritorio Lugar()
    {
        return Ubicacion;
    }
}