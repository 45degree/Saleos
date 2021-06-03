using Microsoft.EntityFrameworkCore;

namespace Saleos.Entity.Data
{
    public class HomePageDbContext: DbContext
    {
        public HomePageDbContext(DbContextOptions<HomePageDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Article { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>().HasKey(x => new {x.ArticleId, x.TagId});
        } 
    }
}