using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using VideoStreamer.Data;
using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain.Services
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
