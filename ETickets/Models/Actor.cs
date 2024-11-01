using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models
{
    public class Actor
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Bio { get; set; }
        [ValidateNever]
        public string ProfilePicture { get; set; }
        [Required]
        public string News { get; set; }
        [ValidateNever]
        public ICollection<ActorMovie> ActorMovies { get; }=new List<ActorMovie>();

    }
}