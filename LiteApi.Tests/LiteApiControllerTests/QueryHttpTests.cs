using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiteApi.Tests.LiteApiControllerTests
{
    [TestClass]
    public class QueryHttpTests : WebApiTestBase
    {
        [TestMethod]
        public void OrderBy_one_property_Http_result()
        {
            Get("http://localhost/api/person?OrderBy=FirstName", message =>
            {
                Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            });
        }

    }
}
