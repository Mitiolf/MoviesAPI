using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Filmy.Models
{
    public class UserCollectionModel
    {

        //Relation to Movies
        public virtual MovieModel Movie { get; set; }
        [JsonIgnore]
        public int MovieId { get; set; }


        // Relation to user
        [JsonIgnore]
        public virtual UserModel User { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }


        [Range(0, 5)]
        public int Rate { get; set; }

    }
}
