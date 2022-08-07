using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestniZadatak.Models
{
   public class ArticleAttribute
   {
      [Key]
      public Guid Id { get; set; }
      public Guid? definitionId { get; set; }
      [ForeignKey("definitionId")]
      public virtual AttributeDefinition definition { get; set; }
      public string value { get; set; }
      [ForeignKey("Article")]
      public Guid articleId { get; set; }
   }
}
