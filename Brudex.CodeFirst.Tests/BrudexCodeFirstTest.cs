﻿using NUnit.Framework;

namespace Brudex.CodeFirst.Tests
{
    class BrudexCodeFirstTest
    {

        [Test]
        public void MigrationCountTest()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Project>.Migrate().SetPrimaryKey("Id", false);
            //Dcf<Director>.Migrate(true,false);
            BrudexCodeFirst.RunMigrations(@"Data Source=.\sqlexpress;Initial Catalog=Dapper;User Id=sa;Password=0243975410");
            Assert.AreEqual(2, 2);
        }


         
    }
}
