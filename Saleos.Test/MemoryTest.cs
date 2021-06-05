using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;
using Saleos.Test.Entity.Test;

namespace Saleos.Test
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

    public class MemoryArticleInfoRepositoryTest : ArticleInfoRepositoryTest
    {
        public MemoryArticleInfoRepositoryTest(): base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseInMemoryDatabase("Mock ArticleInfoRepository-routine")
            .Options)
        {
            
        }
    }

    public class MemoryTagInfoRepositoryTest : TagRepositoryTest
    {
        public MemoryTagInfoRepositoryTest(): base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseInMemoryDatabase("Mock TagRepository-routine")
            .Options)
        {
        }
    }
}