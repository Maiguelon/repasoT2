namespace alquilerlaboral;
using System.Text.Json;

public class AccesoADatosEspacioJson
{
    public List<Espacio> CargarEspacios(string archivo)
    {
        if (!File.Exists(archivo))
        {
            return new List<Espacio>();
        }
        string linea = File.ReadAllText(archivo);
        List<Espacio> espacios = JsonSerializer.Deserialize<List<Espacio>>(linea);
        return espacios ?? new List<Espacio>();
    }

    public void GuardarEspacios(string archivo, List<Espacio> espacios)
    {
        var opciones = new JsonSerializerOptions {WriteIndented = true};
        string json = JsonSerializer.Serialize(espacios, opciones);
        File.WriteAllText(archivo, json);
    }
}
