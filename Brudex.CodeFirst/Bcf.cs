 

namespace Brudex.CodeFirst
{
    public  class Bcf<TEntity> :BrudexCodeFirst where TEntity :class 
    {
        public static IBaseMigrationActions<TEntity> Migrate(bool includeNonPrimitives=false)
        {
            string tableName = TypeHelpers.GetTableName<TEntity>();
            IBaseMigrationActions<TEntity> entity=new BaseMigrationActions<TEntity>(tableName,false,includeNonePrimitives:includeNonPrimitives);
            migrations.Add(entity);
            return entity;
        }

        
        public static IBaseMigrationActions<TEntity> Migrate(bool noPrimaryKey, bool autoIncrementId)
        {
            string tableName = TypeHelpers.GetTableName<TEntity>();
            IBaseMigrationActions<TEntity> entity = new BaseMigrationActions<TEntity>(tableName, false, noPrimaryKey, autoIncrementId);
            migrations.Add(entity);
            return entity;
        } 
        
        public static IBaseMigrationActions<TEntity> Migrate(bool noPrimaryKey, bool autoIncrementId,bool includeNonePrimitives)
        {
            string tableName = TypeHelpers.GetTableName<TEntity>();
            IBaseMigrationActions<TEntity> entity = new BaseMigrationActions<TEntity>(tableName, false, noPrimaryKey,includeNonePrimitives, autoIncrementId);
            migrations.Add(entity);
            return entity;
        } 
        
        public static IBaseMigrationActions<TEntity> Migrate(string tableName,bool includePrivateFields=false,bool includeNonPrimitives=false)
        { 
            IBaseMigrationActions<TEntity> entity=new BaseMigrationActions<TEntity>(tableName,includePrivateFields,includeNonePrimitives:includeNonPrimitives);
            migrations.Add(entity);
            return entity;
        }
        
        public static IBaseMigrationActions<TEntity> Migrate(string tableName,bool includePrivateFields,bool includeNonePrimitives, bool noPrimaryKey,bool autoIncrementId)
        { 
            IBaseMigrationActions<TEntity> entity=new BaseMigrationActions<TEntity>(tableName,includePrivateFields,noPrimaryKey,includeNonePrimitives,autoIncrementId);
            migrations.Add(entity);
            return entity;
        }

            
        public new static  int GetMigrationCount()
        {
            return migrations.Count;
        }

        
    }

   
}
