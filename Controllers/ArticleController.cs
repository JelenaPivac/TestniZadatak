using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TestniZadatak.Data;
using TestniZadatak.Models;

namespace TestniZadatak.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class ArticleController: ControllerBase
   {
      private readonly ApplicationDBContext _context;

      public ArticleController(ApplicationDBContext context) {
         _context = context;
      }

      //[HttpGet]
      //public async Task<IEnumerable<Article>> Get() =>await _context.Article.ToListAsync();


      [HttpPost("AddNewAttribute")]
      public async Task<IActionResult> AddAttributeToArticle(Guid articleId, Guid attributeId, string attributeValue) {
         Article article = _context.Article.FirstOrDefault((x) => x.id == articleId);
         AttributeDefinition attributeDefinition = _context.AttributeDefinitions.FirstOrDefault(x => x.id == attributeId);

         if(article == null || attributeDefinition == null)
            return NotFound();

         var json = JsonSerializer.Serialize(new ArticleAttribute() {
            Id = Guid.NewGuid(),
            name = attributeDefinition,
            value = attributeValue
         });

         if(string.IsNullOrEmpty(article.attributes)) {
            article.attributes = "[" + json +"]";
         } else {
            article.attributes = article.attributes.Insert(article.attributes.Length - 1, ","+json);
         }

         _context.Article.Update(article);
         await _context.SaveChangesAsync();
         return Ok("oke je");
      }

      [HttpGet("GetAttributes")]
      public async Task<IActionResult> GetArticleAttributes(Guid articleId) {
         Article article = _context.Article.FirstOrDefault((x) => x.id == articleId);

         if(article == null)
            return NotFound("Not found");

         List<ArticleAttribute> attributes = JsonSerializer.Deserialize(article.attributes,typeof(List<ArticleAttribute>)) as List<ArticleAttribute>;

         return Ok(JsonSerializer.Serialize(attributes));
      }

      [HttpPost("Create")]
      public async Task<IActionResult> Create(string name,string measurementUnit, float price) {
         Article article = new Article() {
            id = Guid.NewGuid(),
            name = name,
            measurementUnit = measurementUnit,
            price = price,
            attributes = ""
         };

         await _context.Article.AddAsync(article);
         await _context.SaveChangesAsync();

         return Ok("Article added");
      }

   }
}
