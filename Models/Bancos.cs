using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Bancos
    {
        [Key]
        public int BancoID { get; set; }
        [Required]
        public String? nombre { get; set; }
        [Required]
        public decimal interesBanco {  get; set; }
        [Required]
        public decimal cantidad { get; set; }
        public decimal comparacion { get; set; }

        [ForeignKey("creditoID")]
        public Creditos? creditos { get; set; }
        public int creditoID { get; set; }
        [Required]
        public decimal interes { get; set; }
    }
}
