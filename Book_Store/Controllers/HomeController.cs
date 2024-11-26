using Book_Store.Models;
using Book_Store.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Book_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0, string condition = "")
        {
            
            IEnumerable<Book> books = await _homeRepository.GetBooks(sterm, genreId, condition);
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            BookDisplayModel bookModel = new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId,
                BookCondition = condition
            };

            return View(bookModel);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("Home/BookDetails/{bookId}")]
        public async Task<IActionResult> BookDetails(int bookId)
        {
            var book = await _homeRepository.GetBookById(bookId);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
    }
}
