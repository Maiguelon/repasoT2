using Microsoft.AspNetCore.SignalR;

namespace alquilerlaboral;

public class Oficina : Espacio
{
    public int CapacidadPersonas {get; set;}
    public bool TieneProyector {get; set;}

    public double CalcularPrecioOficina()
    {
        double costo = PrecioHora;
        if (TieneProyector)
        {
            costo *= 0.1;
        }
        return costo;
    }

    public int MaximoPersonas()
    {
        return CapacidadPersonas;
    }

    public bool EsProyectable()
    {
        return TieneProyector;
    }
}