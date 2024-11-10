using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Transferencias
    {
        [Key]
        public int transferenciaID { get; set; }
        public double cantidadTransferencia { get; set; }
        public DateTime fechaTransferencia { get; set; }
        public String? detalleTransferencia { get; set; }

        [ForeignKey("clienteID")]
        public Clientes? clientes { get; set; }
        public int clienteID { get; set; }
        public String? cliente { get; set; }
    }
}
