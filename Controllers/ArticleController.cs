using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

      private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions {
         WriteIndented = true,
         Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
      };
      //[HttpGet]
      //public async Task<IEnumerable<Article>> Get() =>await _context.Article.ToListAsync();

      [Authorize]
      [HttpDelete("Delete")]
      public async Task<IActionResult> DeleteArticle(Guid articleId) {
         Article article = _context.Article.FirstOrDefault((x) => x.id == articleId);
         
         if(article == null)
            return NotFound("Article not found");

         _context.Article.Remove(article);
         await _context.SaveChangesAsync();

         return Ok("Article deleted");
      }

      [Authorize]
      [HttpPost("Update")]
      public async Task<IActionResult> UpdateArticle(Article article) {
         Article dbArticle = _context.Article.FirstOrDefault((x) => x.id == article.id);

         if(dbArticle == null)
            return NotFound("Article not found");

         _context.Article.Update(dbArticle);
         await _context.SaveChangesAsync();

         return Ok("Article Updated");
      }

      [Authorize]
      [HttpPost("AddNewAttribute")]
      public async Task<IActionResult> AddAttributeToArticle(Guid articleId, Guid attributeId, string attributeValue) {
         Article article = _context.Article.FirstOrDefault((x) => x.id == articleId);
         AttributeDefinition attributeDefinition = _context.AttributeDefinitions.FirstOrDefault(x => x.id == attributeId);

         if(article == null || attributeDefinition == null)
            return NotFound();

         var articleAttributes = JsonSerializer.Deserialize < List<ArticleAttribute>>(article.attributesJson);
         var attributeExists = articleAttributes.FirstOrDefault((x) => x.definition.id == attributeId) != null;
         if(attributeExists) {
            return BadRequest(string.Format("Attribute {0} already added on the article!", attributeId));
         }

         var json = JsonSerializer.Serialize(new ArticleAttribute() {
            Id = Guid.NewGuid(),
            definition = attributeDefinition,
            value = attributeValue
         });

         if(string.IsNullOrEmpty(article.attributesJson)) {
            article.attributesJson = "[" + json +"]";
         } else {
            article.attributesJson = article.attributesJson.Insert(article.attributesJson.Length - 1, ","+json);
         }

         _context.Article.Update(article);
         await _context.SaveChangesAsync();
         return Ok("oke je");
      }

      [Authorize]
      [HttpGet("GetAttributes")]
      public async Task<IActionResult> GetArticleAttributes(Guid articleId) {
         Article article = _context.Article.FirstOrDefault((x) => x.id == articleId);

         if(article == null)
            return NotFound("Not found");

         List<ArticleAttribute> attributes = JsonSerializer.Deserialize(article.attributesJson,typeof(List<ArticleAttribute>)) as List<ArticleAttribute>;

         return Ok(JsonSerializer.Serialize(attributes));
      }



      [Authorize]
      [HttpGet("FilterByName")]
      public async Task<IActionResult> FilterByName(string name) {
         var articles = _context.Article.Where((x) => x.name == name);
         
         if(articles.Count() == 0)
            return NotFound();

         return Ok(JsonSerializer.Serialize(articles.ToList(), serializeOptions));
      }

      [Authorize]
      [HttpGet("FilterByPrice")]
      public async Task<IActionResult> FilterByprice(float price) {
         var articles = _context.Article.Where((x) => x.price == price);

         if(articles.Count() == 0)
            return NotFound();

         return Ok(JsonSerializer.Serialize(articles.ToList(), serializeOptions));
      }

      [Authorize]
      [HttpGet("FilterByMeasurementUnit")]
      public async Task<IActionResult> FilterByMeasurmentUnit(string unit) {
         var articles = _context.Article.Where((x) => x.measurementUnit == unit);

         if(articles.Count() == 0)
            return NotFound();

         return Ok(JsonSerializer.Serialize(articles.ToList(), serializeOptions));
      }

      [Authorize]
      [HttpGet("FilterByAttribute")]
      public async Task<IActionResult> FilterByAttribute(Guid attributeDefinitionId, string value) {
         var articles = _context.Article.AsEnumerable().Where(x => {
            var attributes = JsonSerializer.Deserialize<List<ArticleAttribute>>(x.attributesJson);
            var attribute = attributes.FirstOrDefault((x) => x.definition.id == attributeDefinitionId);

            if(attribute == null) 
               return false;

            return attribute.value == value;
         });

         if(articles.Count() == 0)
            return NotFound();



         return Ok(JsonSerializer.Serialize(articles.ToList(),serializeOptions));
      }

      [Authorize]
      [HttpPost("Create")]
      public async Task<IActionResult> Create(string name, string measurementUnit, float price) {
         Article article = new Article() {
            id = Guid.NewGuid(),
            name = name,
            measurementUnit = measurementUnit,
            price = price,
            attributesJson = ""
         };

         await _context.Article.AddAsync(article);
         await _context.SaveChangesAsync();

         var identity = HttpContext.User.Identity as ClaimsIdentity;

         if(identity != null) {
            var claims = identity.Claims;
            if(claims != null) {
               var id = claims.FirstOrDefault((x) => x.Type == ClaimTypes.NameIdentifier)?.Value;
               var user = _context.User.FirstOrDefault((x) => x.id == Guid.Parse(id));
               if(string.IsNullOrEmpty(user.articleIdsJson)) {
                  user.articleIdsJson = JsonSerializer.Serialize(new List<Guid> { article.id });
               }else {
                  var articleIds = JsonSerializer.Deserialize<List<Guid>>(user.articleIdsJson);
                  articleIds.Add(article.id);
                  user.articleIdsJson = JsonSerializer.Serialize(articleIds);
               }

               _context.User.Update(user);
               await _context.SaveChangesAsync();
            }
            return BadRequest("Timedout");
         }
         return Ok("Article added");
      }

   }
}
