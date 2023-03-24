﻿using AutoMapper;
using VideoStreamer.Data;
using VideoStreamer.Domain.Entities;
using VideoStreamer.Domain.Services;

namespace VideoStreamer.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public User GetUserById(int id)
        {
            return _mapper.Map<User>(_userRepository.GetUserById(id));
        }
    }
}