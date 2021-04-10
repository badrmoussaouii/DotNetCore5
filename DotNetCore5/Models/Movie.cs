using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        
        public double Rate { get; set; }

        [Required (ErrorMessage = "Remplissage obligatoire"), MaxLength(2500, ErrorMessage = "Vous avez dépasser le nombre maximum de caractère")]
        public string Storeline { get; set; }

        [Required(ErrorMessage = "Remplissage obligatoire")]
        public byte[] Poster { get; set; }

        public int GenreId { get; set; }

        public Genre Genre { get; set; }
    }
}
