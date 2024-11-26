namespace Book_Store
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0, string conditon = "");
        Task<IEnumerable<Genre>> Genres();
        Task<Book> GetBookById(int bookId);
    }
}