using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Entity.Test
{
    public class BaseServicesTest
    {
        protected DbContextOptions<HomePageDbContext> ContextOptions { get; }

        protected BaseServicesTest(DbContextOptions<HomePageDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using var context = new HomePageDbContext(ContextOptions);
            EntityTest.SeedDate(context);
        }
    }
}