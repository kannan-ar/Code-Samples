using System;
using System.Collections.Generic;
using System.Text;
using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain
{
    public interface IRoleService
    {
        public IEnumerable<Role> GetRolesByUserId(int userId);
    }
}
