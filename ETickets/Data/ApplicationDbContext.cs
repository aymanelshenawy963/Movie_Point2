using ETickets.Models;
using Microsoft.EntityFrameworkCore;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ETickets.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Actor> Actors {  get; set; }
        public DbSet<Category> Categories {  get; set; }
        public DbSet<Cinema> Cinemas {  get; set; }
        public DbSet<Movie> Movies {  get; set; }
        public DbSet<ActorMovie> ActorMovies {  get; set; }
        public DbSet<ETickets.ViewModels.ApplicationUserVM> ApplicationUserVM { get; set; } = default!;
        public DbSet<ETickets.ViewModels.LoginVM> LoginVM { get; set; } = default!;


   
   
    }
}
