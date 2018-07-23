using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCoreApi.Models
{
    public interface IPersonService
    {
        List<Person> GetAll();
        Person Get(int id);
        Person Create(string name, int age);

        bool Delete(int id);

        bool Exists(int id);
        bool Validate(string name, int age);
        bool Validate(Person person); 


    }
}
