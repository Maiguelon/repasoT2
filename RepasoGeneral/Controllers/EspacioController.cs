using Microsoft.AspNetCore.Mvc;
using alquilerlaboral;
using System.Text.Json;
namespace ConstructoraApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EspacioController : ControllerBase
{
    private List<Espacio> espacios;
    private AccesoADatosEspacioJson ADEspacio;
    string rutaEspacios = Path.Combine("Data", "Data.json");

    public EspacioController() // constructor
    {
        ADEspacio = new AccesoADatosEspacioJson();
        espacios = ADEspacio.CargarEspacios(rutaEspacios);
    }

    // ----- GET -----
    [HttpGet("GetEspacios")]
    public ActionResult<List<Espacio>> GetEspacios()
    {
        if (!(espacios == null))
        {
            return Ok(espacios);
        } else
        {
            return BadRequest("No hay espacios cargados");
        }
    }

    // ----- Post -----
    [HttpPost("AgregarEspacio")]
    public ActionResult<string> AgregarEspacio([FromBody] Espacio nuevo)
    {
        // Checkeos reglas de negocio
        if (nuevo == null)
        {
            return BadRequest("No se encontraron los datos del nuevo espacio");
        }
        if (nuevo.PrecioHora < 500) // CONSULTAR DECIMALES
        {
            return BadRequest("El precio por hora debe ser mayor a $500.");
        }

        // Checkeo oficina
        if (nuevo is Oficina oficina) // PREGUNTAR CASTEO
        {
            if (oficina.CapacidadPersonas <= 2)
            {
                return BadRequest("Las oficinas deben tener capacidad mayor a 2 personas.");
            }
        }

        // Checkeo Escritorio
        if (nuevo is Escritorio escritorio)
        {
            if (escritorio.Parado() && escritorio.Ubicacion == Escritorio.ubicacion_escritorio.Ventana)
            {
                return BadRequest("Un Escritorio de Pie no puede ir del lado de la ventana");
            }
        }

        // Asignacion automatica de Id
        int NuevoId = espacios.Count > 0 ? espacios.Max(e => e.Id) + 1 : 1;
        nuevo.Id = NuevoId;
        espacios.Add(nuevo);
        ADEspacio.GuardarEspacios(rutaEspacios, espacios);
        return Created("", nuevo);
    }
}