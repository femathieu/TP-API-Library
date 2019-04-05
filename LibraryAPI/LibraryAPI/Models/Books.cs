using System;
using System.Collections.Generic;

namespace LibraryAPI.Models
{
    public partial class Books
    {
        public Books()
        {
            UsersBooks = new HashSet<UsersBooks>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        public ICollection<UsersBooks> UsersBooks { get; set; }
    }
}
