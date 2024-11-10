using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Eventos
    {
        [Key]
        public int eventoID { get; set; }
        public DateTime fechaEvento { get; set; }
        public String? tipoEvento { get; set; }
        public double costoEvento { get; set; }
        public String? detalleEvento { get; set; }
        public String? lugar { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int socioID { get; set; }
        public String? socio { get; set; }
    }
}
