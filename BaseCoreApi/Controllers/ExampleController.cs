using Microsoft.AspNetCore.Mvc;
using BaseCoreApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BaseCoreApi.Controllers
{
    [Route("api/[controller]")]
    public class ExampleController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ILogger<ExampleController> _logger;
        public ExampleController(IPersonService personService,ILogger<ExampleController> logger )
        {
            _personService = personService;
            _logger = logger; 
        }


        // GET: api/<controller>
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var persons = _personService.GetAll();
            _logger.LogDebug($"GetALL: {persons.ToJson()}" );
            return Ok(persons);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {           
            if(_personService.Exists(id))
            {
                var person = _personService.Get(id);
                _logger.LogDebug($"Get id:{id} => {person.ToJson()} " );
                return Ok(person); 
            }
            return NotFound();
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Create(string name, int age)
        {
            
            var person = _personService.Create(name, age);
            _logger.LogDebug($"Create {name}, {age} => {person.ToJson()}" ); 
            return CreatedAtAction(nameof(this.Get), person); 
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if(_personService.Exists(id))
            {
                var status = _personService.Delete(id);
                return Accepted();
            }
            return BadRequest(); 
        }
    }
}
