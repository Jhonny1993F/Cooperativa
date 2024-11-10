using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Bancos
    {
        [Key]
        public int BancoID { get; set; }
        public String? nombre { get; set; }
        public double interesBanco {  get; set; }
        public double cantidad { get; set; }
        public double comparacion { get; set; }

        [ForeignKey("creditoID")]
        public Creditos? creditos { get; set; }
        public int creditoID { get; set; }
        public double interes { get; set; }
    }
}
