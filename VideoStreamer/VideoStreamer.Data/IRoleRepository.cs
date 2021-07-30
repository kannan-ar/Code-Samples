using System;
using System.Collections.Generic;
using System.Text;
using VideoStreamer.Data.Entities;

namespace VideoStreamer.Data
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetRolesByUserId(int userId);
    }
}
