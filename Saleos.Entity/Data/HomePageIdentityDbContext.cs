using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Saleos.Entity.Data
{
    public class HomePageIdentityDbContext : IdentityDbContext
    {
        public HomePageIdentityDbContext(DbContextOptions<HomePageIdentityDbContext> options)
            : base(options)
        {
        }
    }
}