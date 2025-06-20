using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Ahorros
    {
        [Key]
        public int ahorroID { get; set; }
        [Required]
        public decimal montoAhorro { get; set; }
        //[Required(ErrorMessage = "El comprobante es obligatorio.")]
        public String? comprobante { get; set; }
        [Required]
        public DateTime fechaAhorro { get; set; }
        [Required]
        public String? detalleAhorro { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int? socioID { get; set; }
        public String? socio { get; set; }

        [ForeignKey("clienteID")]
        public Clientes? clientes { get; set; }
        public int? clienteID { get; set; }
        public String? cliente { get; set; }

        /*// Propiedad calculada para convertir el byte[] del comprobante a Base64
        [NotMapped]  // Esto indica que no debe ser mapeada a la base de datos
        public string? ComprobanteBase64
        {
            get
            {
                if (comprobante != null)
                {
                    return Convert.ToBase64String(comprobante);
                }
                return null;
            }
        }*/
    }
}

