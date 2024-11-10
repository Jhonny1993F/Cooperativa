using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Ahorros
    {
        [Key]
        public int ahorroID { get; set; }
        public double montoAhorro { get; set; }
        public String? comprobante { get; set; }
        public DateTime fechaAhorro { get; set; }
        public String? detalleAhorro { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int socioID { get; set; }
        public String? socio { get; set; }
    }
}
