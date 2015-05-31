using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrongApi.Tests.StrongApiControllerTests
{
    [TestClass]
    public class ControllerLogicTests
    {
        private readonly ICollection<Person> _persons;
        private PersonController _controller;

        public ControllerLogicTests()
        {
            _persons = Person.Seed();
            _controller = new PersonController(_persons);
        }

        [TestMethod]
        public void Get_with_id_must_return_dto_with_that_id()
        {
            var result = _controller.Get(1) as OkNegotiatedContentResult<PersonDto>;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.Id);
        }

        [TestMethod]
        public void Get_with_id_not_in_collection_must_result_with_not_found()
        {
            var result = _controller.Get(100) as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Post_must_add_a_person_to_collection_and_return_created_with_its_route()
        {
            var result = _controller.Post(new PersonDto() { FirstName = "FirstName1", LastName = "LastName1" }) as CreatedAtRouteNegotiatedContentResult<PersonDto>;
            Assert.IsNotNull(result);
            Assert.AreEqual("FirstName1", result.Content.FirstName);
            Assert.IsTrue(_persons.Any(p => p.FirstName == "FirstName1"));
        }

        //TODO: Test post with invalid modelstate

        [TestMethod]
        public void Put_must_edit_a_person_and_return_ok()
        {
            var getResult = _controller.Get(1) as OkNegotiatedContentResult<PersonDto>;
            getResult.Content.FirstName = "ChangedFirstName";
            var putResult = _controller.Put(getResult.Content.Id, getResult.Content) as OkResult;
            Assert.IsNotNull(putResult);
            Assert.AreEqual("ChangedFirstName", _persons.Single(p => p.Id == 1).FirstName);
        }

        [TestMethod]
        public void Put_must_return_not_found_when_trying_to_change_a_person_that_not_exists()
        {
            var result = _controller.Put(100, new PersonDto()) as NotFoundResult;
            Assert.IsNotNull(result);
        }
    }
}
