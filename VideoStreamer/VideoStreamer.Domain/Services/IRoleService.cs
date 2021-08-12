using System.Collections.Generic;
using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain.Services
{
    public interface IRoleService
    {
        public IEnumerable<Role> GetRolesByUserId(int userId);
    }
}
