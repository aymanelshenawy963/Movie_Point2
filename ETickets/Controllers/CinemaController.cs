using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository cinemaRepository;
        private readonly IMovieRepository movieRepository;

        public CinemaController(ICinemaRepository cinemaRepository, IMovieRepository movieRepository)
        {
            this.cinemaRepository = cinemaRepository;
            this.movieRepository = movieRepository;
        }

        public IActionResult Index(int page =1, string? search = null)
        {
            ViewBag.cinema = cinemaRepository.GetAll().Count() / 5;


            if (ViewBag.cinema % 5 == 0)
            {
                ViewBag.cinema = (int)cinemaRepository.GetAll().Count() / 5;
            }
            else
            {
                ViewBag.cinema = ((int)cinemaRepository.GetAll().Count() / 5) + 1;
            }
            ViewBag.CurrentPage = page;
            if (page <= 0)
                page = 1;

            var cinema = cinemaRepository.GetAll();

            if (search != null)
            {
                search.TrimStart();
                search.TrimEnd();
                cinema = cinema.Where(e => e.Name.Contains(search) );

            }
            cinema = cinema.Skip((page - 1) * 5).Take(5);
            if (cinema.Any())
                return View(cinema);
            return RedirectToAction("NotFoundPage", "Home");


   
        }





        public IActionResult Create()
        {
            Cinema cinema = new Cinema();
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cinema cinema, IFormFile CinemaLogo)
        {
            ModelState.Remove("CinemaLogo");
            if (ModelState.IsValid)
            {
                if (CinemaLogo != null && CinemaLogo.Length > 0)//99656
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinemas", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        CinemaLogo.CopyTo(stream);
                    }
                    cinema.CinemaLogo = fileName;
                }
                cinemaRepository.Add(cinema);
                cinemaRepository.Commit();

                return RedirectToAction(nameof(Index));

            }
            return View(cinema);
        }

        public IActionResult Details(String cinemaName)
        {
            var movies = movieRepository.GetAll([e => e.Category, e => e.Cinema], e => e.Cinema.Name == cinemaName);
              
            return View(movies);
        }




        public IActionResult Edit(int cinemaId)
        {
            var cinema = cinemaRepository.GetOne(null, x => x.Id == cinemaId);
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile CinemaLogo)
        {
            var oldFile = cinemaRepository.GetOne(null, x => x.Id == cinema.Id);
            ModelState.Remove("CinemaLogo");
            if (ModelState.IsValid)
            {

                if (CinemaLogo != null && CinemaLogo.Length > 0)//99656
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Cinemas", fileName);
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Cinemas", oldFile.CinemaLogo);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        CinemaLogo.CopyTo(stream);
                    }

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    cinema.CinemaLogo = fileName;
                }

                else
                {
                    cinema.CinemaLogo = oldFile.CinemaLogo;
                }
                cinemaRepository.Edit(cinema);
                cinemaRepository.Commit();
                return RedirectToAction(nameof(Index));

            }
            return View(cinema);

        }

        public IActionResult Delete(int CinemaId)
        {
            Cinema cinema = new Cinema() { Id = CinemaId };
            cinemaRepository.Delete(cinema);
            cinemaRepository.Commit();
            return RedirectToAction(nameof(Index));
        }

    }
}
