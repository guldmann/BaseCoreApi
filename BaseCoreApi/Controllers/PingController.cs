using Microsoft.AspNetCore.Mvc;

//Heartbeat controller 
namespace BaseCoreApi.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        /// <summary>
        /// Heartbeat to test api for responsiveness  
        /// </summary>
        /// <returns>string "Pong"</returns>
        /// <response code="200">String "Pong"</response> 
        [HttpGet]
        [HttpPost]
        public IActionResult Get()
        {
            var ret = "Pong";
            return Ok(ret);
        }
    }
}
