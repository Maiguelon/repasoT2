namespace alquilerlaboral;

public class Reserva
{
    public enum dia_semana
    {
        Lunes = 1,
        Martes = 2,
        Miercoles = 3,
        Jueves = 4,
        Viernes = 5
    }

    public dia_semana DiaAlquilado {get; set;}
    public int HoraInicio {get; set;} // usar * 60
    public int DuracionHoras {get; set;}
    public string NombreCliente {get; set;}

    public int EndTime()
    {
        // No necesito validaciones porque lo hago en el controller
        int HoraFinal = HoraInicio + DuracionHoras * 60;
        return HoraFinal;
    }

    public bool DiaValido()
    {
        return Enum.IsDefined(typeof(dia_semana), this.DiaAlquilado);
    }
}