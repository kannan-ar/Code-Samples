using System.Collections.Generic;
using VideoStreamer.Infrastructure;
using AutoMapper;
using VideoStreamer.Domain.Entities;
using VideoStreamer.Domain.Services;
using System.Linq;

namespace VideoStreamer.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<Role> GetRolesByUserId(int userId)
        {
            return _mapper.Map<IEnumerable<Role>>(_unitOfWork.RoleRepository.Get(x => x.UserRoles.Any(y => y.UserId == userId)));
        }
    }
}
