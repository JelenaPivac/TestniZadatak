using Microsoft.EntityFrameworkCore;
using TestniZadatak.Models;

namespace TestniZadatak.Data
{
   public class ApplicationDBContext: DbContext
   {
      public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) {

      }

      public DbSet<User> User { get; set; }
      public DbSet<Article> Article { get; set; }

      public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }

      public DbSet<LoginValidation> TokenValidation { get; set; }
   }
}
