using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class PasivosClientes
    {
        [Key]
        public int pasivoClienteID { get; set; }
        public String? tipo { get; set; }
        public double costoPasivo { get; set; }
        public String? detallePasivo { get; set; }
        public DateTime fechaPasivo { get; set; }

        [ForeignKey("clienteID")]
        public Clientes? clientes { get; set; }
        public int clienteID { get; set; }
        public String? cliente { get; set; }
    }
}
