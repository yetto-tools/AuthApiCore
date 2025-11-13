using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WEB_API_CORE.Middleware;
using WEB_API_CORE.Servicios.Auth;
using WEB_API_CORE.Servicios.DataBase;
using WEB_API_CORE.Servicios.JWT;

namespace WEB_API_CORE.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly AuthService _auth;
        private readonly DataBaseService _db;



        public UsuariosController(JwtService jwt, AuthService auth, DataBaseService db)
        {
            _auth = auth;
            _jwt = jwt;
            _db = db;

        }



        // 🔹 Endpoint protegido: obtiene perfil del usuario logueado
        [Authorize]
        [RequireJwtCookie] // ✅ Este filtro asegura que exista la cookie
        [HttpGet("")]
        public async Task<IActionResult> Usuarios([FromQuery] int? Estado)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized(new { message = "Usuario no autenticado" });

            if (!int.TryParse(idClaim, out int userId))
                return BadRequest(new { message = "ID de usuario inválido" });


            var parametros = new[]
            {
                new SqlParameter("@pEstado", System.Data.SqlDbType.Int) { Value = (object?)Estado ?? DBNull.Value  },
            };

            var users = await _db.ExecuteStoredProcedureAsync("sp_Listar_Usuarios", parametros);

            if (users.Tables.Count == 0)
                return Ok(new { message = "No se encontraron usuarios", users = users.Tables[0] });



            return Ok(new
            {
                message=$"Resultados encontrados {users.Tables.Count}",
                users= users.Tables[0]
            });
        }
    }
}
