using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Controller
{
    public class HomePageControllerTest : BaseControllerTest<HomePageDbContext>
    {
        protected MockData _mockData = MockData.GetInstance();
        public HomePageControllerTest(string connectionString)
            : base(new DbContextOptionsBuilder<HomePageDbContext>()
                .UseInMemoryDatabase(connectionString).Options,
                new HomePageDbContextFactory())
        {
        }
    }
}
