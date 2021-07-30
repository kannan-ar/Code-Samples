using System;
using System.Collections.Generic;
using System.Text;
using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain
{
    public interface IUserService
    {
        public User GetUserById(int id);
    }
}
