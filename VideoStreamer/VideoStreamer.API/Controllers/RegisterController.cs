using Microsoft.AspNetCore.Mvc;
using VideoStreamer.Domain.Services;

namespace VideoStreamer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public RegisterController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = _userService.GetUserById(1);
            var roles = _roleService.GetRolesByUserId(1);

            return Ok(user);
        }
    }
}
