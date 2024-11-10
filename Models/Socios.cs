using System.ComponentModel.DataAnnotations;

namespace Cooperativa.Models
{
    public class Socios
    {
        [Key]
        public int socioID { get; set; }
        public String? nombres { get; set; }
        public String? apellidos { get; set; }
        public String? cedula { get; set; }
        public DateOnly fechaNacimiento { get; set; }
        public String? direccion { get; set; }
        public String? telefono { get; set; }
        public String? socio { get; set; }
        public int inscripcion { get; set; }
        public String? correo { get; set; }
        public String? contraseña { get; set; }
    }
}
