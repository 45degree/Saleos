using System.Threading.Tasks;

namespace Saleos.Entity.Services.IdentityService
{
    public interface IPasswordHash
    {
        public Task<string> GetPasswordHashAsync(string password, string salt = null);
    }
}
