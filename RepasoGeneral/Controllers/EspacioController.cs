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
}