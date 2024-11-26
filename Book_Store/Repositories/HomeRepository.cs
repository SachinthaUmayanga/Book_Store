using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Book_Store.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0, string conditon = "")
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Book> books = await (from book in _db.Books
                         join genre in _db.Genres
                         on book.GenreId equals genre.Id
                         join stock in _db.Stocks
                         on book.Id equals stock.BookId
                         into book_stocks
                         from bookWithStock in book_stocks.DefaultIfEmpty()
                         where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.BookName.ToLower().StartsWith(sTerm))
                         && (string.IsNullOrEmpty(conditon) || book.BookCondition == conditon)
                         select new Book
                         {
                             Id = book.Id,
                             Image = book.Image,
                             AuthorName = book.AuthorName,
                             BookName = book.BookName,
                             GenreId = book.GenreId,
                             Price = book.Price,
                             GenreName = genre.GenreName,
                             BookCondition = book.BookCondition,
                             Quantity = bookWithStock==null ? 0 : bookWithStock.Quantity
                         }
                         ).ToListAsync();

            if (genreId > 0)
            {
                books = books.Where(a => a.GenreId == genreId).ToList();
            }
            return books;
        }

        public async Task<Book> GetBookById(int bookId)
        {
            var book = await (from b in _db.Books
                              join genre in _db.Genres on b.GenreId equals genre.Id
                              join stock in _db.Stocks on b.Id equals stock.BookId into book_stocks
                              from bookWithStock in book_stocks.DefaultIfEmpty()
                              where b.Id == bookId
                              select new Book
                              {
                                  Id = b.Id,
                                  Image = b.Image,
                                  AuthorName = b.AuthorName,
                                  BookName = b.BookName,
                                  GenreId = b.GenreId,
                                  Price = b.Price,
                                  GenreName = genre.GenreName,
                                  BookCondition = b.BookCondition,
                                  Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity
                              }).FirstOrDefaultAsync();

            return book;
        }
    }
}
