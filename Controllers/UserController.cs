using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestniZadatak.Data;
using TestniZadatak.Models;

namespace TestniZadatak.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UserController: ControllerBase
   {
      private readonly ApplicationDBContext _context;

      public UserController(ApplicationDBContext context) {
         _context = context;
      }

      //[HttpGet("id")]
      //[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
      //[ProducesResponseType(StatusCodes.Status404NotFound)]
      //public async Task<IActionResult> GetById(Guid id) {
      //   var user=await _context.User.FindAsync(id);
      //   return user == null ? NotFound() : Ok();
      //}

      private User GetCurrentUser() {
         var identity = HttpContext.User.Identity as ClaimsIdentity;

         if(identity != null) {
            var claims = identity.Claims;

            var id = Guid.Parse(claims.FirstOrDefault((x) => x.Type == "id")?.Value);

            return _context.User.FirstOrDefault((x) => x.id == id);
         }

         return null;
      }

      [Authorize]
      [HttpPost("AddAttributeDefinition")]
      public async Task<IActionResult> AddNewAttirbute(string name) {
         var currentUserId = GetCurrentUser().id;
         if(_context.AttributeDefinitions.FirstOrDefault((x)=>x.name == name && x.userId == currentUserId) != null) {
            return BadRequest("An Attribute with the name already exists");
         }


         var attribute = new AttributeDefinition() {
            name = name,
            id = Guid.NewGuid()
         };
         await _context.AttributeDefinitions.AddAsync(attribute);
         var currentUser = GetCurrentUser();

         currentUser.attributeDefinitions.Add(attribute);

         _context.User.Update(currentUser);
         await _context.SaveChangesAsync();

         return Ok("");
      }

      [HttpPost("Create")]
      public async Task<IActionResult> Create(string firstName, string lastName, string email, string phone, string password, string confirmPassword) {
         var userExists = _context.User.Where((x) => x.email == email).Count() != 0;
         if(userExists) {
            return BadRequest("Email already in use");
         }
         if(password != confirmPassword) {
            return BadRequest("Passwords not matching!");
         }

         User newUser = new User() {
            id = Guid.NewGuid(),
            firstName = firstName,
            lastName = lastName,
            email = email,
            phoneNumber = phone,
            password = password,
         };
         await _context.User.AddAsync(newUser);
         await _context.SaveChangesAsync();
         return Ok("New user created!");
      }
   }
}
