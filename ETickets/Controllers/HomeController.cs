using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Diagnostics;
using System.Linq;

namespace ETickets.Controllers
{
    public class HomeController : Controller
    {
        //ApplicationDbContext dbcontext = new ApplicationDbContext();
        private readonly IActorMovieRepository actorMovieRepository;
        private readonly IMovieRepository movieRepository;

    
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,IActorMovieRepository actorMovieRepository, IMovieRepository movieRepository)
        {
            _logger = logger;
            this.actorMovieRepository = actorMovieRepository;
            this.movieRepository = movieRepository;
        }

        public IActionResult Index()
        {
            var movies = movieRepository.GetAll([e => e.Category, e => e.Cinema]).ToList();

            //var movies = dbcontext.Movies
            //    .Include(e => e.Category)
            //    .Include(e => e.Cinema).Include(e => e.ActorMovies)
            //    .ToList();
            return View(movies);
        }



        public IActionResult Details(int movieId)
        {
            ViewBag.actors = actorMovieRepository.GetAll([e=>e.Actor],e => e.MovieId == movieId).Select(e => e.Actor).ToList();
            //ViewBag.actors = dbcontext.ActorMovies
            //.Where(e => e.MovieId == movieId)
            //.Select(e => e.Actor)
            //.ToList();

            var movie = movieRepository.GetOne([e => e.Cinema, e => e.Category], e => e.Id == movieId);
            //var movie = dbcontext.Movies
            //    .Include(e => e.Cinema)
            //    .Include(e => e.Category)
            //    .FirstOrDefault(e => e.Id == movieId);

            if (movie == null)
            {
                return NotFound(); // Handle case when the movie is not found
            }

            return View(model: movie);
        }




        public IActionResult Search(string search)
        {

            var movie = movieRepository.GetAll([e => e.Cinema, e => e.Category], e => e.Name.Contains(search)).ToList();
            //var movie = dbcontext.Movies
            //    .Include(e => e.Cinema)
            //    .Include(e => e.Category)
            //    .Where(e => e.Name.Contains(search)).ToList();
            if (movie != null)
            {
                return View(movie);

            }
            return View("NotFoundPage");

        }

        public IActionResult NotFoundPage()
        {
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


