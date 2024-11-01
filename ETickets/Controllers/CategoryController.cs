using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Controllers
{
    public class CategoryController : Controller
    {
        //ApplicationDbContext dbcontext = new();
        private readonly ICategoryRepository categoryRepository;
        private readonly IMovieRepository movieRepository;

        public CategoryController(ICategoryRepository categoryRepository, IMovieRepository movieRepository)
        {
            this.categoryRepository = categoryRepository;
            this.movieRepository = movieRepository;
        }

        public IActionResult Index()
        {
            var categories = categoryRepository.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Details(String categoryName)
        {
            var movies = movieRepository.GetAll([e => e.Category, e => e.Cinema, e => e.ActorMovies], e => e.Category.Name == categoryName);
         
            return View(movies);
        }



        public IActionResult Create()
        {
            var category = new Category();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {

            if (ModelState.IsValid)
            {
                categoryRepository.Add(category);
                movieRepository.Commit();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }



        public IActionResult Edit(int categoryId)
        {
            var category = categoryRepository.GetOne(null,e => e.Id == categoryId);
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                //Category category = new Category() { Name = categoryName };
                categoryRepository.Edit(category);
                categoryRepository.Commit();
                return RedirectToAction(nameof(Index));
            }
            return View(category);

        }


        public IActionResult Delete(int categoryId)
        {
            Category category = new Category() { Id = categoryId };
            categoryRepository.Delete(category);
            categoryRepository.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
