using System.Collections.Generic;
using System.Threading.Tasks;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetReviewsByUserAndBookAsync(int bookId);
    Task<Review> AddReviewAsync(CreateReviewDTO request);
}

