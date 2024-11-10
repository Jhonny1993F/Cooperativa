using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Depositos
    {
        [Key]
        public int depositoID { get; set; }
        public double cantidadDeposito { get; set; }
        public DateTime fechaDeposito { get; set; }
        public string? detalleDeposito { get; set;}

        [ForeignKey("clienteID")]
        public Clientes? clientes { get; set; }
        public int clienteID { get; set; }
        public String? cliente { get; set;}

    }
}
