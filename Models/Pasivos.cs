using System.ComponentModel.DataAnnotations;

namespace Cooperativa.Models
{
    public class Pasivos
    {
        [Key]
        public int pasivoID { get; set; }
        public String? tipo { get; set; }
        public double costoPasivo { get; set; }
        public String? detalle { get; set; }
        public DateTime fechaPasivo { get; set; }
    }
}
