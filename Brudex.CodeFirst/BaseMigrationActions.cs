using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Brudex.CodeFirst
{
  public  class BaseMigrationActions<TEntity> :IBaseMigrationActions<TEntity> where TEntity:class 
    {

        public string IdField { get; private set; }
        public bool AutoIncrementId { get; private set; }
        public List<string> FiedsToIgnore { get; private set; }
        public bool NoPrimaryKey { get; private set; }
        public string TableName { get;  private set; }
        public List<ColumnMap> Columns { get; private set; }
        public List<ColumnMap> _oldColumnsSchema { get; private set; }
        public List<ForeignKey> ForeignKeys { get; private set; }
        public bool HasSeeds {   get; private set; }
        public List<Dictionary<string, EntityVariable>> SeedMappings { get; private set; }
        public bool IncludeNonPrimitives { get; set; }
      
       public BaseMigrationActions(string tableName,bool incldePrivateFields, bool noPrimaryKey=false,bool includeNonePrimitives=false, bool autoIncrementId=true)
       {
           TableName = tableName;
           FiedsToIgnore=new List<string>();
           IdField = "Id";
           IncludeNonPrimitives = includeNonePrimitives;
           NoPrimaryKey = noPrimaryKey;
           AutoIncrementId = autoIncrementId;
           Columns = TypeHelpers.GetFields<TEntity>(incldePrivateFields, TableName, includeNonePrimitives);
           ForeignKeys = new List<ForeignKey>();
           SeedMappings = new List<Dictionary<string, EntityVariable>>();
       } 

      public string GetTableName()
      {
          return TableName;
      }

        public void Seed(List<TEntity> seeds)
        {
            HasSeeds = true;
            foreach (var seed in seeds)
            {
              var  seedMap = TypeHelpers.GetTypeValues(seed);
                var s = SanitizeSeedMap(seedMap);
                SeedMappings.Add(s);
            }           
        }


      private Dictionary<string, EntityVariable> SanitizeSeedMap(Dictionary<string, EntityVariable> seedMap)
      {
          if (AutoIncrementId)
          {
              ColumnMap c = Columns.FirstOrDefault(x => x.ColumnName == IdField);
              if (c != null)
              {
                  var IsAutoIncrementable = c.FieldType == DataType.BigInteger ||
                                         c.FieldType == DataType.Integer || c.FieldType == DataType.Decimal ||
                                         c.FieldType == DataType.Short;
                  if (IsAutoIncrementable)
                  {
                      seedMap.Remove(c.ColumnName);
                  }
              }
          }
          return seedMap;
      }

        public void AutoSeed(int seedCount=10)
        {
            HasSeeds = true;
            for (int i = 0; i < seedCount; i++)
            {
                Dictionary<string, EntityVariable> seed = SeedMaker.CreateSeed(Columns);
                SeedMappings.Add(SanitizeSeedMap(seed));
            }
        }

        public IBaseMigrationActions<TEntity> IgnoreFields(List<string> fieldNames)
        {
            FiedsToIgnore.AddRange(fieldNames);
            return this;
        }

        public IBaseMigrationActions<TEntity> SetPrimaryKey(string field, bool autoIncrement)
        {
            IdField = field;
            AutoIncrementId = autoIncrement;
            return this;
        }

        public IBaseMigrationActions<TEntity> SetPrimaryKey(Expression<Func<TEntity, object>> lambda, bool autoIncrement=true)
        {
            ColumnMap column = TypeHelpers.GetColumn<TEntity>(lambda);
            return SetPrimaryKey(column.ColumnName, autoIncrement);
        }

        public IBaseMigrationActions<TEntity> CreateForeignKeyOn(Expression<Func<TEntity, object>> lambda, string foreignTable,string foreignField)
        {
            ForeignKey foreignKey=new ForeignKey();
            ColumnMap column = TypeHelpers.GetColumn<TEntity>(lambda);
            foreignKey.TableName = this.TableName;
            foreignKey.ReferenceTable = foreignTable;
            foreignKey.ReferenceField = foreignField;
            foreignKey.ColumnName = column.ColumnName;
            if (!ForeignKeyAdded(foreignKey))
            ForeignKeys.Add(foreignKey);
            return this;
        }

        public IBaseMigrationActions<TEntity> CreateForeignKeyOn<TForeignClass>(Expression<Func<TEntity, object>> fieldName, Expression<Func<TForeignClass, object>> referenceField)
        {
            ForeignKey foreignKey = new ForeignKey();
            ColumnMap currentTable = TypeHelpers.GetColumn<TEntity>(fieldName);
            ColumnMap referenceTable = TypeHelpers.GetColumn<TForeignClass>(referenceField);
            foreignKey.TableName = this.TableName;
            foreignKey.ReferenceTable = referenceTable.EnityName;
            foreignKey.ReferenceField = referenceTable.ColumnName;
            foreignKey.ColumnName = currentTable.ColumnName;
            if(!ForeignKeyAdded(foreignKey))
            ForeignKeys.Add(foreignKey);
            return this;   
        }

      private bool ForeignKeyAdded(ForeignKey column)
      {
          foreach (var foreignKey in ForeignKeys)
          {
              if (foreignKey.ReferenceTable == column.ReferenceTable &&
                  foreignKey.ReferenceField == column.ReferenceField)
              {
                  return true;
              }
          }
          return false;
      }

        public IBaseMigrationActions<TEntity> IgnoreField(Expression<Func<TEntity, object>> lambda)
        {
            ColumnMap column = TypeHelpers.GetColumn<TEntity>(lambda);
            this.FiedsToIgnore.Add(column.ColumnName);
            return this;
        }

        public string GetCreateSql()
        {
            if (FiedsToIgnore.Contains(IdField))
            {
                throw new ArgumentException(string.Format("'{0}' is ignored and cannot be made a key field. ", IdField));
            }
            var allcolumns = Columns.ToList();
            foreach (var columnMap in allcolumns)
            {
                if (FiedsToIgnore.Contains(columnMap.ColumnName))
                {
                    Columns.Remove(columnMap);
                }
            }

            bool createPrimaryKey = !NoPrimaryKey;
            string s=SqlHelper.GenerateSql(TableName,Columns,createPrimaryKey,IdField,AutoIncrementId);
            StringBuilder sb=new StringBuilder(s);
            if (ForeignKeys.Count > 0)
            {
                sb.AppendLine(GetForeignKeySql());
            }
            return sb.ToString();
        }
       
      private string GetForeignKeySql()
      {
          StringBuilder sb = new StringBuilder();
          sb.Append("");
          foreach (var foreignKey in ForeignKeys)
          {
              sb.Append(SqlHelper.GetPForeignKeySnippet(foreignKey));
              sb.Append(";");
          }

          return sb.ToString();
      }

       public string GetSeedSql()
       {
           StringBuilder sb = new StringBuilder();
           foreach (var seedMapping in SeedMappings)
           {
               string singleSeedSql = SqlHelper.GetSeedSql(seedMapping,Columns,TableName);
               sb.AppendLine(singleSeedSql);
           }
           return sb.ToString();

       }

      public string DropExistingTable()
      {
          StringBuilder sb = new StringBuilder();
          sb.Append(SqlHelper.DropTable(TableName));
           sb.Append(Environment.NewLine);
           return sb.ToString();
      }

      

      public bool ModelChanged(ConnectionFactory connection,out string alterStatementSql)
      {
         var shemaChanged = ModelChanged(connection);
          alterStatementSql = string.Empty;
          if (shemaChanged)
          {
              alterStatementSql = SqlHelper.GetAlterStatement(_oldColumnsSchema, Columns);
          }
          return shemaChanged;

      }

      public bool ModelChanged(ConnectionFactory connection)
      {
          bool shemaChanged = false;
          Func<ColumnMap, ColumnMap, bool> schemaEqual = (oldschema, newschema) =>
              {
                  if (oldschema.FieldType == newschema.FieldType)
                  {
                      return true;
                  }
                  return false;
              };

          Func<List<ColumnMap>, List<ColumnMap>, bool> columnsRemoved = (oldschema, newschema) =>
              {
                  foreach (var columnMap in oldschema)
                  {
                      var nschema = newschema.FirstOrDefault(c => c.ColumnName == columnMap.ColumnName);
                      if (newschema == null)
                      {
                          return true;
                      }
                  }

                  return false;
              };

            _oldColumnsSchema = connection.GetTableInformation(TableName);
          foreach (var columnMap in Columns)
          {
              var oldschema = _oldColumnsSchema.FirstOrDefault(x => x.ColumnName == columnMap.ColumnName);
              if (oldschema == null)
              {
                  shemaChanged = true;
                  break;
              }
              else
              {
                  shemaChanged = schemaEqual(oldschema, columnMap);
                  if (shemaChanged)
                  {
                      break;
                  }
              }
          }

          if (!shemaChanged)
          {
              shemaChanged = columnsRemoved(_oldColumnsSchema, Columns);
          }
          return shemaChanged;
      }
       

      public bool IsFirstMigration(ConnectionFactory connection)
      {
         var list= connection.GetTableInformation(TableName);
          return list.Count > 0;

      }
      
     
    }
}
