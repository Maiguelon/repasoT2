namespace alquilerlaboral;

public class Reserva
{
    public enum dia_semana
    {
        Lunes = 1,
        Martes = 2,
        Miercoles = 3,
        Jueves = 4,
        Viernes = 5,
        Sabado = 6,
        Domingo = 7
    }

    public dia_semana DiaAlquilado {get; set;}
    public int HoraInicio {get; set;}
    public int DuracionMinutos {get; set;}
    public string NombreCliente {get; set;}
}