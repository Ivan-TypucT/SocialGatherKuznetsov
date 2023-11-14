using Microsoft.AspNetCore.Mvc;
using SocialGatherKuznetsov.Models;
using System.Diagnostics;

namespace SocialGatherKuznetsov.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /*
            var model = ;
            // Получение токена из cookie
            string cookieToken = Request.Cookies["token"]?.Value;

            if (cookieToken != model.Token)
            {
                // Токены не совпадают, возвращаем код ошибки 401
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            // Токены совпадают, выполнение дальнейших действий

            // Возвращаем ваш представление с моделью User*/
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