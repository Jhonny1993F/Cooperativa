using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Creditos
    {
        [Key]
        public int creditoID { get; set; }
        [Required]
        public decimal montoCredito { get; set; }
        [Required]
        public DateTime fechaCredito { get; set; }
        [Required]
        public String? tipoCredito { get; set; }
        [Required]
        public int tiempo { get; set; }
        [Required]
        public decimal interes { get; set; }
        [Required]
        public decimal cuota { get; set; }
        [Required]
        public String? estado { get; set; }
        [Required]
        public decimal totalCredito { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int? socioID { get; set; }
        
        public String? socio { get; set; }

        [ForeignKey("clienteID")]
        public Clientes? clientes { get; set; }
        public int? clienteID { get; set; }
        
        public String? cliente { get; set; }
    }
}
