using System.Collections.Generic;
using NUnit.Framework;
using Brudex.CodeFirst;

namespace Dapper.CodeFirst.Tests
{
    class ConnectionFactoryTest
    {
        [Test]
        public void TestGetTableInformation()
        {
            ConnectionFactory confactory=new ConnectionFactory(ConnectionType.SqlServer, ConnectionStrings.SqlServer);
            var list= confactory.GetTableInformation("Director");
            Assert.Greater(list.Count,0);
        }

        [Test]
        public void TestIsFirstMigrationsIsFalseForExistingTables()
        {
            ConnectionFactory confactory=new ConnectionFactory(ConnectionType.SqlServer, ConnectionStrings.SqlServer);
            var existings = new List<string>() {"Director", "Movie"};
            var IsFalse= confactory.IsFirstMigrations(existings);
            Assert.IsFalse(IsFalse);
           
        }

        [Test]
        public void TestIsFirstMigrationsIsTrueForNonExistingTables()
        {
            ConnectionFactory confactory = new ConnectionFactory(ConnectionType.SqlServer, ConnectionStrings.SqlServer);
          
            var nonexistings = new List<string>() { "Order", "Employer" };
            var IsTrue = confactory.IsFirstMigrations(nonexistings);
             Assert.IsTrue(IsTrue);
        }


    }
}
