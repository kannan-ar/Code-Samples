using AutoMapper;
using Data = VideoStreamer.Infrastructure;

namespace VideoStreamer.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Data.Entities.User, Domain.Entities.User>();
        }
    }
}
