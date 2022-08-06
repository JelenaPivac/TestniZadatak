using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestniZadatak.Data;
using TestniZadatak.Models;

namespace TestniZadatak.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class LoginController: ControllerBase
   {
      private readonly ApplicationDBContext _context;
      private readonly IConfiguration _configuration;

      public LoginController(ApplicationDBContext context,IConfiguration configuration) {
         _context = context;
         _configuration = configuration;
      }

      [AllowAnonymous]
      [HttpPost]
      public IActionResult Login(UserLogin userLogin) {
         var user = _context.User.FirstOrDefault((x) => x.email == userLogin.email && x.password == userLogin.password);
         if(user != null) {
            var token = GenerateToken(user);
            return Ok(token);
         }
         return NotFound("User not found!");
      }

      [HttpDelete]
      public IActionResult Logout() {
         return Ok("Logged out");

      }


      private string GenerateToken(User user) {
         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

         var claims = new[] {
            new Claim(ClaimTypes.GivenName,user.firstName),
            new Claim(ClaimTypes.Surname,user.lastName),
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.MobilePhone, user.phoneNumber),
            new Claim("password",user.password),
            new Claim("attributes",user.definedAttributes),
            new Claim("articles", user.articleIdsJson),   
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
         };

         var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

         return new JwtSecurityTokenHandler().WriteToken(token);
      }
   }
}
