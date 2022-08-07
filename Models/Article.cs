using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TestniZadatak.Models
{
   public class Article
   {
      [Key]
      public Guid id { get; set; }
      public string name { get; set; }
      public string measurementUnit { get; set; }
      public float price { get; set; }

      public virtual ICollection<ArticleAttribute> attributes { get; set; }

      [ForeignKey("User")]
      public Guid userId { get; set; }
   }
}
