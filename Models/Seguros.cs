using Microsoft.AspNetCore.Http.Timeouts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Seguros
    {
        [Key]
        public int seguroID { get; set; }
        public decimal valor { get; set; }
        public DateTime fechaSeguro { get; set; }
        public String? Tipo { get; set; }
        public int tiempo { get; set; }
        public String? descripcion { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int socioID { get; set; }
        public String? socio { get; set; }
        public decimal inscripcion { get; set; }
    }
}
