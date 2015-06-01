using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StrongApi.Tests.StrongApiControllerTests
{
    [TestClass]
    public class RoutingTests : WebApiTestBase
    {
        [TestMethod]
        public void Post_must_return_created_url()
        {
            PostAsJson("http://localhost/api/person", new PersonDto() { Id = 100, FirstName = "New F1", LastName = "new L1" }, message =>
            {
                Assert.AreEqual(HttpStatusCode.Created, message.StatusCode);
                Assert.AreEqual("http://localhost/api/person?id=100", message.Headers.Location.AbsoluteUri);
            });
        }

        [TestMethod]
        public void Get_with_querystring_must_route_correctly()
        {
            Get("http://localhost/api/person?id=0", message => Assert.AreEqual(HttpStatusCode.OK, message.StatusCode));
        }

        //[TestMethod]
        //public void Get_with_a_query()
        //{
        //    Get("http://localhost/api/person?FirstName=ali");
        //    Get("http://localhost/api/person?Id_Gt=2");
        //    Get("http://localhost/api/person?Id_Gt=2&Skip=2&Take=10&OrderBy=FirstName,LastName");
        //}
    }
}
