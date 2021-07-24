using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DAO;
using Saleos.Entity.Data;
using Saleos.Entity.Services.IdentityService;
using Xunit;

namespace Saleos.Test.Entity.IdentityServicesTest
{
    public abstract class IdentityServerTest : BaseIdentityServicesTest
    {
        public IdentityServerTest(DbContextOptions<IdentityDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        [Fact]
        public async Task CreateCustomerTest()
        {
            using var context = new IdentityDbContext(ContextOptions);

            var passwordHash = new PasswordHash();
            var identityServer = new IdentityServiceImpl(context, passwordHash);

            await identityServer.CreateCustomerAsync("TestUser", "1234", "13");

            Assert.True(await context.Users.AnyAsync(x => x.Username == "TestUser"));

            var user = await context.Users.Where(x => x.Username == "TestUser")
                .Include(x => x.Roles).SingleOrDefaultAsync();
            Assert.Single(user.Roles);
            Assert.Equal("Customer", user.Roles[0].RoleName);
        }

        [Fact]
        public async Task LoginTest()
        {
            using var context = new IdentityDbContext(ContextOptions);

            var passwordHash = new PasswordHash();
            var identityServer = new IdentityServiceImpl(context, passwordHash);

            var loginDAO = new LoginDAO
            {
                Username = _mockData.Users[0].Username,
                Password = "123"
            };

            // No Exception Throw
            await identityServer.Login(loginDAO);

            var user = await context.Users.Where(x => x.Username == loginDAO.Username)
                .Include(x => x.Roles).SingleOrDefaultAsync();
            Assert.True(user.IsLogin);
        }

        [Fact]
        public async Task LogoutTest()
        {
            using var context = new IdentityDbContext(ContextOptions);

            var passwordHash = new PasswordHash();
            var identityServer = new IdentityServiceImpl(context, passwordHash);

            // No Exception Throw
            await identityServer.Logout(_mockData.Users[0].Username);
            var user = await context.Users.Where(x => x.Username == _mockData.Users[0].Username)
                .Include(x => x.Roles).SingleOrDefaultAsync();
            Assert.False(user.IsLogin);
        }
    }
}
