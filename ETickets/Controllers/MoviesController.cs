using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieRepository movieRepository;
        private readonly ICinemaRepository cinemaRepository;
        private readonly ICategoryRepository categoryRepository;

        public MoviesController(IMovieRepository movieRepository,ICinemaRepository cinemaRepository,ICategoryRepository categoryRepository)
        {
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var movie = this.movieRepository.GetAll([e => e.ActorMovies, e => e.Category, e => e.Cinema]);
            return View(movie);
        }

        public IActionResult Create()
        {
            ViewBag.CinemaId = new SelectList(cinemaRepository.GetAll(),"Id", "Name");
            ViewBag.CategoryId = new SelectList(categoryRepository.GetAll(), "Id", "Name");
            ViewBag.MovieStatusList = new SelectList(Enum.GetValues(typeof(MovieStatus)));

            return View(new Movie());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, IFormFile ImgUrl)
        {
            ModelState.Remove("ImgUrl");
            if (ModelState.IsValid)
            {
                if (ImgUrl.Length > 0)//99656
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImgUrl.CopyTo(stream);
                    }
                    movie.ImgUrl = fileName;
                }
                movieRepository.Add(movie);
                movieRepository.Commit();
                //TempData["Success"] = "Add Product Successfully";

                return RedirectToAction(nameof(Index));


            }
            return View(movie);
        }


        public IActionResult Edit(int movieId)
        {
            ViewBag.CinemaId = new SelectList(cinemaRepository.GetAll(), "Id", "Name");
            ViewBag.CategoryId = new SelectList(categoryRepository.GetAll(), "Id", "Name");
            ViewBag.MovieStatusList = new SelectList(Enum.GetValues(typeof(MovieStatus)));
            var movie = movieRepository.GetOne(null, x => x.Id == movieId);
            return View(movie);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile ImgUrl)
        {
            var oldFile = movieRepository.GetOne(null, x => x.Id == movie.Id);
            ModelState.Remove("ImgUrl");

            if (ModelState.IsValid)
            {

                if (ImgUrl != null && ImgUrl.Length > 0)//99656
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", oldFile.ImgUrl);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImgUrl.CopyTo(stream);
                    }

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    movie.ImgUrl = fileName;
                }

                else
                {
                    movie.ImgUrl = movie.ImgUrl;
                }
                movieRepository.Edit(movie);
                movieRepository.Commit();
                return RedirectToAction(nameof(Index));

            }
            return View(movie);

        }

        public IActionResult Delete(int movieId)
        {
            Movie movie = new Movie() { Id=movieId};
            movieRepository.Delete(movie);
            movieRepository.Commit();
            return RedirectToAction(nameof(Index));

        }
    }
}
