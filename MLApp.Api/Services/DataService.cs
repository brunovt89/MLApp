using Dapper;
using Microsoft.Data.SqlClient;
using MLApp.Api.DTOs;

namespace MLApp.Api.Services
{
    public interface IDataService
    {
        Task<TrabajadorDto?> LoginAsync(int numeroDocumento, DateTime? fechaNacimiento);
        Task<TrabajadorDto?> ObtenerTrabajadorPorDniAsync(int numeroDocumento);
        Task<IEnumerable<TurnoDto>> ObtenerTurnosPorDniAsync(int numeroDocumento, bool soloFuturos = false);
        Task<IEnumerable<SiniestroDto>> ObtenerSiniestrosPorDniAsync(int numeroDocumento);
        Task<IEnumerable<FormularioSiniestroDto>> ObtenerFormulariosPorSiniestroAsync(int siniestro);
        Task<IEnumerable<ExamenDto>> ObtenerExamenesPorDniAsync(int numeroDocumento);
    }

    public class DataService : IDataService
    {
        private readonly string _connectionString;

        public DataService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlFederacionTest") 
                ?? throw new InvalidOperationException("Cadena de conexión SqlFederacionTest no configurada.");
        }

        public async Task<TrabajadorDto?> LoginAsync(int numeroDocumento, DateTime? fechaNacimiento)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT TOP 1
                    t.Trabajador AS IdTrabajador,
                    RTRIM(t.Apellido) AS Apellido,
                    RTRIM(t.Nombres) AS Nombres,
                    RTRIM(t.TipoDocumento) AS TipoDocumento,
                    t.NumeroDocumento,
                    RTRIM(t.CuilCuit) AS CuilCuit,
                    t.FechaNacimiento,
                    RTRIM(t.TelefonoCelular) AS TelefonoCelular,
                    RTRIM(t.EMail) AS EMail,
                    RTRIM(t.Localidad) AS Localidad,
                    RTRIM(t.Provincia) AS Provincia,
                    RTRIM(e.RazonSocial) AS EmpleadorRazonSocial
                FROM Trabajadores t WITH (NOLOCK)
                LEFT JOIN Empleadores e WITH (NOLOCK) ON t.Empleador = e.Empleador
                WHERE t.NumeroDocumento = @NumeroDocumento
                  AND (t.Baja IS NULL OR t.Baja = ' ' OR t.Baja = '0')";

            var trabajador = await connection.QueryFirstOrDefaultAsync<TrabajadorDto>(sql, new { NumeroDocumento = numeroDocumento });
            if (trabajador == null) return null;

            if (fechaNacimiento.HasValue && trabajador.FechaNacimiento.HasValue)
            {
                if (trabajador.FechaNacimiento.Value.Date != fechaNacimiento.Value.Date)
                {
                    return null;
                }
            }

