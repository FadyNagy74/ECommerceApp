using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Repositories;

namespace E_CommerceApp.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;

        public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
        }

        public async Task<int> AddReview(string userId,string productId, ReviewDTO reviewDTO) {

            Product? product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("A product with this Id doesn't exist");

            if (reviewDTO.RateValue > 5 || reviewDTO.RateValue <= 0) {
                throw new ArgumentOutOfRangeException("Rate value can only be from 1 to 5");
            }
            Review review = new Review();
            review.Description = reviewDTO.Description;
            review.RateValue = reviewDTO.RateValue;
            review.ProductId = productId;
            review.UserId = userId;

            _reviewRepository.Add(review);
            int result = await _reviewRepository.SaveChangesAsync();
            return result;
            
        }

        public async Task<int> EditReview(string userId, string reviewId, ReviewDTO reviewDTO) {
            Review? review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) throw new KeyNotFoundException("Review with this Id was not found");

            if (review.UserId != userId) throw new UnauthorizedAccessException("You are not allowed to edit this review.");
            review.Description = reviewDTO.Description;
            review.RateValue = reviewDTO.RateValue;
            review.IsEdited = true;
            review.LastModifiedAt = DateTime.UtcNow;

            _reviewRepository.Update(review);
            int result = await _reviewRepository.SaveChangesAsync();
            return result;
        }

        public async Task<int> RemoveReview(string userId, string reviewId) {
            Review? review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) throw new KeyNotFoundException("Review with this Id was not found");

            if(review.UserId != userId) throw new UnauthorizedAccessException("You are not allowed to delete this review.");

            _reviewRepository.Remove(review);
            int result = await _reviewRepository.SaveChangesAsync();
            return result;
        }

        public async Task<int> RemoveUserReview(string userId, string reviewId)
        {
            Review? review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) throw new KeyNotFoundException("Review with this Id was not found");

            _reviewRepository.Remove(review);
            int result = await _reviewRepository.SaveChangesAsync();
            return result;
        }
    }
}
