using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Entity.Test
{
    public class SqliteEntityTest : EntityTest
    {
        public SqliteEntityTest()
            : base(new DbContextOptionsBuilder<HomePageDbContext>()
                .UseSqlite("Data Source=Entity-routine.db")
                .Options)
        {
        }
    }

    public class SqliteArticleRepositoryTest : ArticleRepositoryTest
    {
        public SqliteArticleRepositoryTest() : base(new DbContextOptionsBuilder<HomePageDbContext>()
            .UseSqlite("Data Source=ArticleRepository-routine.db")
            .Options)
        {
        }
    }
}