using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using static Book_Store.Repositories.StockRepository;

namespace Book_Store.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockByBookId(int bookId) => await _context.Stocks.FirstOrDefaultAsync(s => s.BookId == bookId);

        public async Task ManageStock(StockDTO stockToManage)
        {
            // If there is no stock for given book id, then add new record
            // If there is already stock for given book id, update stock's quantity
            var existingStock = await GetStockByBookId(stockToManage.BookId);
            if (existingStock is null)
            {
                var stock = new Stock { BookId = stockToManage.BookId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from book in _context.Books
                                join stock in _context.Stocks
                                on book.Id equals stock.BookId
                                into book_stock
                                from bookStok in book_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) ||
                                book.BookName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    BookId = book.Id,
                                    BookName = book.BookName,
                                    Quantity = bookStok == null ? 0 : bookStok.Quantity
                                }).ToListAsync();
            return stocks;
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocksForReport()
        {
            return await (from book in _context.Books
                          join stock in _context.Stocks
                          on book.Id equals stock.BookId
                          into book_stock
                          from bookStok in book_stock.DefaultIfEmpty()
                          select new StockDisplayModel
                          {
                              BookId = book.Id,
                              BookName = book.BookName,
                              Quantity = bookStok == null ? 0 : bookStok.Quantity
                          }).ToListAsync();
        }

    }
}