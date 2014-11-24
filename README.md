# BrudexCodeFirst

If you are like me and you prefer micro orms like dapper, ormlite. BrudexCodeFirst is just for you.

I created Code first migrations to allow me to still use my micro orms and not have to open management studio and have to use bulky EF just for CodeFirstMigrations.

Lets cut to the chase.
The Nuget Command: 

###Install-Package Brudex.CodeFirst

The package has several migration options you can work in.
- Migrate Once - Allows you to migrate you db only the first time the application runs. Subsequent application runs does not trigger migrations. Best for production environment where Database is to be migrated once.
   
- Recreate Every Time - Recreate Tables every time the application runs. Best for development time where you are still structuring your classes.

- Recreate Only Changed Models - Recreates Tables for classes that have changed only. Best for development where you have data in other tables you don't won't to loose but want to scrap other tables.
- Alter On Model Changes - Does not do a recreate but modifies tables to reflect changes. Best for situation where models have changed but you don't want to loose the data in their tables.


Assuming we have the following classes we want to migrate to the database.

```
 public class Director
    {
        public string Id;
        public int realeasNo;
        public string FirstName { get; set; }
        public string LastName;
        public int ActorId{get;set;}
        private string Unreleased = "Superman";
    }

    public class Actor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int movieId { get; set; }
        public int DirectorId { get; set; }

    }
    
    public class Movie
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Author{ get; set; }
        public int AgeLimit{ get; set; }
        private string Director { get; set; }
        public int DirectorId { get; set; }
        public Director directors { get; set; }
        public List<string> actors { get; set; }
    }
    
```

We first have to notify BrudexCodeFirst that we want to add the above to classes to be migrated with the following code.

```
Bcf<Director>.Migrate();
Bcf<Actor>.Migrate();

//Default connection is read from the web.config set second parameter to false to if passing explicit connectionstring
BrudexCodeFirst.RunMigrations("DefaultConnection", true); //Always call this last
```


####Specify a primary key apart from the default Id
```
Bcf<Director>.Migrate().SetPrimaryKey("releaseNo",autoincrement:false);
Bcf<Director>.Migrate().SetPrimaryKey(f=>f.releaseNo ,autoincrement:false); //same as above 

//Don't create primary key. don't autoincrement
Bcf<Actor>.Migrate().NoPrimaryKey(disableautoincrement:false); 
BrudexCodeFirst.RunMigrations("DefaultConnection", true);//Call this last to run db commands
```


####Other chainable commands
```
Bcf<Director>.Migrate().DisableAutoIncrementID() //Disable auto-increment but still create primary key
Bcf<Director>.Migrate().IgnoreFieds("Unreleased"); //Ignore the Unreleased field when migrating
Bcf<Actor>.Migrate().CreateForeignKeyOn<Movie>(f => f.movieId, q => q.Id).CreateForeignKeyOn(f => f.DirectorId, "Director", "Id"); //Creating foreign key in refrencing Movie and another referencing Director
 
Bcf<Actor>.Migrate().CreateForeignKeyOn(f => f.DirectorId, "Director", "Id"); //create foreign key referencing Director
 
```

###What about Seeding 
```
Bcf<Actor>.Migrate().NoPrimaryKey(disableautoincrement:true).Seed(new List());  //
Bcf<Director>.Migrate().CreateForeignKeyOn (x=>;x.ActorId, "Actor","Id").Seed(new List()); //make sure you pass a non-empty list to the seed method, I was lazy
Bcf.Migrate().IgnoreFieds("Unreleased").Seed(new List()); //Fluent call breaks after the Seed or Autoseed methods so call them last in the chain if necessary
```

####Or if you are too ninja to write seeds
```
Bcf<Direcotr>.Migrate().AutoSeed(20);  Create 20 rows of director in the database
Bcf<Actor>.Migrate().AutoSeed(200);  Create 200 rows of actors may or may not be working with this director 
```