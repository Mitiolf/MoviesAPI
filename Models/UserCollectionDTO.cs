using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Filmy.Models
{
    public class UserCollectionDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [Range(0, 5)]
        public int Rate { get; set; }

    }
}
