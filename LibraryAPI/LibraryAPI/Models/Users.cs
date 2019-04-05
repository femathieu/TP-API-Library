using System;
using System.Collections.Generic;

namespace LibraryAPI.Models
{
    public partial class Users
    {
        public Users()
        {
            UsersBooks = new HashSet<UsersBooks>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public ICollection<UsersBooks> UsersBooks { get; set; }
    }
}
