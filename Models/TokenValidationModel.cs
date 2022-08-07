using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestniZadatak.Models
{
   public class LoginValidation
   {
      [Key]
      [Column(TypeName = "nvarchar(max)")]
      public string token { get; set; }
      public bool isValid { get; set; }
   }
}
