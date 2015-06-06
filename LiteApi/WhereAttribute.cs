using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteApi
{
    public class WhereAttribute : Attribute
    {
        public string WhereClause { get; private set; }
        public WhereAttribute(string whereClause)
        {
            WhereClause = whereClause;
        }
    }
}
