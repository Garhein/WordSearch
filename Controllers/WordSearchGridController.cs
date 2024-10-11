using Microsoft.AspNetCore.Mvc;
using WordSearch.Helpers;
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
            SearchWordModel model = new SearchWordModel(RandomHelper.GenerateRandomValue(Grid.MIN_WORD_LENGTH, Grid.MAX_WORD_LENGTH));
            model.Grid.Generate();

            return View("Index", model);
        }
    }
}
