using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    /// <summary>
    /// Fetches all reviews for a specific book.
    /// </summary>
    /// <param name="bookId">The ID of the book.</param>
    /// <returns>A list of reviews for the specified user and book.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews(int bookId)
    {
        var reviews = await _reviewRepository.GetReviewsByUserAndBookAsync(bookId);
        return Ok(reviews);
    }

    /// <summary>
    /// Adds a new review.
    /// </summary>
    /// <param name="review">The review to add.</param>
    /// <returns>The added review.</returns>
    [HttpPost]
    public async Task<ActionResult<Review>> AddReview([FromBody] CreateReviewDTO payload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var addedReview = await _reviewRepository.AddReviewAsync(payload);
        return CreatedAtAction(nameof(GetReviews), new { userId = addedReview.UserId, bookId = addedReview.BookId }, addedReview);
    }
}
