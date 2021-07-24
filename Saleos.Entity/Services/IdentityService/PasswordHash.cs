using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Saleos.Entity.Services.IdentityService
{
    public class PasswordHash : IPasswordHash
    {
        public async Task<string> GetPasswordHashAsync(string password, string salt = null)
        {
            // add salt to the password
            if(salt != null)
            {
                password += salt;
            }

            // caculate the sha1 of the password
            var sha1 = SHA1.Create();
            var originalPwd = Encoding.UTF8.GetBytes(password);

            using var memoryStream = new MemoryStream(originalPwd);
            var encryPwd = await sha1.ComputeHashAsync(memoryStream);
            return string.Join("", encryPwd.Select(b => string.Format("{0:x2}", b))
                .ToArray()).ToUpper();
        }
    }
}
