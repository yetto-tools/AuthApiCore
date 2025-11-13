namespace WEB_API_CORE.Servicios.Auth
{
    using System.Data;
    using BCrypt.Net;
    using Microsoft.Data.SqlClient;
    using WEB_API_CORE.Core.Authenticate.Models;
    using WEB_API_CORE.Servicios.DataBase;
    
    using WEB_API_CORE.Servicios.JWT;


    public class AuthService
    {
        private readonly DataBaseService _db;
        private readonly JwtService _jwt;

        public AuthService(DataBaseService db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<DataSet> RegistrarUsuarioAsync(string nombre, string email, string password)
        {
            string passwordHash = BCrypt.HashPassword(password);

            var parametros = new[]
            {
                new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = nombre },
                new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email },
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 200) { Value = passwordHash }
            };

            // Puedes ejecutar un procedimiento almacenado de registro
            return await _db.ExecuteStoredProcedureAsync("sp_RegistrarUsuario", parametros);
        }

        public async Task<DataSet> ValidarLoginAsync(string email, string password)
        {
            var parametros = new[]
            {
                new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email }
            };

            var ds = await _db.ExecuteStoredProcedureAsync("sp_LoginUsuario", parametros);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return _db.GetErrorDataSet("Usuario no encontrado.");

            var userRow = ds.Tables[0].Rows[0];
            string hash = userRow["PasswordHash"].ToString();

            if (!BCrypt.Verify(password, hash))
                return _db.GetErrorDataSet("Contraseña incorrecta.");

            // Si llega aquí, login correcto
            return ds;
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario)
        {
            var parametros = new[]
                {
            new SqlParameter("@IdUsuario", idUsuario)
        };

            var ds = await _db.ExecuteStoredProcedureAsync("spUsuarios_ObtenerPorId", parametros);
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            var row = ds.Tables[0].Rows[0];
            return new Usuario
            {
                Id = Convert.ToInt32(row["Id"]),
                UserRef = Guid.Parse(row["UserRef"].ToString()!),
                Nombre = row["Nombre"].ToString()!,
                Email = row["Email"].ToString()!,
                Rol = row["Rol"].ToString()!,
                Activo = Convert.ToBoolean(row["Activo"])
            };
        }

    }


}
