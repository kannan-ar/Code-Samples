using AutoMapper;
using System.Threading.Tasks;
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

        public async Task<User> GetUserById(int id)
        {
            return _mapper.Map<User>(await _unitOfWork.UserRepository.GetById(id));
        }
    }
}
