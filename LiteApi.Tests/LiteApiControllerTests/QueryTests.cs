﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            _controller = new PersonController(_persons);
        }

        [TestMethod]
        public void OrderBy_one_property()
        {
            var queryDescriptor = new PersonDtoQueryDescriptor() { OrderBy = new[] { "FirstName" } };
            var result = _controller.Get(queryDescriptor) as OkNegotiatedContentResult<IEnumerable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.IsTrue(items[1].FirstName == "F2");
        }

        [TestMethod]
        public void OrderBy_two_properties()
        {
            var queryDescriptor = new PersonDtoQueryDescriptor() { OrderBy = new[] { "FirstName", "LastName" } };
            var result = _controller.Get(queryDescriptor) as OkNegotiatedContentResult<IEnumerable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.IsTrue(items[1].FirstName == "F2");
            Assert.IsTrue(items[3].LastName == "L3");
        }

        [TestMethod]
        public void Where_property_equals_value()
        {
            var qd = new PersonDtoQueryDescriptor() { FirstName = "F4" };
            var result = _controller.Get(qd) as OkNegotiatedContentResult<IEnumerable<PersonDto>>;
            Assert.IsNotNull(result);
            var items = result.Content.ToList();
            Assert.AreEqual(2, items.Count);
        }
    }
}