            return trabajador;
        }

        public async Task<TrabajadorDto?> ObtenerTrabajadorPorDniAsync(int numeroDocumento)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT TOP 1
                    t.Trabajador AS IdTrabajador,
                    RTRIM(t.Apellido) AS Apellido,
                    RTRIM(t.Nombres) AS Nombres,
                    RTRIM(t.TipoDocumento) AS TipoDocumento,
                    t.NumeroDocumento,
                    RTRIM(t.CuilCuit) AS CuilCuit,
                    t.FechaNacimiento,
                    RTRIM(t.TelefonoCelular) AS TelefonoCelular,
                    RTRIM(t.EMail) AS EMail,
                    RTRIM(t.Localidad) AS Localidad,
                    RTRIM(t.Provincia) AS Provincia,
                    RTRIM(e.RazonSocial) AS EmpleadorRazonSocial
                FROM Trabajadores t WITH (NOLOCK)
                LEFT JOIN Empleadores e WITH (NOLOCK) ON t.Empleador = e.Empleador
                WHERE t.NumeroDocumento = @NumeroDocumento
                  AND (t.Baja IS NULL OR t.Baja = ' ' OR t.Baja = '0')";

            return await connection.QueryFirstOrDefaultAsync<TrabajadorDto>(sql, new { NumeroDocumento = numeroDocumento });
        }

        public async Task<IEnumerable<TurnoDto>> ObtenerTurnosPorDniAsync(int numeroDocumento, bool soloFuturos = false)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    ts.IdTurnoSiniestro,
                    ts.FechaHora,
                    RTRIM(ts.Concurrio) AS Concurrio,
                    ts.Anulado,
                    ts.Telemedicina,
                    RTRIM(ts.Observaciones) AS Observaciones,
                    s.Siniestro AS IdSiniestro,
                    RTRIM(s.Estado) AS EstadoSiniestro,
                    s.FechaHoraAccidente,
                    RTRIM(p.RazonSocial) AS PrestadorNombre,
                    RTRIM(p.CalleLegal) + ' ' + ISNULL(CAST(p.NumeroLegal AS VARCHAR), '') AS PrestadorDireccion,
                    RTRIM(p.LocalidadLegal) AS PrestadorLocalidad,
                    RTRIM(p.Telefonos) AS PrestadorTelefono,
                    RTRIM(pr.Nombre) AS ProfesionalNombre,
                    RTRIM(pr.Matricula) AS ProfesionalMatricula,
                    RTRIM(e.Nombre) AS EspecialidadNombre
                FROM TurnosSiniestro ts WITH (NOLOCK)
                INNER JOIN Siniestros s WITH (NOLOCK) ON ts.IdSiniestro = s.Siniestro
                INNER JOIN Trabajadores t WITH (NOLOCK) ON s.Trabajador = t.Trabajador
                LEFT JOIN Prestadores p WITH (NOLOCK) ON ts.IdPrestador = p.Prestador
                LEFT JOIN Profesionales pr WITH (NOLOCK) ON ts.IdProfesional = pr.Profesional
                LEFT JOIN Especialidades e WITH (NOLOCK) ON ts.IdEspecialidad = e.Especialidad
                WHERE t.NumeroDocumento = @NumeroDocumento
                  AND (t.Baja IS NULL OR t.Baja = ' ' OR t.Baja = '0')
                  AND ts.Anulado = 0";

            if (soloFuturos)
            {
                sql += " AND ts.FechaHora >= GETDATE()";
            }

            sql += " ORDER BY ts.FechaHora DESC";

            return await connection.QueryAsync<TurnoDto>(sql, new { NumeroDocumento = numeroDocumento });
        }

        public async Task<IEnumerable<SiniestroDto>> ObtenerSiniestrosPorDniAsync(int numeroDocumento)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    s.Siniestro,
                    s.FechaHoraAccidente,
                    RTRIM(s.Estado) AS Estado,
                    s.FechaAlta,
                    s.FechaPrimeraCuracion,
                    s.NumeroDenuncia,
                    s.FechaDenuncia,
                    RTRIM(s.DiagnosticoOms) AS DiagnosticoOms,
                    RTRIM(s.TipoSucesoArt) AS TipoSucesoArt,
                    s.Presiniestro,
                    RTRIM(p.RazonSocial) AS PrestadorNombre,
                    RTRIM(a.RazonSocial) AS ArtNombre
                FROM Siniestros s WITH (NOLOCK)
                INNER JOIN Trabajadores t WITH (NOLOCK) ON s.Trabajador = t.Trabajador
                LEFT JOIN Prestadores p WITH (NOLOCK) ON s.Prestador = p.Prestador
                LEFT JOIN Arts a WITH (NOLOCK) ON s.Art = a.Art
                WHERE t.NumeroDocumento = @NumeroDocumento
                  AND (t.Baja IS NULL OR t.Baja = ' ' OR t.Baja = '0')
                ORDER BY s.FechaHoraAccidente DESC";

            return await connection.QueryAsync<SiniestroDto>(sql, new { NumeroDocumento = numeroDocumento });
        }

        public async Task<IEnumerable<FormularioSiniestroDto>> ObtenerFormulariosPorSiniestroAsync(int siniestro)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    fs.Siniestro,
                    fs.NroFormulario,
                    RTRIM(fs.Formulario) AS Formulario,
                    fs.FechaHoraFormulario,
                    RTRIM(pr.Nombre) AS ProfesionalNombre,
                    RTRIM(e.Nombre) AS EspecialidadNombre,
                    fs.Auditado,
                    COALESCE(RTRIM(fpa.Diagnostico), RTRIM(iet.Diagnostico), RTRIM(iami.NaturalezaLesion)) AS Diagnostico,
                    COALESCE(RTRIM(fpa.DetalleAutorizacion), RTRIM(iet.Observaciones), RTRIM(iami.ComoOcurrioAccidente)) AS Observaciones,
                    COALESCE(RTRIM(fpa.PlanTerapeutico), RTRIM(iet.Motivo), RTRIM(iami.FormaAccidente)) AS DetalleAdicional
                FROM FormulariosSiniestro fs WITH (NOLOCK)
                LEFT JOIN Profesionales pr WITH (NOLOCK) ON fs.Profesional = pr.Profesional
                LEFT JOIN Especialidades e WITH (NOLOCK) ON fs.Especialidad = e.Especialidad
                LEFT JOIN Fpa fpa WITH (NOLOCK) ON fs.Siniestro = fpa.Siniestro AND fs.NroFormulario = fpa.NroFormulario
                LEFT JOIN Iet iet WITH (NOLOCK) ON fs.Siniestro = iet.Siniestro AND fs.NroFormulario = iet.NroFormulario
                LEFT JOIN Iami iami WITH (NOLOCK) ON fs.Siniestro = iami.Siniestro AND fs.NroFormulario = iami.NroFormulario
                WHERE fs.Siniestro = @Siniestro
                ORDER BY fs.FechaHoraFormulario DESC";

            return await connection.QueryAsync<FormularioSiniestroDto>(sql, new { Siniestro = siniestro });
        }

        public async Task<IEnumerable<ExamenDto>> ObtenerExamenesPorDniAsync(int numeroDocumento)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT TOP 30
                    er.ExamenRealizado AS IdExamenRealizado,
                    er.FechaExamen,
                    CAST(er.ResultadoExamen AS VARCHAR(MAX)) AS ResultadoExamen,
                    CAST(er.Observaciones AS VARCHAR(MAX)) AS Observaciones,
                    er.Autorizado,
                    er.FechaAutorizacion,
                    RTRIM(em.RazonSocial) AS Empleador,
                    RTRIM(pr.Nombre) AS Profesional
                FROM ExamenesRealizados er WITH (NOLOCK)
                INNER JOIN Trabajadores t WITH (NOLOCK) ON er.Trabajador = t.Trabajador
                LEFT JOIN Empleadores em WITH (NOLOCK) ON er.Empleador = em.Empleador
                LEFT JOIN Profesionales pr WITH (NOLOCK) ON er.Profesional = pr.Profesional
                WHERE t.NumeroDocumento = @NumeroDocumento
                  AND (t.Baja IS NULL OR t.Baja = ' ' OR t.Baja = '0')
                ORDER BY er.FechaExamen DESC";

            return await connection.QueryAsync<ExamenDto>(sql, new { NumeroDocumento = numeroDocumento });
        }
    }
}