namespace MLApp.Api.DTOs
{
    public class LoginRequestDto
    {
        public int NumeroDocumento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }

    public class TrabajadorDto
    {
        public int IdTrabajador { get; set; }
        public string? Apellido { get; set; }
        public string? Nombres { get; set; }
        public string? TipoDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public string? CuilCuit { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? TelefonoCelular { get; set; }
        public string? EMail { get; set; }
        public string? Localidad { get; set; }
        public string? Provincia { get; set; }
        public string? EmpleadorRazonSocial { get; set; }
    }

    public class TurnoDto
    {
        public int IdTurnoSiniestro { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Concurrio { get; set; }
        public bool Anulado { get; set; }
        public bool? Telemedicina { get; set; }
        public string? Observaciones { get; set; }
        public int IdSiniestro { get; set; }
        public string? EstadoSiniestro { get; set; }
        public DateTime? FechaHoraAccidente { get; set; }
        public string? PrestadorNombre { get; set; }
        public string? PrestadorDireccion { get; set; }
        public string? PrestadorLocalidad { get; set; }
        public string? PrestadorTelefono { get; set; }
        public string? ProfesionalNombre { get; set; }
        public string? ProfesionalMatricula { get; set; }
        public string? EspecialidadNombre { get; set; }
    }

    public class SiniestroDto
    {
        public int Siniestro { get; set; }
        public DateTime FechaHoraAccidente { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaPrimeraCuracion { get; set; }
        public int? NumeroDenuncia { get; set; }
        public DateTime? FechaDenuncia { get; set; }
        public string? DiagnosticoOms { get; set; }
        public string? TipoSucesoArt { get; set; }
        public bool? Presiniestro { get; set; }
        public string? PrestadorNombre { get; set; }
        public string? ArtNombre { get; set; }
    }

    public class FormularioSiniestroDto
    {
        public int Siniestro { get; set; }
        public int NroFormulario { get; set; }
        public string? Formulario { get; set; } // Iami, Iet, Fpa, Fpt, Am
        public DateTime? FechaHoraFormulario { get; set; }
        public string? ProfesionalNombre { get; set; }
        public string? EspecialidadNombre { get; set; }
        public bool? Auditado { get; set; }
        
        // Detalles específicos según tipo de formulario
        public string? Diagnostico { get; set; }
        public string? Observaciones { get; set; }
        public string? DetalleAdicional { get; set; }
    }

    public class EstudioExamenDto
    {
        public int ExamenRealizado { get; set; }
        public int Secuencia { get; set; }
        public string? NombreEstudio { get; set; }
        public string? Observaciones { get; set; }
        public string? ResultadoEstudio { get; set; }
        public DateTime? FechaRealizacion { get; set; }
    }

    public class ExamenDto
    {
        public int IdExamenRealizado { get; set; }
        public DateTime? FechaExamen { get; set; }
        public string? ResultadoExamen { get; set; }
        public string? Observaciones { get; set; }
        public bool? Autorizado { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
        public string? Empleador { get; set; }
        public string? Profesional { get; set; }
        public List<EstudioExamenDto> Estudios { get; set; } = new();
    }
}