using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brudex.CodeFirst
{
    public class ForeignKey
    {
      public  string KeyName { get; set; }
      public string TableName { get; set; }
      public string ReferenceField { get; set; }
      public string ReferenceTable { get; set; }
      public string ColumnName { get; set; }
    }
}
    