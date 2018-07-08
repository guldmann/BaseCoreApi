using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCoreApi.Models
{
    public class PersonService : IPersonService
    {
        private static List<Person> Persons;

        public PersonService()
        {
            //Add a Person 
            if(Persons == null)
            {
                Persons = new List<Person>();
                Persons.Add(new Person { Id = 1, Age = 32, Name = "Joe" });
            }
        }


        public Person Create(string name, int age)
        {
            int id = Persons.LastOrDefault().Id + 1;
            Persons.Add(new Person { Name = name, Age = age, Id = id });
            return Persons.LastOrDefault();           
        }

        public bool Delete(int id)
        {
            if(Persons.Exists(x => x.Id == id))
            {
                var index = Persons.FindIndex(x => x.Id == id);
                Persons.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            return Persons.Exists(x => x.Id == id);
        }

        public Person Get(int id)
        {
            if (Persons.Exists(x => x.Id == id))
            {
                return Persons.Find(x => x.Id == id);
            }
            return null; 
        }

        public List<Person> GetAll()
        {
            return Persons; 
        }
    }
}
