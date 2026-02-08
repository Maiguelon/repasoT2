namespace alquilerlaboral;

public class Espacio
{
    public int Id {get; set;}
    public string Nombre {get; set;}
    public decimal PrecioHora {get; set;}
    public List<Reserva> Reservas {get; set;}
}