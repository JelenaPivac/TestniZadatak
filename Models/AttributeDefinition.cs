using System.ComponentModel.DataAnnotations;

namespace TestniZadatak.Models
{
   public class AttributeDefinition
   {
      [Key]
      public Guid id { get; set; }
      public string name { get; set; }
   }
}
