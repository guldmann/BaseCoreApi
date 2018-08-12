using System;
using System.Collections.Generic;
using System.Linq;
using BaseCoreApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseCoreApi.Models
{


    //TODO: Rewrite this to use database 
    public class PersonService : IPersonService    
    {
        private static List<Person> Persons;
        private readonly PersonContext _personContext;

        public PersonService(PersonContext personContext)
        {
            _personContext = personContext;

            //Add a default Person 
            if (Persons == null || Persons.Count <1 )
            {
                Persons = new List<Person>();
                Persons.Add(new Person { PersonId = 1, Age = 32, Name = "Joe" });
            }
        }
        public bool Validate(string name, int age)
        {
            //TODO: Implement
            return false; 
        }
        public bool Validate(Person person)
        {
            //TODO: Implement
            return false; 
        }

        public Person Create(string name, int age)
        {
            _personContext.Person.Add(new Person { Name = name, Age = age });
            /*
            int id = Persons.LastOrDefault().PersonId + 1;
            Persons.Add(new Person { Name = name, Age = age, PersonId = id });
            */
           var result =  _personContext.SaveChanges();
            return _personContext.Person.LastOrDefault();           
        }

        public bool Delete(int id)
        {
            if(Persons.Exists(x => x.PersonId == id))
            {
                var index = Persons.FindIndex(x => x.PersonId == id);
                Persons.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            return Persons.Exists(x => x.PersonId == id);
        }

        public Person Get(int id)
        {
           


            if (Persons.Exists(x => x.PersonId == id))
            {
                return Persons.Find(x => x.PersonId == id);
            }
            return null; 
        }

        public List<Person> GetAll()
        {
            
                return Persons; 
        }
    }
}
