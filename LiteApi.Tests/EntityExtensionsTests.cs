using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using LiteApi;

namespace LiteApi.Tests
{
    [TestClass]
    public class EntityExtensionsTests
    {
        [TestMethod]
        public void GetId_must_find_property_with_name_Id()
        {
            var obj = new Entity1 { Id = 1 };
            Assert.AreEqual(1, obj.GetId());
        }
        [TestMethod]
        public void GetId_must_find_property_with_Key_attribute()
        {
            var obj = new Entity2 { ID = 1 };
            Assert.AreEqual(1, obj.GetId());
        }
        [TestMethod]
        public void GetId_must_not_find_property_with_name_ID()
        {
            Exception expectedException = null;
            try
            {
                var obj = new Entity3 { ID = 1 };
                var id = obj.GetId();
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);
        }
        [TestMethod]
        public void GetId_must_find_property_with_Key_attribute_then_with_name_Key()
        {
            var obj = new Entity4 { Name = "1" };
            Assert.AreEqual("1", obj.GetId());
        }

    }

    public class Entity1
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Entity2
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
    }

    public class Entity3
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Entity4
    {
        public int Id { get; set; }
        [Key]
        public string Name { get; set; }
    }
}
