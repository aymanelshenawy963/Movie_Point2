
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace ETickets.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            this.cartRepository = cartRepository;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var ApplicationUserId = userManager.GetUserId(User);

            var cartProduct = cartRepository.GetAll([e => e.Movie] ,e => e.ApplicationUserId == ApplicationUserId).ToList();

            ViewBag.Total = cartProduct.Sum(e => e.Movie.Price * e.Count);

            return View(cartProduct.ToList());
        }
        public IActionResult AddToCart(int count, int movieId)
        {
            Cart cart = new Cart()
            {
                Count = count,
                MovieId = movieId,
                ApplicationUserId = userManager.GetUserId(User)
            };
            cartRepository.Add(cart);
            cartRepository.Commit();
            TempData["success"] = "Add Product To Cart success";
            return RedirectToAction("Index", "Home");
        }

               public IActionResult Increment(int movieId)
        {
            var ApplicationUserId = userManager.GetUserId(User);
            var movie = cartRepository.GetOne(null, e =>e.ApplicationUserId == ApplicationUserId && e.MovieId== movieId);

            if (movie != null)
            {
                movie.Count++;
                cartRepository.Commit();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundPage","Home");
        }
        public IActionResult Decrement(int movieId)
        {
            var ApplicationUserId = userManager.GetUserId(User);
            var movie = cartRepository.GetOne(null, e => e.ApplicationUserId == ApplicationUserId && e.MovieId == movieId);

            if (movie != null)
            {
                movie.Count--;
                if (movie.Count > 0)
                    cartRepository.Commit();
                else
                    movie.Count = 1;

                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundPage", "Home");
        }

        public IActionResult Delete(int movieId)
        {
            var ApplicationUserId = userManager.GetUserId(User);

            var movie = cartRepository.GetOne(null, e => e.ApplicationUserId == ApplicationUserId && e.MovieId == movieId);

            if (movie != null)
            {
                cartRepository.Delete(movie);
                cartRepository.Commit();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundPage", "Home");
        }


        public IActionResult Pay()
        {
            var ApplicationUserId = userManager.GetUserId(User);

            var cartProduct = cartRepository.GetAll([e => e.Movie], e => e.ApplicationUserId == ApplicationUserId).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            foreach (var item in cartProduct)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                        },
                        UnitAmount = (long)item.Movie.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }





    }
}
