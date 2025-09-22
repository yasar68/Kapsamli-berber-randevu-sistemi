using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.Models;

namespace BerberApp.API.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);

        Task<IEnumerable<Review>> GetByUserIdAsync(int userId);

        Task AddAsync(Review review);

        Task UpdateAsync(Review review);

        Task DeleteAsync(Review review);
    }
}
