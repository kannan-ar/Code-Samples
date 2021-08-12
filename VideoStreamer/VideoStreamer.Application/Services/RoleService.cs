using System.Collections.Generic;
using VideoStreamer.Data;
using AutoMapper;
using VideoStreamer.Domain.Entities;
using VideoStreamer.Domain.Services;

namespace VideoStreamer.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public IEnumerable<Role> GetRolesByUserId(int userId)
        {
            return _mapper.Map<IEnumerable<Role>>(_roleRepository.GetRolesByUserId(1));
        }
    }
}
