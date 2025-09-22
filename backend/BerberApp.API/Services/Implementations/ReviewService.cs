using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;
using BerberApp.API.DTOs.Review;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Services.Interfaces;

namespace BerberApp.API.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> AddReviewAsync(CreateReviewDto createReviewDto)
        {
            var review = new Review
            {
                UserId = createReviewDto.UserId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                CreatedDate = System.DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);

            return review;
        }
        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _reviewRepository.GetByIdAsync(reviewId);
        }

        public async Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(updateReviewDto.ReviewId);
            if (existingReview != null)
            {
                existingReview.Rating = updateReviewDto.Rating;
                existingReview.Comment = updateReviewDto.Comment;
                // Güncelleme zamanı veya başka alanlar varsa burada ekle

                await _reviewRepository.UpdateAsync(existingReview);
            }
            else
            {
                // İstersen hata fırlatabilirsin
                throw new KeyNotFoundException("Güncellenecek yorum bulunamadı.");
            }
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review != null)
            {
                await _reviewRepository.DeleteAsync(review);
            }
            else
            {
                // İstersen hata fırlatabilirsin
                throw new KeyNotFoundException("Silinecek yorum bulunamadı.");
            }
        }
    }
}
