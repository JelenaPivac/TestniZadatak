using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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
      [HttpPost("Login")]
      public async Task<IActionResult> Login(UserLogin userLogin) {
         var user = _context.User.FirstOrDefault((x) => x.email == userLogin.email && x.password == userLogin.password);
         if(user != null) {
            var token = await GenerateToken(user);
            return Ok(token);
         }
         return NotFound("User not found!");
      }

      [Authorize]
      [HttpDelete("Logout")]
      public async Task<IActionResult> Logout() {
         var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

         if(_bearer_token != null) {
            var tokenValidation = _context.TokenValidation.FirstOrDefault((x) => x.token == _bearer_token);
            tokenValidation.isValid = false;

            _context.TokenValidation.Update(tokenValidation);
            await _context.SaveChangesAsync();
            return Ok("Logged out");
         }



         return BadRequest("Something went wrong");
      }


      private async Task<string> GenerateToken(User user) {
         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

         var claims = new[] {
            new Claim("id", user.id.ToString()),
         };

         var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);


         var tokenString =  new JwtSecurityTokenHandler().WriteToken(token);
         await _context.TokenValidation.AddAsync(new LoginValidation() {
            token = tokenString,
            isValid = true
         });
         await _context.SaveChangesAsync();

         return tokenString;
      }
   }
}
