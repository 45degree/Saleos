using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DAO;
using Saleos.Entity.DAOExtension;
using Saleos.Entity.Data;

namespace Saleos.Entity.Services.IdentityService
{
    public class IdentityServiceImpl : IIdentityService
    {
        private IdentityDbContext _identityDbContext;
        private IPasswordHash _passwordHash;

        public IdentityServiceImpl(IdentityDbContext context, IPasswordHash passwordHash)
        {
            _identityDbContext = context;
            _passwordHash = passwordHash;
        }

        public async Task CreateCustomerAsync(string username, string password, string salt)
        {
            if(!await _identityDbContext.Roles.AnyAsync(x => x.RoleName == "Customer"))
            {
                await _identityDbContext.Roles.AddAsync(new Role {RoleName = "Customer"});
            }
            var roles = await _identityDbContext.Roles.Where(x => x.RoleName == "Customer")
                .ToListAsync();


            var encryPwd = await _passwordHash.GetPasswordHashAsync(password, salt);
            var user = new User
            {
                Username = username,
                PasswordSha1 = encryPwd,
                Salt = salt,
                Roles = roles,
                CreateTime = DateTime.Now
            };

            await _identityDbContext.Users.AddAsync(user);
            await _identityDbContext.SaveChangesAsync();
        }

        public async Task<bool> IsLogin(string username)
        {
            if(!await _identityDbContext.Users.AnyAsync(x => x.Username == username))
            {
                throw new Exception("don't have this user");
            }

            var user = await _identityDbContext.Users.Where(x => x.Username == username)
                .SingleOrDefaultAsync();

            return user.IsLogin;
        }

        public async Task<UserDAO> Login(LoginDAO loginDAO)
        {
            if(!await _identityDbContext.Users.AnyAsync(x => x.Username == loginDAO.Username))
            {
                throw new Exception("don't have this user");
            }

            var user = await _identityDbContext.Users.Where(x => x.Username == loginDAO.Username)
                .Include(x => x.Roles)
                .SingleOrDefaultAsync();

            // get the encrypt password and compare it
            var salt = user.Salt;
            var passwordHash = new PasswordHash();
            var encryptPWD = await passwordHash.GetPasswordHashAsync(loginDAO.Password, salt);
            if(encryptPWD != user.PasswordSha1)
            {
                throw new Exception("password is wrong");
            }

            user.IsLogin = true;
            user.LastLoginTime = DateTime.Now;

            await _identityDbContext.SaveChangesAsync();

            return user.GetUserDAOFromUser();
        }

        public async Task Logout(string username)
        {
            if(!await _identityDbContext.Users.AnyAsync(x => x.Username == username))
            {
                throw new Exception("don't have this user");
            }

            var user = await _identityDbContext.Users.Where(x => x.Username == username)
                .SingleOrDefaultAsync();

            user.LastLogoutTime = DateTime.Now;
            await _identityDbContext.SaveChangesAsync();
        }
    }
}
