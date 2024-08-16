using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WordSearch.Models;

namespace WordSearch.Controllers
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
            int nbInvalidGrid   = 0;
            int nbValidGrid     = 0;

            for (int i = 0; i < 100; i++)
            {
                Grid grid = new Grid(5);
                grid.Generate();

                if (grid.IsValid)
                {
                    nbValidGrid++;
                }
                else
                {
                    nbInvalidGrid++;
                }
            }

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
