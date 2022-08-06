using System.ComponentModel.DataAnnotations;

namespace TestniZadatak.Models
{
   public class LoginValidation
   {
      [Key]
      public string token { get; set; }
      public bool isValid { get; set; }
   }
}
