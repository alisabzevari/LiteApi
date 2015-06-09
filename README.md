# LiteApi
LiteApi is an ApiController that simplifies creating RESTfulish apis. It helps service designer to use ASP.NET WebApi for writing message based services. It inspired by ServiceStack's AutoQuery.

**Caution:** This project is in early stages of development.

## How to use
To use LiteApi, you must define 3 types and then inherit your controller from `LiteApiController`. These types are:

1. Dto
2. Entity
3. QueryDescriptor

All of these types can be simple classes and no need to inherit them from specific classes. LiteApiController maps Entity to Dto with `AutoMapper`. Users of controller see Dto but LiteApiController works with entity when it needs to work with persistence layer.

QueryDescriptor defines how users of controller can query collection of entity. 

###Example:

Suppose that your entity is:
```csharp
public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
}
```

And your Dto is:
```csharp
public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
```

And your QueryDesriptor is:
```csharp
public class PersonDtoQueryDescriptor
    {
        public string FirstName { get; set; }
        public int? Id_Gt { get; set; }
        public int? Id_Ge { get; set; }
        public int? Id_Lt { get; set; }
        public int? Id_Le { get; set; }
        public int? Id_Ne { get; set; }
        public string[] OrderBy { get; set; }
        public string[] OrderByDesc { get; set; }
        public int? Take { get; set; }
        public int? Skip { get; set; }
    }
```

You just need to define your ApiController as:
```csharp
public class PersonController : LiteApiController<int, PersonDto, Person, PersonDtoQueryDescriptor>
    {
        public PersonController()
            : base(new InMemoryCollectionPersistenceService<Person>(Person.Seed()))
        { }
        public PersonController(IPersistenceService collection)
            : base(collection)
        { }
    }
```
**Note:** `InMemoryCollectionPersistenceService<Person>` is an implementation of `IPersistenceService` that connects your api to your DAO class. You can write your own IPersistenceService. There is an implementation for `ICollection<T>` and NHibernate PersistenceService implemented out of the box.

After that you can work with your api:

Request | Meaning
--------|--------
`GET http://localhost/api/Person/1` | Get Person with Id = 1
`GET http://localhost/api/Person?Take=10&Skip=2` | Get 2nd 10 Persons
`GET http://localhost/api/Person?Id_Gt=3` | Get Persons with Id Greater Than 3
