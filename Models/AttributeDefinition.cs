using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestniZadatak.Models
{
   public class AttributeDefinition
   {
      [Key]
      public Guid id { get; set; }
      public string name { get; set; }

      [ForeignKey("User")]
      public Guid userId { get; set; }


   }
}
