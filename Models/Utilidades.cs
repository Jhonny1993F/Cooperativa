using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Utilidades
    {
        [Key]
        public int utilidadID { get; set; }
        public decimal utilidadTotal { get; set; }
        public decimal utilidadPorSocio { get; set; }
        public DateTime fechaUtilidad { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int socioID { get; set; }
        public String? socio { get; set; }
        public decimal inscripcion { get; set; }

        [ForeignKey("creditoID")]
        public Creditos? creditos { get; set; }
        public int creditoID { get; set; }
        public decimal interes { get; set; }
        public decimal totalCredito { get; set; }

        [ForeignKey("eventoID")]
        public Eventos? eventos { get; set; }
        public int eventoID { get; set; }
        public decimal costoEvento { get; set;}

        [ForeignKey("ahorroID")]
        public Ahorros? ahorros { get; set; }
        public int ahorroID { get; set; }
        public decimal montoAhorro { get;set; }

        [ForeignKey("pasivoID")]
        public Pasivos? pasivos { get; set; }
        public int pasivoID { get; set; }
        public decimal costoPasivo { get;set; }
    }
}
