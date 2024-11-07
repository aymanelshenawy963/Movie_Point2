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
        public DbSet<Cart> Carts {  get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Cart>()
        //   .HasKey(c => new { c.MovieId, c.ApplicationUserId });

        //    modelBuilder.Entity<Cart>()
        //     .HasOne(c => c.Movie)
        //     .WithMany()
        //     .HasForeignKey(c => c.MovieId)
        //     .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<Cart>()
        //    .HasOne(c => c.ApplicationUser)
        //    .WithMany()
        //    .HasForeignKey(c => c.ApplicationUserId)
        //    .OnDelete(DeleteBehavior.Cascade);
        //}



        }
}
