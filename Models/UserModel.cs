using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmy.Models
{
    public class UserModel
    { 
        public Guid Id { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

        //Relaton 
        public virtual ICollection<UserCollectionModel> UserMovies { get; set; } = new List<UserCollectionModel>();


    }
}
