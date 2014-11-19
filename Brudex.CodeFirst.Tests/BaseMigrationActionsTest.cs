using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Brudex.CodeFirst.Tests
{
    [TestFixture]
    class BaseMigrationActionsTest
    {
        [Test]
            public void TestInitializationOfBaseMigrationActions()
            {
                BaseMigrationActions<Movie> m=new BaseMigrationActions<Movie>("Movie",false);
                Assert.AreEqual(m.TableName,"Movie");
                Assert.AreEqual(m.IdField,"Id");
                Assert.AreEqual(m.NoPrimaryKey,false);
                Assert.AreEqual(m.AutoIncrementId, true);
                Assert.AreEqual(m.Columns.Count, 4);
                Assert.AreEqual(m.ForeignKeys.Count,0);
            }

        [Test]
        public void IgnoreFieldsTest()
        {
            BaseMigrationActions<Movie> m = new BaseMigrationActions<Movie>("Movie", false);
            m.IgnoreFields(new List<string>() {"Author"});
            Assert.Contains("Author", m.FiedsToIgnore);
             
        } 
        
        [Test]
        public void IgnoreSingleFieldTest()
        {
            BaseMigrationActions<Movie> m = new BaseMigrationActions<Movie>("Movie", false);
            m.IgnoreField(f=>f.Author);
            Assert.Contains("Author", m.FiedsToIgnore);
             
        } 
        
        [Test]
        public void SetPrimaryKeyTest()
        {
            BaseMigrationActions<Movie> m = new BaseMigrationActions<Movie>("Movie", false);
            m.SetPrimaryKey(f=>f.Id,false);
            Assert.AreEqual(m.IdField, "Id");   
            Assert.AreEqual(m.AutoIncrementId,false);
        }

        [Test]
        public void CreateForeignKeyTest()
        {
            BaseMigrationActions<Movie> m = new BaseMigrationActions<Movie>("Movie", false);
            m.CreateForeignKeyOn(f => f.DirectorId, "Director", "Id");
            
            Assert.AreEqual(m.ForeignKeys.Count, 1);
            var fk = m.ForeignKeys.First();
            Assert.AreEqual(fk.ReferenceTable,"Director");
            Assert.AreEqual(fk.ReferenceField,"Id");
            Assert.AreEqual(fk.ColumnName,"DirectorId");
        } 
        
        [Test]
        public void CreateForeignKeyTest2()
        {
            BaseMigrationActions<Movie> m = new BaseMigrationActions<Movie>("Movie", false);
            m.CreateForeignKeyOn<Director>(f => f.DirectorId,t=>t.Id);
            Assert.AreEqual(m.ForeignKeys.Count, 1);
            var fk = m.ForeignKeys.First();
            Assert.AreEqual(fk.ReferenceTable,"Director");
            Assert.AreEqual(fk.ReferenceField,"Id");
            Assert.AreEqual(fk.ColumnName,"DirectorId");
        }
        
        [Test]
        public void CreateForeignKeyTest3()
        {
            BaseMigrationActions<Movie> m = new BaseMigrationActions<Movie>("Movie", false);
            m.CreateForeignKeyOn<Director>(f => f.DirectorId, t => t.Id).CreateForeignKeyOn<Director>(f => f.Author, t => t.LastName);
            Assert.AreEqual(m.ForeignKeys.Count, 2);
            var fk = m.ForeignKeys.First();
            Assert.AreEqual(fk.ReferenceTable,"Director");
            Assert.AreEqual(fk.ReferenceField,"Id");
            Assert.AreEqual(fk.ColumnName,"DirectorId");
        }





    }


      
}
