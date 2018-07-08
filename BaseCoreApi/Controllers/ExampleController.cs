using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BaseCoreApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BaseCoreApi.Controllers
{
    [Route("api/[controller]")]
    public class ExampleController : Controller
    {
        private readonly IPersonService _personService;
        public ExampleController(IPersonService personService)
        {
            _personService = personService; 
        }


        // GET: api/<controller>
        [HttpGet]
        public IActionResult GetAll()
        {
            var persons = _personService.GetAll();
            return Ok(persons);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if(_personService.Exists(id))
            {
                var person = _personService.Get(id);
                return Ok(person); 
            }
            return NotFound();
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Create(string name, int age)
        {
            var person = _personService.Create(name, age);
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
