using System.Collections.Generic;
using System.Threading.Tasks;
using VideoStreamer.Domain.Entities;

namespace VideoStreamer.Domain.Services
{
    public interface IRoleService
    {
        public Task<IEnumerable<Role>> GetRolesByUserId(int userId);
    }
}
