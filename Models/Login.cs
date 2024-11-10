using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cooperativa.Models
{
    public class Login
    {
        [Key]
        public int loginID { get; set; }

        //[Required]
        public String? socio { get; set; }

        //[Required]
        public String? cliente { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String? contraseña { get; set; }
    }
}
