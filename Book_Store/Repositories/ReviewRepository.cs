using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Fetches all reviews for a specific book.
    /// </summary>
    /// <param name="bookId">The ID of the book.</param>
    /// <returns>A list of reviews for the specified user and book.</returns>
    public async Task<IEnumerable<Review>> GetReviewsByUserAndBookAsync(int bookId)
    {
        return await _context.Reviews
            .Where(r => r.BookId == bookId)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new review to the database.
    /// </summary>
    /// <param name="request">The details of the review to add.</param>
    /// <returns>The added review.</returns>
    public async Task<Review> AddReviewAsync(CreateReviewDTO request)
    {

        Review review = new()
        {
            Title = request.Title,
            Content = request.Content,
            UserId = request.UserId,
            BookId = request.BookId
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }
}

