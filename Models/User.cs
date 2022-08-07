using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestniZadatak.Models
{
   public class User {
      [Key]
      public Guid id { get; set; }
      public string firstName { get; set; }
      public string lastName { get; set; }
      public string email { get; set; }
      public string phoneNumber { get; set; }
      public string password { get; set; }

      public virtual ICollection<AttributeDefinition> attributeDefinitions { get; set; }
   
      public virtual ICollection<Article> articles { get; set; }
   }
}
