using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoStreamer.API.Models;
using VideoStreamer.Domain;

namespace VideoStreamer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUserService userService, IRoleService roleService)
        {
            _logger = logger;
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var user = _userService.GetUserById(1);
            var roles = _roleService.GetRolesByUserId(1);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
