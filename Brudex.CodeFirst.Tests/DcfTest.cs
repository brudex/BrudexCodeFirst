﻿using NUnit.Framework;

namespace Brudex.CodeFirst.Tests
{
    [TestFixture]
    class DcfTest
    {
    //    [Test]
//        public void CheckIfFluentMethodCallWorks()
//        {
//            Dcf<Movie>.Migrate().SetPrimaryKey(f => f.DirectorId).IgnoreField(f => f.Author).IgnoreField(f => f.Name);
//            var m = DapperCodeFirst.GetMigration<Movie>(0);
//            Assert.AreEqual(m.IdField, "DirectorId");
//            Assert.AreEqual(m.FiedsToIgnore.Count, 2);
//        }


        [Test]
        public void MigrationCountTest()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate();
            Bcf<Director>.Migrate();
            int c = BrudexCodeFirst.GetMigrationCount();
            Assert.AreEqual(c, 2);
        }

        [Test]
        public void MigrationSqlTest()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate(true, false);
            Bcf<Director>.Migrate(true,false);
            string sql= BrudexCodeFirst.GetSqlQuery();
            Assert.AreEqual(sql, "");
        }
        
        
    }


    


}
