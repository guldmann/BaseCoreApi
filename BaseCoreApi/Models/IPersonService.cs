using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCoreApi.Models
{
    interface IPersonService
    {
        Person GetAll();
        Person Get(int id);
        Person Create(string name, int age);

        bool Delete(int id); 


    }
}
