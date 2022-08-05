using System.ComponentModel.DataAnnotations;

namespace TestniZadatak.Models
{
   public class ArticleAttribute
   {
      [Key]
      public Guid Id { get; set; }
      public AttributeDefinition name { get; set; }
      public string value { get; set; }
   }
}
