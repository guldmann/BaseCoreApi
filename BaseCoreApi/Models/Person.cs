
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BaseCoreApi.Models
{
    /*
     * This exist for demo purpose only
     */
    public class Person
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,200)]
        public int Age { get; set; }
       
        internal int Id { get; set; } 
    }
}
