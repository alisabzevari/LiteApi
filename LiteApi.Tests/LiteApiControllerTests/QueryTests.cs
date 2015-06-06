using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Dynamic;

namespace LiteApi.Tests.LiteApiControllerTests
{
    [TestClass]
    public class QueryTests
    {
        private readonly ICollection<Person> _persons;
        private readonly PersonController _controller;

        public QueryTests()
        {
            _persons = Person.Seed();
            _controller = new PersonController(new InMemoryCollectionPersistenceService<Person>(_persons));
        }

        [TestMethod]
        public void OrderBy_one_property()
        {
            var queryDescriptor = new PersonDtoQueryDescriptor() { OrderBy = new[] { "FirstName" } };
            var result = _controller.Get(queryDescriptor) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.IsTrue(items[1].FirstName == "F2");
        }

        [TestMethod]
        public void OrderBy_two_properties()
        {
            var queryDescriptor = new PersonDtoQueryDescriptor() { OrderBy = new[] { "FirstName", "LastName" } };
            var result = _controller.Get(queryDescriptor) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.IsTrue(items[1].FirstName == "F2");
            Assert.IsTrue(items[3].LastName == "L3");
        }

        [TestMethod]
        public void Where_property_equals_value()
        {
            var qd = new PersonDtoQueryDescriptor() { FirstName = "F4" };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(2, items.Count);
        }

        [TestMethod]
        public void Where_property_greater_than_value()
        {
            var qd = new PersonDtoQueryDescriptor() { Id_Gt = 2 };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(2, items.Count);
        }

        [TestMethod]
        public void Where_property_Gt_and_Le_together()
        {
            var qd = new PersonDtoQueryDescriptor() { Id_Gt = 2, Id_Le = 3 };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(1, items.Count);
            Assert.AreEqual(3, items[0].Id);
        }

        [TestMethod]
        public void Skip_and_Take()
        {
            var qd = new PersonDtoQueryDescriptor() { Skip = 1, Take = 2 };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(1, items[0].Id);
        }

        [TestMethod]
        public void DynamicLinqTest()
        {
            IQueryable list = _persons.AsQueryable();
            var result = list.Where("Id == @0", 2);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void CustomWhereClause()
        {
            var qd = new PersonDtoQueryDescriptor() { OldPersons = "" };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(2, items.Count);
        }

        [TestMethod]
        public void CustomWhereClause_must_be_ignored_when_value_of_property_is_null()
        {
            var qd = new PersonDtoQueryDescriptor();
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(5, items.Count);
        }

        [TestMethod]
        public void CustomWhereClause_with_one_parameter()
        {
            var qd = new PersonDtoQueryDescriptor() { OlderThan = 50 };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(2, items.Count);
        }

        [TestMethod]
        public void CustomWhereClause_whith_multiple_value_parameter()
        {
            var qd = new PersonDtoQueryDescriptor() { AgeBetween = new[] { 0, 50 } };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IQueryable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(4, items.Count);
        }
    }
}
