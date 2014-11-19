using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brudex.CodeFirst;
using Brudex.CodeFirst.Tests;
using NUnit.Framework;

namespace Dapper.CodeFirst.Tests
{
    [TestFixture]
    class BaseMigrationSqlTest
    {
        private static ConnectionFactory cf=new ConnectionFactory(ConnectionType.SqlServer,ConnectionStrings.SqlServer);
        [Test]
        public void TestMigrateWithIgnoreFields()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate(includeNonPrimitives: true).IgnoreField(f => f.AgeLimit).IgnoreFields(new List<string>() { "directors" });
            var imigrate = BrudexCodeFirst.GetMigration<Movie>(0);
            var m = imigrate as BaseMigrationActions<Movie>;
            BrudexCodeFirst.RunMigrations(ConnectionStrings.SqlServer,options:MigrationOptions.RecreateEveryTime);
            var schema = cf.GetTableInformation("Movie");
            var columnnames = schema.Select(x => x.ColumnName).ToList();
            bool notcontains = !columnnames.Contains("directors");
            bool notcontains2 = !columnnames.Contains("AgeLimit");
            Assert.AreEqual(m.Columns.Count ,schema.Count);

            Assert.IsTrue(notcontains);
            Assert.IsTrue(notcontains2);
             
            
        }

        
        [Test]
        public void TestMigrateSetPrimaryKey()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate().SetPrimaryKey(f => f.Id);
            var imigrate = BrudexCodeFirst.GetMigration<Movie>(0);
            var m = imigrate as BaseMigrationActions<Movie>;

            BrudexCodeFirst.RunMigrations(ConnectionStrings.SqlServer, options: MigrationOptions.RecreateEveryTime);
            var schema = cf.GetTableInformation("Movie");
            
             //Manualy checked to see if Id is primary key in Database
            //Send me a mail (brudexgh@gmail.com) if anyone finds a way to unit test this without manual check
            Assert.AreEqual(m.Columns.Count, schema.Count);
 
        }


        /// <summary>
        /// Test to see if auto-increment is added for integer primary keys
        /// </summary>
        [Test]
        public void TestMigrateSetPrimaryKey2()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Project>.Migrate().SetPrimaryKey("id", true);
            var imigrate = BrudexCodeFirst.GetMigration<Project>(0);
            var m = imigrate as BaseMigrationActions<Project>;

            BrudexCodeFirst.RunMigrations(ConnectionStrings.SqlServer, options: MigrationOptions.RecreateEveryTime);
            var schema = cf.GetTableInformation("Project");

            //Manualy checked to see if Id is primary key in Database
            //Send me a mail (brudexgh@gmail.com) if anyone finds a way to unit test this without manual check
            Assert.AreEqual(m.Columns.Count, schema.Count);
            

        }



        [Test]
        public void TestMigrateSetForeignKey()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate();
            Bcf<Director>.Migrate();
            Bcf<Actor>.Migrate().CreateForeignKeyOn<Movie>(f => f.movieId, q => q.Id).CreateForeignKeyOn(f => f.DirectorId, "Director", "Id");
            BrudexCodeFirst.RunMigrations(ConnectionStrings.SqlServer, options: MigrationOptions.RecreateEveryTime);
         
            
           
        }

 
         




    }
}
