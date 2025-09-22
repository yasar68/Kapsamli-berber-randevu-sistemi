using AutoMapper;
using BerberApp.API.DTOs.Review;
using BerberApp.API.Models;

namespace BerberApp.API.Mappings
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            // CreateReviewDto'dan Review modeline dönüşüm
            CreateMap<CreateReviewDto, Review>();

            // UpdateReviewDto'dan Review modeline dönüşüm
            CreateMap<UpdateReviewDto, Review>();

            // Review modelinden CreateReviewDto ve UpdateReviewDto'ya dönüşüm isteğe bağlı
            CreateMap<Review, CreateReviewDto>();
            CreateMap<Review, UpdateReviewDto>();
        }
    }
}
