using DotNetCore5.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5.ViewModel
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        
        [Range(0, 10)]
        public double Rate { get; set; }

        [Required, MaxLength(2500)]
        public string Storeline { get; set; }
        
        [Display(Name = "Select Poster")]
        public byte[] Poster { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        public IEnumerable<Genre> Genres { get; set; }

    }
}
