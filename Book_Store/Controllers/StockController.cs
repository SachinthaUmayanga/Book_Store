using crypto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Book_Store.Repositories.StockRepository;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Signatures;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Digests;

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

        
        public async Task<IActionResult> GenerateReport()
        {
            try
            {
                var stocks = await _stoctRepo.GetStocksForReport();

                if (stocks == null || !stocks.Any())
                {
                    TempData["errorMessage"] = "No data available to generate the report.";
                    return RedirectToAction(nameof(Index));
                }

                var stream = new MemoryStream();
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Title
                document.Add(new Paragraph("Stock Report")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20));

                // Date
                document.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                    .SetFontSize(10));

                document.Add(new Paragraph("\n")); // Add space

                // Table
                var table = new Table(3);
                table.AddHeaderCell("Book ID");
                table.AddHeaderCell("Book Name");
                table.AddHeaderCell("Quantity");

                foreach (var stock in stocks)
                {
                    table.AddCell(stock.BookId.ToString());
                    table.AddCell(stock.BookName);
                    table.AddCell(stock.Quantity.ToString());
                }

                document.Add(table);
                document.Close();

                stream.Position = 0; // Reset position before returning the file
                return File(stream.ToArray(), "application/pdf", "StockReport.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateReport: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["errorMessage"] = "Failed to generate report. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
