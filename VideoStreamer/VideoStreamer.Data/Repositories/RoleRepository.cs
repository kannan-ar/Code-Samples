using System;
using System.Collections.Generic;
using System.Text;
using VideoStreamer.Data.Entities;

namespace VideoStreamer.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public IEnumerable<Role> GetRolesByUserId(int userId)
        {
            yield return new Role
            {
                Id = 1,
                RoleName = "Admin"
            };
        }
    }
}
