using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WEB_API_CORE.Servicios.DataBase
{
    /// <summary>
    /// Servicio optimizado para ejecutar procedimientos almacenados en SQL Server
    /// con soporte para alta concurrencia y configuración moderna (.NET 6+).
    /// </summary>
    public sealed class DataBaseService
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;


        public DataBaseService(IConfiguration configuration, IWebHostEnvironment env)
        {
            // 🔹 Determina entorno actual
            var environment = env.EnvironmentName.ToLower();

            // 🔹 Selecciona la cadena de conexión correcta
            if (environment.Contains("production") || environment.Contains("release"))
            {
                _connectionString = configuration.GetConnectionString("Production")
                    ?? throw new InvalidOperationException("La cadena de conexión 'Production' no está configurada.");
            }
            else
            {
                _connectionString = configuration.GetConnectionString("Development")
                    ?? throw new InvalidOperationException("La cadena de conexión 'Development' no está configurada.");
            }

            // 🔹 Timeout opcional configurable (por defecto 300 segundos)
            _commandTimeout = configuration.GetSection("DatabaseSettings").GetValue<int>("CommandTimeout", 300);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado de forma sincrónica.
        /// </summary>
        public DataSet ExecuteStoredProcedure(string storedProcedureName, IEnumerable<SqlParameter> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedureName))
                throw new ArgumentNullException(nameof(storedProcedureName));

            var dataSet = new DataSet();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(storedProcedureName, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = _commandTimeout; // 5 minutos

                if (parameters != null)
                {
                    foreach (var param in parameters)
                        command.Parameters.Add(param);
                }

                try
                {
                    connection.Open();
                    adapter.Fill(dataSet);
                }
                catch (SqlException sqlEx)
                {
                    return CreateErrorDataSet($"[SQL ERROR] {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    return CreateErrorDataSet($"[GENERAL ERROR] {ex.Message}");
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado de forma asíncrona.
        /// </summary>
        public async Task<DataSet> ExecuteStoredProcedureAsync(
            string storedProcedureName,
            IEnumerable<SqlParameter> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedureName))
                throw new ArgumentNullException(nameof(storedProcedureName));

            var dataSet = new DataSet();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = _commandTimeout;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            command.Parameters.Add(param);
                    }

                    await connection.OpenAsync().ConfigureAwait(false);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dataSet)).ConfigureAwait(false);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                return CreateErrorDataSet($"[SQL ERROR] {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return CreateErrorDataSet($"[GENERAL ERROR] {ex.Message}");
            }

            return dataSet;
        }

        private static DataSet CreateErrorDataSet(string message)
        {
            var dataSet = new DataSet("ErrorResult");
            var table = new DataTable("Error");
            table.Columns.Add("ErrorMessage", typeof(string));
            table.Rows.Add(message);
            dataSet.Tables.Add(table);
            return dataSet;
        }

        public DataSet GetErrorDataSet(string message)
        {
            var ds = new DataSet("ErrorResult");
            var table = new DataTable("Error");
            table.Columns.Add("ErrorMessage", typeof(string));
            table.Rows.Add(message);
            ds.Tables.Add(table);
            return ds;
        }

    }
}
