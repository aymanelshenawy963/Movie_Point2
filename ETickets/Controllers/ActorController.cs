using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Controllers
{
    public class ActorController : Controller
    {
        private readonly IActorRepository actorRepository;

        public ActorController(IActorRepository actorRepository)
        {
            this.actorRepository = actorRepository;
        }
        public IActionResult Index(int page=1,string? search=null)
        {
            ViewBag.actor = actorRepository.GetAll().Count() / 7;


            if (ViewBag.actor % 7 == 0)
            {
                ViewBag.actor = (int)actorRepository.GetAll().Count() / 7;
            }
            else
            {
                ViewBag.actor = ((int)actorRepository.GetAll().Count() / 7)+1;
            }
            ViewBag.CurrentPage = page;
            if (page<=0)
                page=1;

            var actors = actorRepository.GetAll();

            if (search != null)
            {
                search.TrimStart();
                search.TrimEnd();
                actors = actors.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search));

            }
             actors = actors.Skip((page - 1) * 7).Take(7);

            if(actors.Any())
            return View(actors);
            return RedirectToAction("NotFoundPage","Home");
        }
 

        public IActionResult Create()
        {
            Actor actor = new Actor();
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Actor actor, IFormFile ProfilePicture)
        {
            ModelState.Remove("ProfilePicture");
            if (ModelState.IsValid)
            {
                if (ProfilePicture!=null && ProfilePicture.Length > 0)//99656
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ProfilePicture.CopyTo(stream);
                    }
                    actor.ProfilePicture = fileName;
                }
                actorRepository.Add(actor);
                actorRepository.Commit();

                return RedirectToAction(nameof(Index));

            }
            return View(actor);
        }

        public IActionResult Edit(int actorId)
        {
            var actor = actorRepository.GetOne(null, x => x.Id == actorId);
            return View(actor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Actor actor, IFormFile ProfilePicture)
        {
            var oldFile = actorRepository.GetOne(null, x => x.Id == actor.Id);

            if (ModelState.IsValid)
            {

                if (ProfilePicture != null && ProfilePicture.Length > 0)//99656
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", oldFile.ProfilePicture);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ProfilePicture.CopyTo(stream);
                    }

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    actor.ProfilePicture = fileName;
                }

                else
                {
                    actor.ProfilePicture = oldFile.ProfilePicture;
                }
                actorRepository.Edit(actor);
                actorRepository.Commit();
                return RedirectToAction(nameof(Index));

            }
            return View(actor);

        }

        public IActionResult Delete(int actorId)
        {
            Actor actor = new Actor() { Id = actorId };
            actorRepository.Delete(actor);
            actorRepository.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
