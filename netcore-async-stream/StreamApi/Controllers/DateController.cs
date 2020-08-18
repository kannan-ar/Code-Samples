namespace StreamApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class DateController : ControllerBase
    {
        public async Task<DateTime> Get(int delay)
        {
            await Task.Delay(delay);
            return DateTime.Now;
        }
    }
}