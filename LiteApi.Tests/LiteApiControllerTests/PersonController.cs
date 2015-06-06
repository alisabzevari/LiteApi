using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LiteApi.Tests.LiteApiControllerTests
{
    [RoutePrefix("api/person")]
    public class PersonController : LiteApiController<int, PersonDto, Person, PersonDtoQueryDescriptor>
    {
        public PersonController()
            : base(new InMemoryCollectionPersistenceService<Person>(Person.Seed()))
        { }
        public PersonController(IPersistenceService collection)
            : base(collection)
        { }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public int Age { get; set; }

        public static ICollection<Person> Seed()
        {
            return new List<Person>()
            {
                new Person {Id = 0, FirstName = "F1", LastName = "L1", IsAdmin = true, Age = 18},
                new Person {Id = 1, FirstName = "F3", LastName = "L3", IsAdmin = false, Age = 81},
                new Person {Id = 2, FirstName = "F2", LastName = "L2", IsAdmin = true, Age = 50},
                new Person {Id = 3, FirstName = "F4", LastName = "L4", IsAdmin = false, Age = 40},
                new Person {Id = 4, FirstName = "F4", LastName = "L3", IsAdmin = false, Age = 35},
            };
        }
    }

    public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
    }

    public class PersonDtoQueryDescriptor
    {
        public string FirstName { get; set; }
        public string FirstName_Contains { get; set; }
        public int? Id_Gt { get; set; }
        public int? Id_Ge { get; set; }
        public int? Id_Lt { get; set; }
        public int? Id_Le { get; set; }
        public int? Id_Ne { get; set; }
        public string[] OrderBy { get; set; }
        public string[] OrderByDesc { get; set; }
        public int? Take { get; set; }
        public int? Skip { get; set; }
        [Where("Age >= 50")]
        public string OldPersons { get; set; }
    }
}
