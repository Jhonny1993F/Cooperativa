using System.ComponentModel.DataAnnotations;

namespace Cooperativa.Models
{
    public class Clientes
    {
        [Key]
        [Required]
        public int clienteID { get; set; }
        [Required]
        public String? nombres { get; set; }
        [Required]
        public String? apellidos { get; set; }
        [Required]
        public String? cedula { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime fechaNacimiento { get; set; }
        [Required]
        public String? direccion { get; set; }
        [Required]
        public String? telefono { get; set; }
        [Required]
        public String? cliente { get; set; }
        [Required]
        public decimal inscripcion { get; set; }
        [Required]
        public String? correo { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public String? contraseña { get; set; }
    }
}
