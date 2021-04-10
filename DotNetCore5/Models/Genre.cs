using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity )]
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }
    }
}
