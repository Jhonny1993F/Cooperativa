using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Utilidades
    {
        [Key]
        public int utilidadID { get; set; }
        public double utilidadTotal { get; set; }
        public double utilidadPorSocio { get; set; }
        public DateTime fechaUtilidad { get; set; }

        [ForeignKey("socioID")]
        public Socios? socios { get; set; }
        public int socioID { get; set; }
        public String? socio { get; set; }
        public int inscripcion { get; set; }

        [ForeignKey("creditoID")]
        public Creditos? creditos { get; set; }
        public int creditoID { get; set; }
        public double interes { get; set; }
        public double totalCredito { get; set; }

        [ForeignKey("eventoID")]
        public Eventos? eventos { get; set; }
        public int eventoID { get; set; }
        public double costoEvento { get; set;}

        [ForeignKey("ahorroID")]
        public Ahorros? ahorros { get; set; }
        public int ahorroID { get; set; }
        public double montoAhorro { get;set; }

        [ForeignKey("pasivoID")]
        public Pasivos? pasivos { get; set; }
        public int pasivoID { get; set; }
        public double costoPasivo { get;set; }
    }
}
