using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Retiros
    {
        [Key]
        public int retiroID { get; set; }
        public double cantidadRetiro { get; set; }
        public DateTime fechaRetiro { get; set; }
        public String? detalleRetiro { get; set; }

        [ForeignKey("clienteID")]
        public Clientes? clientes { get; set; }
        public int clienteID { get; set; }
        public String? cliente { get; set; }
    }
}
