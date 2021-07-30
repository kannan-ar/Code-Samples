using System;
using System.Collections.Generic;
using System.Text;
using VideoStreamer.Data.Entities;

namespace VideoStreamer.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        public User GetUserById(int id)
        {
            return new User
            {
                Id = id,
                UserName = $"User{id}"
            };
        }
    }
}
