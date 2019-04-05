using System;
using System.Collections.Generic;

namespace LibraryAPI.Models
{
    public partial class UsersBooks
    {
        public int Id { get; set; }
        public int UsersId { get; set; }
        public int BooksId { get; set; }
        public DateTime Date { get; set; }

        public Books Books { get; set; }
        public Users Users { get; set; }
    }
}
