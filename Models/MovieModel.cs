using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Filmy.Models
{
    public class MovieModel
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }


        // Relatin to usermovies
        [JsonIgnore]
        public virtual ICollection<UserCollectionModel> UserMovies { get; set; } = new List<UserCollectionModel>();
    }
}
