using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Entity.Test
{
    public class MemoryEntityTest : EntityTest
    {
        public MemoryEntityTest() : base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseInMemoryDatabase("Mock Database")
            .Options)
        {
        }
    }

    public class MemoryArticleRepositoryTest : ArticleRepositoryTest
    {
        public MemoryArticleRepositoryTest() : base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseInMemoryDatabase("Mock ArticleRepository-routine")
            .Options)
        {
        }
    }
}