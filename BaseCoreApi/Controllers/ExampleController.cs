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

        /// <summary>
        /// Get all persons 
        /// </summary>
        /// <remarks>
        /// To access this authentication in header is needed like:
        /// Authorization: Bearer TOKEN-STRING-HERE
        /// </remarks>
        /// <returns>Persons</returns>
        /// <response code="200">Returns all persons</response>        
        [Authorize]
        [HttpGet]
        [ProducesResponseType(200)]
        [Produces("application/json")]
        public IActionResult GetAll()
        {
            var persons = _personService.GetAll();
            _logger.LogDebug($"GetALL: {persons.ToJson()}" );
            return Ok(persons);
        }

        /// <summary>
        /// Get Person by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Person</returns>
        /// <response code="200">Returns person with requested id</response> 
        /// <response code="404">Person with id not found </response> 
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
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

        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="person"></param>        
        /// <returns>New created person</returns>
        /// <response code="201">Returns created person</response> 
        /// <response code="400">Missing name or age</response> 
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public IActionResult Create([Bind("Name","Age")] Person person)
        {
            if(ModelState.IsValid)
            {
                var p = _personService.Create(person.Name, person.Age);
                _logger.LogDebug($"Create  => {person.ToJson()}");
                return CreatedAtAction(nameof(this.Get), p);
            }

            return BadRequest(ModelState);

            //if (string.IsNullOrEmpty(name)) return BadRequest("Missing Name!");
            //if (age > 200 || age < 1 ) return BadRequest("Age ti high or to low "); 

          
        }


        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="person">Person object</param>
        /// <returns>New created person</returns>        
        /// <response code="201">Returns created person</response> 
        /// <response code="400">Missing name or bad age</response> 
        [HttpPut]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public IActionResult Put([FromBody]Person person)
        {
            if(ModelState.IsValid)
            {
                var p = _personService.Create(person.Name, person.Age);
                _logger.LogDebug($"Create  => {person.ToJson()}");
                return CreatedAtAction(nameof(this.Get), p);
            }
            return BadRequest(ModelState);
            //if (string.IsNullOrEmpty(person.Name)) return BadRequest("Missing Name");
            //if (person.Age < 1 || person.Age > 200) return BadRequest("Age to high or to low"); 


        }

        /// <summary>
        /// Delete a person by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="202">Person deleted</response> 
        /// <response code="404">Person with id not found</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        public IActionResult Delete(int id)
        {
            if(_personService.Exists(id))
            {
                var status = _personService.Delete(id);
                return Accepted($"Person with {id} deleted = {status}");
            }
            return NotFound($"Person with {id} not found! "); 
        }
    }
}
