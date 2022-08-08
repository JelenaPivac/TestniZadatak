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

         var userId = GetUserId();

         if(userId != null) {
            if(userId.Value != article.userId) {
               return BadRequest("Article doesnt belong to the user logged in");
            }
         } else {
            return BadRequest("Couldn't find logged in user");
         }

         if(article == null)
            return NotFound("Article not found");
         foreach(var attribute in article.attributes) {
            _context.Attributes.Remove(attribute);
         }
         _context.Article.Remove(article);
         await _context.SaveChangesAsync();

         return Ok("Article deleted");
      }

      [Authorize]
      [HttpPost("Update")]
      public async Task<IActionResult> UpdateArticle(Article article) {
         Article dbArticle = _context.Article.FirstOrDefault((x) => x.id == article.id);

         var userId = GetUserId();

         if(userId != null) {
            if(userId.Value != article.userId) {
               return BadRequest("Article doesnt belong to the user logged in");
            }
         } else {
            return BadRequest("Couldn't find logged in user");
         }

         if(dbArticle == null)
            return NotFound("Article not found");

         dbArticle.price = article.price;
         dbArticle.attributes = article.attributes;
         dbArticle.measurementUnit = article.measurementUnit;
         dbArticle.name = article.name;

         _context.Article.Update(dbArticle);
         await _context.SaveChangesAsync();

         return Ok("Article Updated");
      }

      [Authorize]
      [HttpPost("AddNewAttribute")]
      public async Task<IActionResult> AddAttributeToArticle(Guid articleId, Guid definitionId, string attributeValue) {
         Article article = _context.Article.FirstOrDefault((x) => x.id == articleId);
         AttributeDefinition attributeDefinition = _context.AttributeDefinitions.FirstOrDefault(x => x.id == definitionId);

         if(article == null || attributeDefinition == null)
            return NotFound();


         var userId = GetUserId();

         if(userId != null) {
            if(userId.Value != article.userId || userId.Value != attributeDefinition.userId) {
               return BadRequest("Article or attribute definition doesnt belong to the user logged in");
            }
         } else {
            return BadRequest("Couldn't find logged in user");
         }

         var attributeExists = article.attributes.FirstOrDefault((x) => x.definition.id == definitionId) != null;
         if(attributeExists) {
            return BadRequest(string.Format("Attribute {0} already added on the article!", definitionId));
         }

         var attribute = new ArticleAttribute() {
            articleId = articleId,
            definition = attributeDefinition,
            Id = Guid.NewGuid(),
            value = attributeValue
         };

         await _context.Attributes.AddAsync(attribute);
         article.attributes.Add(attribute);

         _context.Article.Update(article);
         await _context.SaveChangesAsync();
         return Ok("Attribute added");
      }

      [Authorize]
      [HttpGet("GetAttributes")]
      public async Task<IActionResult> GetArticleAttributes(Guid articleId) {
         Article article = _context.Article.AsEnumerable().FirstOrDefault((x) => x.id == articleId);

         if(article == null)
            return NotFound("Not found");

         return Ok(JsonSerializer.Serialize(article.attributes));
      }



      [Authorize]
      [HttpGet("FilterByName")]
      public async Task<IActionResult> FilterByName(string name) {
         var userId = GetUserId();
         var articles = _context.Article.Where((x) => x.name == name && x.userId == userId);
         
         if(articles.Count() == 0)
            return NotFound();

         return Ok(JsonSerializer.Serialize(articles.ToList(), serializeOptions));
      }

      [Authorize]
      [HttpGet("FilterByPrice")]
      public async Task<IActionResult> FilterByprice(float price) {
         var userId = GetUserId();
         var articles = _context.Article.Where((x) => x.price == price && x.userId == userId);

         if(articles.Count() == 0)
            return NotFound();

         return Ok(JsonSerializer.Serialize(articles.ToList(), serializeOptions));
      }

      [Authorize]
      [HttpGet("FilterByMeasurementUnit")]
      public async Task<IActionResult> FilterByMeasurmentUnit(string unit) {
         var userId = GetUserId();
         var articles = _context.Article.Where((x) => x.measurementUnit == unit && x.userId == userId);

         if(articles.Count() == 0)
            return NotFound();

         return Ok(JsonSerializer.Serialize(articles.ToList(), serializeOptions));
      }

      [Authorize]
      [HttpGet("FilterByAttribute")]
      public async Task<IActionResult> FilterByAttribute(Guid attributeDefinitionId, string value) {
         var userId = GetUserId();
         var articles = _context.Article.ToList();
         var filteredArticles= articles.Where(x=>x.userId == userId).Where(x => {
             var attribute = x.attributes.FirstOrDefault((x) => x.definition.id == attributeDefinitionId);

             if(attribute == null)
                return false;

             return attribute.value == value;
          });
         if(filteredArticles == null || filteredArticles.Count() == 0)
            return NotFound();



         return Ok(JsonSerializer.Serialize(filteredArticles.ToList(),serializeOptions));
      }

      [Authorize]
      [HttpPost("Create")]
      public async Task<IActionResult> Create(string name, string measurementUnit, float price) {

         var identity = HttpContext.User.Identity as ClaimsIdentity;

         if(identity != null) {
            var claims = identity.Claims;
            if(claims != null) {
               var id = claims.FirstOrDefault((x) => x.Type == "id")?.Value;
               var user = _context.User.FirstOrDefault((x) => x.id == Guid.Parse(id));


               Article article = new Article() {
                  id = Guid.NewGuid(),
                  name = name,
                  measurementUnit = measurementUnit,
                  price = price,
                  userId = Guid.Parse(id)
               };

               await _context.Article.AddAsync(article);
               await _context.SaveChangesAsync();

               user.articles.Add(article);
               _context.User.Update(user);
               await _context.SaveChangesAsync();

               return Ok("Article added");
            }
            return BadRequest("Timedout");
         }
         return Ok("Article added");
      }

      private Guid? GetUserId() {

         var identity = HttpContext.User.Identity as ClaimsIdentity;

         if(identity != null) {
            var claims = identity.Claims;

            return Guid.Parse(claims.FirstOrDefault((x) => x.Type == "id")?.Value);

         }

         return null;
      }
   }

}
