using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Creditos
    {
        [Key]
        public int creditoID { get; set; }
        public double montoCredito { get; set; }
        public DateTime fechaCredito { get; set; }
        public int tiempo { get; set; }
        public double interes { get; set; }
        public double cuota { get; set; }
        public String? estado { get; set; }
        public double totalCredito { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int socioID { get; set; }
        public String? socio { get; set; }
    }
}
