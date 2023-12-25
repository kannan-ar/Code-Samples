using AutoMapper;
using System.ComponentModel.Composition;
using Data = VideoStreamer.Infrastructure;

namespace VideoStreamer.Application.Profiles
{
    [Export(typeof(Profile))]
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Data.Entities.User, Domain.Entities.User>();
        }
    }
}
