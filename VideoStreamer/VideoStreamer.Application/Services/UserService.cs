using AutoMapper;
using VideoStreamer.Domain.Entities;
using VideoStreamer.Domain.Services;
using VideoStreamer.Infrastructure;

namespace VideoStreamer.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public User GetUserById(int id)
        {
            return _mapper.Map<User>(_unitOfWork.UserRepository.GetById(id));
        }
    }
}
