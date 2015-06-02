using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiteApi
{
    //TODO: use [Key] attribute to find Id
    internal static class EntityExtensions
    {
        public static object GetId(this object obj)
        {
            var prop = GetIdProperty(obj);
            return prop.GetValue(obj);
        }
        public static void SetId(this object obj, object value)
        {
            var prop = GetIdProperty(obj);
            prop.SetValue(obj, value);
        }

        private static PropertyInfo GetIdProperty(object obj)
        {
            var propsWithKeyAttribute = obj.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeyAttribute))).ToArray();
            if (propsWithKeyAttribute.Length > 1)
                throw new Exception("More than one property with KeyAttribute defined.");
            if (propsWithKeyAttribute.Length == 0)
            {
                return obj.GetType().GetProperty("Id");
            }
            return propsWithKeyAttribute[0];
        }

    }
}
