using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WEB_API_CORE.Servicios.DataBase;

namespace WEB_API_CORE.Core.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CoreController : ControllerBase
{
    private readonly DataBaseService _db;

    public CoreController(DataBaseService db)
    {
        _db = db;
    }

    [Authorize]
    [HttpGet("week")]
    public async Task<IActionResult> GetReporte([FromQuery] DateOnly? FechaInicial, [FromQuery] DateOnly? FechaFinal )
    {
            // Usa las fechas recibidas o valores por defecto
            var fechaInicio = FechaInicial?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(-7);
            var fechaFin = FechaFinal?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.Now;

            var parametros = new[]
            {
                new SqlParameter("@pFechaInicio", System.Data.SqlDbType.DateTime) { Value = fechaInicio },
                new SqlParameter("@pFechaFin", System.Data.SqlDbType.DateTime) { Value = fechaFin }
            };
            var ds = await _db.ExecuteStoredProcedureAsync("sp_GetWeek", parametros);

        if (ds.Tables.Contains("Error"))
            return BadRequest(ds.Tables["Error"].Rows[0]["ErrorMessage"]);
            ds.Tables[0].TableName = "fechas";
        

        return Ok(ds);
    }
}
}
