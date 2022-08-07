using Microsoft.EntityFrameworkCore;
using TestniZadatak.Models;

namespace TestniZadatak.Data
{
   public class ApplicationDBContext: DbContext
   {
      public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) {

      }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
         base.OnConfiguring(optionsBuilder);
         optionsBuilder.UseLazyLoadingProxies(true);
      }
      protected override void OnModelCreating(ModelBuilder modelBuilder) {
         base.OnModelCreating(modelBuilder);
         foreach(var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
         }
      }
      public DbSet<User> User { get; set; }
      public DbSet<Article> Article { get; set; }

      public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }

      public DbSet<LoginValidation> TokenValidation { get; set; }

      public DbSet<ArticleAttribute> Attributes { get; set; }
   }
}
