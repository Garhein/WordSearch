using Microsoft.AspNetCore.Mvc;
using WordSearch.Models;

namespace WordSearch.Controllers
{
    public class WordSearchGridController : Controller
    {
        private readonly ILogger<WordSearchGridController> _logger;

        public WordSearchGridController(ILogger<WordSearchGridController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            SearchWordModel model = new SearchWordModel();
            model.Grid = new Grid(5);
            model.Grid.Generate();

            return View("Index", model);
        }
    }
}
