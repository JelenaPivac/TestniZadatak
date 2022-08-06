using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TestniZadatak.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>((x)=>x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
   options.Events = new JwtBearerEvents() {
      OnTokenValidated = context => {
         var dbcontext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDBContext>();
         var _bearer_token = context.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

         var tokenValidation = dbcontext.TokenValidation.FirstOrDefault((x) => x.token == _bearer_token);
         if(tokenValidation != null) {
            if(tokenValidation.isValid)
               return Task.CompletedTask;
            else {
               context.Fail("Invaild token");
               return Task.CompletedTask;
            }
         }
         context.Fail("Invalid Token");
         return Task.CompletedTask;
      }
   };
   
   options.TokenValidationParameters = new TokenValidationParameters() {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["Jwt:Issuer"],
      ValidAudience = builder.Configuration["Jwt:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
   };
});

builder.Services.AddSwaggerGen(c => {
   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
      In = ParameterLocation.Header,
      Description = "Please insert JWT with Bearer into field",
      Name = "Authorization",
      Type = SecuritySchemeType.ApiKey
   });
   c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
