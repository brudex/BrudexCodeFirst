using System;
using System.Collections.Generic;
using System.Linq.Expressions;
 
namespace Brudex.CodeFirst
{
    public interface IBaseMigrationActions<TEntity> : IMigration
   {
        void Seed(List<TEntity> seeds);
        void AutoSeed(int seedCount=10);
//        string IdField { get; set; }
//        bool AutoIncrementId { get; set; }
//        List<string> FiedsToIgnore { get; set; }
//        bool NoPrimaryKey { get; set; }
        
            
       IBaseMigrationActions<TEntity> IgnoreFields(List<string> fieldNames);
       IBaseMigrationActions<TEntity> SetPrimaryKey(string field, bool autoIncrement);
       IBaseMigrationActions<TEntity> SetPrimaryKey(Expression<Func<TEntity, object>> lambd, bool autoIncrement=true);
       IBaseMigrationActions<TEntity> CreateForeignKeyOn(Expression<Func<TEntity, object>> lambda, string foreignTable,
                                                    string foreignField);
       IBaseMigrationActions<TEntity> CreateForeignKeyOn<TTable>(Expression<Func<TEntity, object>> fieldName, Expression<Func<TTable, object>> referenceField);
       IBaseMigrationActions<TEntity> IgnoreField(Expression<Func<TEntity, object>> lambda);
   }
}

