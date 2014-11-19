using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Brudex.CodeFirst.Tests
{
    [TestFixture]
    class TypeHelpersTest
    {
        [Test]
        public void GetTableNameTest()
        {
            var tname = TypeHelpers.GetTableName<Movie>();
            Assert.AreEqual(tname,"Movie");
        }

        [Test]
        public void GetColumnTest()
        {
            var c = TypeHelpers.GetColumn<Movie>(m => m.Name);
            Assert.AreEqual(c.ColumnName,"Name");
            Assert.AreEqual(c.FieldType,DataType.String);
        }

//        [Test]
//        public void GetPropertiesAndFieldsTest()
//        {
//            var cols = TypeHelpers.GetPropertiesAndfields<Movie>();
//            Assert.AreEqual(cols.Count,6);
//        }

        [Test]
        public void GetValueTest()
        {
            Director actor = new Director() { FirstName = "Martin", LastName = "Fowler", Id = 1 };
            var members = TypeHelpers.GetMembers<Director>();
            var mv = members.First(x => x.Name == "FirstName");
            var v = mv.GetValue(actor);
            Assert.AreEqual(v.ToString(),"Superman");
        }
        
        [Test]
        public void GetTypeValuesTest()
        {
            Director actor = new Director() { FirstName = "Martin", LastName = "Fowler", Id =1};
             var dict = TypeHelpers.GetTypeValues(actor);
             Assert.AreEqual(dict["FirstName"].FieldValue,"Martin");
             Assert.AreEqual(dict["LastName"].FieldValue,"Fowler");
             Assert.AreEqual(dict["Id"].FieldValue,"1");          
        }
        [Test]
        public void FloatTest()
        {
            float f = 90.56f;
            string s = f.ToString();
            Assert.AreEqual("90.56",s);
        }


    }
}
