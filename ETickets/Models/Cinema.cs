using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models
{
    public class Cinema
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }
        [Required]

        public string Description { get; set; }

        [ValidateNever]
        public string CinemaLogo { get; set; }
        [Required]

        public string Address { get; set; }
        [ValidateNever]

        public List<Movie> Movies { get;  }=new List<Movie>();

    }
}