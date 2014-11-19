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
    class SeedTest
    {

        [Test]
        public void ManualSeedingWithLists()
        {
            BrudexCodeFirst.ClearMigrations();
            var movielist = new List<Movie>();
            movielist.Add(new Movie() {Id=0,Name="Bill",Author = "Me",AgeLimit = 20,DirectorId = 2});
            movielist.Add(new Movie() {Id=0,Name="Jill",Author = "Him",AgeLimit = 21,DirectorId = 3});
            Bcf<Movie>.Migrate().Seed(movielist);
            //Bcf<Director>.Migrate();
            BrudexCodeFirst.RunMigrations(ConnectionStrings.SqlServer, options: MigrationOptions.RecreateEveryTime);
            
        }

        [Test]
        public void AutoSeedingWithLists()
        {
            BrudexCodeFirst.ClearMigrations();
            var directors = new List<Director>();
            directors.Add(new Director() { Id = 0, LastName = "Bill", FirstName = "Me" });
            Bcf<Director>.Migrate().AutoSeed();
            //Bcf<Director>.Migrate();
            BrudexCodeFirst.RunMigrations(ConnectionStrings.SqlServer, options: MigrationOptions.RecreateEveryTime);

        }

    }
}
