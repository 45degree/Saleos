using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Test.Controller
{
    public abstract class BaseControllerTest<T> where T:DbContext
    {
        protected DbContextOptions<T> _dbContextOptions;
        protected IDbContextFactory<T> _dbContextFactory;

        public BaseControllerTest(DbContextOptions<T> dbContextOptions,
            IDbContextFactory<T> dbContextFactory)
        {
            _dbContextOptions = dbContextOptions;
            _dbContextFactory = dbContextFactory;
            dbContextFactory.SeedDate(_dbContextOptions);
        }

        protected T getContext() => _dbContextFactory.CreateDbContext(_dbContextOptions);
    }

    public interface IDbContextFactory<T> where T: DbContext
    {
        T CreateDbContext(DbContextOptions<T> options);

        void SeedDate(DbContextOptions<T> options);
    }

    public class HomePageDbContextFactory : IDbContextFactory<HomePageDbContext>
    {
        public HomePageDbContext CreateDbContext(DbContextOptions<HomePageDbContext> options)
        {
            return new HomePageDbContext(options);
        }

        public void SeedDate(DbContextOptions<HomePageDbContext> options)
        {
            using var context = new HomePageDbContext(options);
            MockData.SeedData(context);
        }
    }
}
