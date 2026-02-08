namespace alquilerlaboral;
using System.Text.Json.Serialization; 

// necesario para la herencia
[JsonDerivedType(typeof(Oficina), typeDiscriminator: "oficina")]
[JsonDerivedType(typeof(Escritorio), typeDiscriminator: "escritorio")]
public class Espacio
{
    public int Id {get; set;}
    public string Nombre {get; set;}
    public decimal PrecioHora {get; set;}
    public List<Reserva> Reservas {get; set;}
}