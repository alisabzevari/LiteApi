using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrongApi
{
    //TODO: use [Key] attribute to find Id
    internal static class EntityExtensions
    {
        public static object GetId(this object obj)
        {
            var prop = obj.GetType().GetProperty("Id");
            return prop.GetValue(obj);
        }
        public static void SetId(this object obj, object value)
        {
            var prop = obj.GetType().GetProperty("Id");
            prop.SetValue(obj, value);
        }

    }
}
