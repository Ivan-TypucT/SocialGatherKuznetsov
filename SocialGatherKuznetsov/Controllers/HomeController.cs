using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialGatherKuznetsov.Models;
using System.Diagnostics;
using SocialGatherKuznetsov.Models;

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
            return View();
        }

        public IActionResult Chats()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }


        public IActionResult Create()
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