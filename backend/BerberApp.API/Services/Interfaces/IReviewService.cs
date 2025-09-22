using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;
using BerberApp.API.DTOs.Review;

namespace BerberApp.API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review> AddReviewAsync(CreateReviewDto createReviewDto);
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task UpdateReviewAsync(UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(int reviewId);
    }
}
