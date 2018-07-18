using Microsoft.AspNetCore.Mvc;

//Heartbeat controller 
namespace BaseCoreApi.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {       
        [HttpGet]
        [HttpPost]
        public IActionResult Get()
        {
            var ret = "Pong";
            return Ok(ret);
        }

    }
}
