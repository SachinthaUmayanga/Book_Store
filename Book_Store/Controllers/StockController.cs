using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Book_Store.Repositories.StockRepository;

namespace Book_Store.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stoctRepo;

        public StockController(IStockRepository stoctRepo)
        {
            _stoctRepo = stoctRepo;
        }

        public async Task<IActionResult> Index(string sTerm="")
        {
            var stocks = await _stoctRepo.GetStocks(sTerm);
            return View(stocks);
        }

        public async Task<IActionResult> ManageStock(int bookId)
        {
            var existingStock = await _stoctRepo.GetStockByBookId(bookId);
            var stock = new StockDTO
            {
                BookId = bookId,
                Quantity = existingStock != null ? existingStock.Quantity : 0

            };
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> Managestock(StockDTO stock)
        {
            if (!ModelState.IsValid)
                return View(stock);

            try
            {
                await _stoctRepo.ManageStock(stock);
                TempData["successMessage"] = "Stock is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
