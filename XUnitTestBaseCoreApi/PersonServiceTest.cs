using System;
using Xunit;
using BaseCoreApi.Models;

namespace XUnitTestBaseCoreApi
{
    public class PersonServiceTest
    {
        private readonly PersonService personService; 

        public PersonServiceTest()
        {
            personService = new PersonService(); 
        }

        [Fact]
        public void PersonServiceCreate()
        {
            var person = personService.Create("Anders", 45); 
            if(person.Name == "Anders" && person.Age == 45)
            {
                Assert.True(true);
                return;
            }
            Assert.True(false);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void PersonServiceGet(int value)
        {
             
            if(value < 2)
            {
                var  person = personService.Get(value); 
                if(person != null)
                {
                    Assert.True(true); 
                }
            }
            else
            {
                var person = personService.Get(value);
                if (person == null)
                {
                    Assert.False(false);
                }
                
            }
        }

     

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void PersonServiceExistsTrue(int value)
        {
            if(value < 2)
            {
                var result = personService.Exists(value);
                Assert.True(result);
                return;
            }
            else 
            {
                var result = personService.Exists(value);
                Assert.False(result);
            }

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void PersonServiceDelete(int value)
        {
            if (value < 2)
            {
                var result = personService.Delete(value);
                Assert.True(result);
                return;
            }
            else
            {
                var result = personService.Delete(value);
                Assert.False(result);
            }
        }
    }
}
