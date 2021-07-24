using System.Threading.Tasks;
using Saleos.Entity.Services.IdentityService;
using Xunit;

namespace Saleos.Test.Entity.IdentityServicesTest
{
    public class PasswordHashTest
    {
        [Fact]
        public async Task GetPasswordHash_EncryptThePassword()
        {
            // the sha1 result is calculated by https://1024tools.com/hash
            var passwordHash = new PasswordHash();
            var encryptPassword = await passwordHash.GetPasswordHashAsync("12345");
            Assert.Equal("8CB2237D0679CA88DB6464EAC60DA96345513964", encryptPassword);

            encryptPassword = await passwordHash.GetPasswordHashAsync("1234", "asfh");
            Assert.Equal("2AC25407725E41012FBD0A0176D655EB08DABDFA", encryptPassword);
        }
    }
}
