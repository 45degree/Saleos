using System.Collections.Generic;

namespace Saleos.DAO
{
    public class UserDAO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}
