using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Entity.IdentityServicesTest
{
    public class BaseIdentityServicesTest
    {
        protected DbContextOptions<IdentityDbContext> ContextOptions { get; }
        protected MockData _mockData = MockData.GetInstance();

        protected BaseIdentityServicesTest(DbContextOptions<IdentityDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using var context = new IdentityDbContext(ContextOptions);
            MockData.SeedData(identityContext: context);
        }
    }
}
