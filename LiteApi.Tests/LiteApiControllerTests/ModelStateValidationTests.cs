using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiteApi.Tests.LiteApiControllerTests
{
    [TestClass]
    public class ModelStateValidationTests : WebApiTestBase
    {
        [TestMethod]
        public void Post_must_result_bad_request_with_enpty_required_field_in_model()
        {
            PostAsJson("http://localhost/api/ModelStateValidationTest", new ModelStateValidationTestDto(),
                message =>
                {
                    Assert.AreEqual(HttpStatusCode.BadRequest, message.StatusCode);
                });
        }
    }

    public class ModelStateValidationTestDto
    {
        public int Id { get; set; }
        [Required]
        public string Required { get; set; }
        [StringLength(5)]
        public string StringLength { get; set; }

        public static ICollection<ModelStateValidationTestDto> Seed()
        {
            return new List<ModelStateValidationTestDto>();
        }
    }

    public class ModelStateValidationQd
    {
    }

    public class ModelStateValidationTestController : LiteApiController<int, ModelStateValidationTestDto, ModelStateValidationTestDto, ModelStateValidationQd>
    {
        public ModelStateValidationTestController()
            : base(new InMemoryCollectionPersistenceService<ModelStateValidationTestDto>(ModelStateValidationTestDto.Seed()))
        { }

        public ModelStateValidationTestController(IPersistenceService persistenceService)
            : base(persistenceService)
        { }
    }
}
