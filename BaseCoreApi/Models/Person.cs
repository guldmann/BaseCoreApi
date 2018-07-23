
using System.ComponentModel.DataAnnotations;

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
        public int Id { get; set; }
    }
}
