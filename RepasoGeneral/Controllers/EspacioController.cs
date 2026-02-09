using Microsoft.AspNetCore.Mvc;
using alquilerlaboral;
using System.Text.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Http.HttpResults;
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
        }
        else
        {
            return BadRequest("No hay espacios cargados");
        }
    }

    // Devuelve espacios que no tengan reservas en un dia dado
    [HttpGet("GetEspaciosDisponibles/{solicitado}")]
    public ActionResult<List<Espacio>> GetEspaciosDisponibles(Reserva.dia_semana solicitado)
    {
        List<Espacio> disponibles = espacios.Where(e =>
        e.Reservas == null ||
        !e.Reservas.Any(r => r.DiaAlquilado == solicitado)).ToList();

        if (disponibles.Count > 0)
        {
            return Ok(disponibles);
        }
        else
        {
            return NotFound("No hay espacios disponibles ese día");
        }
    }

    [HttpGet("GetRango/{dia}/{inicio}/{fin}")] // obeter RESERVAS en una franja
    public ActionResult<List<Espacio>> GetRango(Reserva.dia_semana dia, int inicio, int fin)
    {
        List<Reserva> enRango = new List<Reserva>();
        foreach (var e in espacios)
        {
            if (e.Reservas != null)
            {
                foreach (var booked in e.Reservas)
                {
                    if (booked.DiaAlquilado == dia)
                    {
                        if ((booked.HoraInicio < fin && booked.HoraInicio > inicio) ||
                        (booked.EndTime() < fin && booked.EndTime() > inicio) ||
                        (booked.HoraInicio > inicio && booked.EndTime() < fin))
                        {
                            enRango.Add(booked);
                        }
                    }
                }
            }
        }
        if (enRango.Count > 0)
        {
            return Ok(enRango);
        }
        else
        {
            return NotFound("No se encontraron reservas en esa franja horaria");
        }
    }

    [HttpGet("GetInforme")]
    public ActionResult<List<object>> GetInforme()
    {
        List<object> lista = new List<object>();
        foreach (var e in espacios)
        {
            string tipoEspacio = "";
            if (e is Oficina)
            {
                tipoEspacio = "Oficina";
            }
            else
            {
                tipoEspacio = "Escritorio";
            }
            var info = new
            {
                Nombre = e.Nombre,
                Tipo = tipoEspacio,
                GananciaTotal = e.Ganancia()
            };
            lista.Add(info);
        }
        if (lista.Count > 0)
        {
            return Ok(lista);
        }
        else
        {
            return NotFound("Error, no se encontraron espacios para el informe");
        }
    }

    // ----- POST -----
    [HttpPost("AgregarEspacio")]
    public ActionResult<string> AgregarEspacio([FromBody] Espacio nuevo)
    {
        // Checkeos reglas de negocio
        if (nuevo == null)
        {
            return BadRequest("No se encontraron los datos del nuevo espacio");
        }
        if (nuevo.PrecioHora < 500)
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


    // ----- PUT -----
    [HttpPut("AgregarReserva/{idEspacio}")]
    public ActionResult AgregarReservaPorId(int idEspacio, [FromBody] Reserva nuevaReserva)
    {
        Espacio aReservar = espacios.FirstOrDefault(e => e.Id == idEspacio);
        if (aReservar == null)
        {
            return BadRequest("No hay espacio con ese Id");
        }

        // Checks Reglas de negocio
        if (!nuevaReserva.DiaValido())
        {
            return BadRequest("Solo se puede alquilar de Lunes a Viernes");
        }
        if (nuevaReserva.HoraInicio < 540 || nuevaReserva.EndTime() > 1080)
        {
            return BadRequest("Solo se puede alquilar entre las 9 y 18 hs");
        }
        if (nuevaReserva.DuracionHoras < 1 || nuevaReserva.DuracionHoras > 4)
        {
            return BadRequest("Las reservas solo pueden durar entre 1 y 4 horas");
        }

        // Check solapamiento con todos
        // foreach (var e in espacios)
        // {
        //     if (e.Reservas.Any(r => r.DiaAlquilado == nuevaReserva.DiaAlquilado))
        //     {
        //         foreach (var r in e.Reservas)
        //         {
        //             if (nuevaReserva.HoraInicio < r.EndTime() && nuevaReserva.EndTime() > r.HoraInicio)
        //             {
        //                 return BadRequest("Hay solapamiento con otro alquiler");
        //             }
        //         }
        //     }
        // }

        // solapamiento solo con el propio espacio
        bool haySolapamiento = aReservar.Reservas.Any(r =>
        r.DiaAlquilado == nuevaReserva.DiaAlquilado &&
        nuevaReserva.HoraInicio < r.EndTime() && nuevaReserva.EndTime() > r.HoraInicio);
        if (haySolapamiento)
        {
            return BadRequest("Hay solapamiento de reservas por ese espacio en ese momento");
        }

        aReservar.Reservas.Add(nuevaReserva); // reservo
        ADEspacio.GuardarEspacios(rutaEspacios, espacios);
        return Ok("Espacio Reservado Exitosamente");
    }

    // ----- DELETE -----
    [HttpDelete("BorrarEspacio/{idABorrar}")]
    public ActionResult BorrarEspacio(int idABorrar)
    {
        Espacio aBorrar = espacios.FirstOrDefault(e=> e.Id == idABorrar);
        if (aBorrar == null)
        {
            return BadRequest("No hay espacio con ese Id");
        }

        espacios.Remove(aBorrar);
        ADEspacio.GuardarEspacios(rutaEspacios, espacios);
        return Ok("Espacio aniquilado existosamente.");
    }
}