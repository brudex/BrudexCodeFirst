using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brudex.CodeFirst;
using Brudex.CodeFirst.Tests;
using NUnit.Framework;

namespace Dapper.CodeFirst.Tests
{
    /// <summary>
    /// These tests are mainly to test if database changes are right
    /// </summary>
     [TestFixture]
    class BaseMigrationsTest
    {
        [Test]
        public void TestMigrateWithIgnoreFields()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate(includeNonPrimitives:true).IgnoreField(f=>f.AgeLimit).IgnoreFields(new List<string>(){"directors"});
            var imigrate= BrudexCodeFirst.GetMigration<Movie>(0);
            var  m = imigrate as  BaseMigrationActions<Movie>;
            Assert.Contains("directors", m.FiedsToIgnore);
            Assert.Contains("AgeLimit", m.FiedsToIgnore);
             
        }

        [Test]
        public void TestMigrateSetPrimaryKey()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Movie>.Migrate().SetPrimaryKey(f=>f.Id);
            var imigrate= BrudexCodeFirst.GetMigration<Movie>(0);
            var  m = imigrate as  BaseMigrationActions<Movie>;
            Assert.AreEqual(false, m.NoPrimaryKey);
            Assert.AreEqual("Id", m.IdField);
        }

         [Test]
        public void TestMigrateSetPrimaryKey2()
        {

            BrudexCodeFirst.ClearMigrations();
            Bcf<Project>.Migrate().SetPrimaryKey("project_name",false);
            var imigrate= BrudexCodeFirst.GetMigration<Project>(0);
            var  m = imigrate as  BaseMigrationActions<Project>;
            Assert.AreEqual(false, m.NoPrimaryKey);
            Assert.AreEqual("project_name", m.IdField);
             
        }



         [Test]
        public void TestMigrateSetForeignKey()
        {
            BrudexCodeFirst.ClearMigrations();
            Bcf<Actor>.Migrate().CreateForeignKeyOn<Movie>(f=>f.movieId,q=>q.Id).CreateForeignKeyOn(f => f.DirectorId,"Director","Id");
            
            var imigrate= BrudexCodeFirst.GetMigration<Actor>(0);
            var  m = imigrate as  BaseMigrationActions<Actor>;
            
            Assert.AreEqual(2, m.ForeignKeys.Count);
            var fkfield1 = m.ForeignKeys.First(x => x.ReferenceTable == "Movie");
            var fkfield2 = m.ForeignKeys.First(x => x.ReferenceTable == "Director");
            Assert.AreEqual("Movie", fkfield1.ReferenceTable);
            Assert.AreEqual("Actor", fkfield1.TableName);
            Assert.AreEqual("movieId", fkfield1.ColumnName);
            Assert.AreEqual("Id", fkfield1.ReferenceField);

            Assert.AreEqual("Director", fkfield2.ReferenceTable);
            Assert.AreEqual("Actor", fkfield2.TableName);
            Assert.AreEqual("DirectorId", fkfield2.ColumnName);
            Assert.AreEqual("Id", fkfield2.ReferenceField);     
        }

 
         
       
    }
}
