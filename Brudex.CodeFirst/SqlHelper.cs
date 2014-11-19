using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brudex.CodeFirst
{
    class SqlHelper
    {
        public static string GenerateSql(string table,List<ColumnMap> columns,bool createPrimaryKey,string primaryKeyField,bool autoincrement)
        {
            
            ColumnMap primary = columns.FirstOrDefault(c=>string.Equals(c.ColumnName.ToLower(),primaryKeyField.ToLower()));
            if (createPrimaryKey)
            {
                if (primary == null)
                {
                    throw new ArgumentException("Primary key field was not found in class (default is Id if not specified). Specify primary key field or set flag to Ignore");
                }
            }
            StringBuilder sb =new StringBuilder();
            
            foreach (var columnMap in columns)
            {

                bool IsPrimaryKey = false;
                if (createPrimaryKey)
                {
                    IsPrimaryKey = string.Equals(columnMap.ColumnName, primary.ColumnName);
                }
                 
                sb.Append(GetColumnSqlSnippet(columnMap,IsPrimaryKey,autoincrement));
                
            }
            
            var primarySql = "";
            if(createPrimaryKey && primary != null)
            {
                primarySql = GetPrimaryKeySnippet(primary,table);
            }
 
            string sql = string.Format(@"CREATE TABLE [dbo].[{0}]({1}{2}) ON [PRIMARY]",table,sb,primarySql);
            sb=new StringBuilder();
            sb.Append(sql);
 

            return sb.ToString();
        }

        private static string GetColumnSqlSnippet(ColumnMap column,  bool isPrimaryKey,bool autoincrement)
        {
            string IdentityIncrement = "";
            if (autoincrement)
            {
                if (column.FieldType == DataType.BigInteger || column.FieldType == DataType.Integer)
                {
                    IdentityIncrement = "IDENTITY(1,1) ";
                      
                }
               
            }
            string isNull = "NULL";
            if (isPrimaryKey)
            {
                isNull =   IdentityIncrement + "NOT NULL";
                if (column.FieldType == DataType.String)
                {
                    column.FieldType=DataType.KeyString;
                }
            }

            string sqlType = column.GetSqlType();
            if (autoincrement && IdentityIncrement!="")
            {
                if (column.ColumnName.ToLower() == "id")
                {
                    isNull =   IdentityIncrement + "NOT NULL" ;
                }
                  
            }
            string s = string.Format("[{0}] {1} {2},",column.ColumnName,sqlType,isNull);
            return s;

        }

        private static string GetPrimaryKeySnippet(ColumnMap column,string tableName)
        {
            string sql = string.Format(@"CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([{1}] ASC ) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
",tableName,column.ColumnName);
            return sql;
        }

        public static string GetPForeignKeySnippet(ForeignKey column)
        {
            return string.Format(@"ALTER TABLE {0} ADD CONSTRAINT FK_{0}_{2}_{3} FOREIGN KEY ( {1} ) REFERENCES [dbo].[{2}] ({3})",column.TableName,column.ColumnName,column.ReferenceTable,column.ReferenceField);
        }
         
        public static string GetSeedSql(Dictionary<string, EntityVariable> seedMapping, List<ColumnMap> columns,string TableName)
        {
            StringBuilder cellNames = new StringBuilder();
            StringBuilder cellValues = new StringBuilder();
            string lastpublicColumn = columns.Last(c => c.IsPrivate == false).ColumnName;
            for (int p = 0; p < columns.Count; p++)
            {
                if (!columns[p].IsPrivate)
                {
                    var col = columns[p];
                    var entityVar = new EntityVariable();
                    if (seedMapping.TryGetValue(col.ColumnName, out entityVar))
                    {
                        string comma = col.ColumnName == lastpublicColumn ? "" : ",";
                        cellNames.Append(string.Format("[{0}]{1}", entityVar.FieldName,comma));
                        //we do a switch case to determin how to format figures when inserting
                        switch (entityVar.FieldType)
                        {
                            case DataType.Float:
                            case DataType.Decimal:
                            case DataType.BigInteger:
                            case DataType.Integer:
                                cellValues.Append(string.Format("{0}{1}", entityVar.FieldValue.ToString(),comma));
                                break;
                            case DataType.Bolean:
                                int b = bool.Parse(entityVar.FieldValue.ToString()) == true ? 1 : 0;
                                 cellValues.Append(string.Format("{0}{1}", b,comma));
                                break;
                            case DataType.String:
                            case DataType.Char:
                                 cellValues.Append(string.Format("'{0}'{1}", entityVar.FieldValue,comma));
                                break;
                            case DataType.Date:
                                string date = ((DateTime) entityVar.FieldValue).ToString("yyyyMMdd HH:mm:ss");
                                cellValues.Append(string.Format("'{0}'{1}", date,comma));
                                break;
                            
                            case DataType.ByteArray:
                                byte[] mybytes = ((byte[]) entityVar.FieldValue);
                                string array = "0x" + BitConverter.ToString(mybytes).Replace("-", "");
                                cellValues.Append(string.Format("'{0}'{1}", array, comma));
                                break;
                            case DataType.Short:
                            case DataType.SByte:
                            case DataType.Byte:
                                 
                            cellValues.Append(string.Format("{0}{1}", entityVar.FieldValue,comma));
                                break;
                            case DataType.UniqIdentifier:
                                string g = ((Guid) entityVar.FieldValue).ToString();
                                cellValues.Append(string.Format("{0}{1}", g, comma));
                                break;
                            case DataType.DateTimeOffset:
                                string offset =
                                    ((DateTimeOffset) entityVar.FieldValue).ToString("yyyyMMdd HH:mm:ss zzz");
                                cellValues.Append(string.Format("{0}{1}", offset, comma));
                                break;
                        }
                      }
                        
                    }

                }             
            string sql = string.Format(@"INSERT INTO [dbo].[{0}] ({1})  VALUES  ({2}) ; ", TableName, cellNames , cellValues );
            return sql;
        }

        public static string DropTable(string tableName)
        {
            return string.Format(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{0}') AND type in (N'U')) DROP TABLE {0};", tableName);           
        }

        public static string GetAlterStatement(List<ColumnMap> oldtableschema, List<ColumnMap> freshtableschema)
        {
            var removedColumns = new List<string>();
            var freshColumns = new List<ColumnMap>();
            var changedColumns = new List<ColumnMap>();

            Action getTableChanges = () =>
                {
                    var newColumns =
                        freshtableschema.Select(f => f.ColumnName)
                                        .Except(oldtableschema.Select(c => c.ColumnName))
                                        .ToList();

                     removedColumns =
                        oldtableschema.Select(f => f.ColumnName)
                                      .Except(freshtableschema.Select(c => c.ColumnName))
                                      .ToList();
                    newColumns.ForEach(p =>
                        {
                            var c = freshtableschema.First(j => j.ColumnName == p);
                            freshColumns.Add(c);
                        });
                    freshtableschema.ForEach(f =>
                        {
                            var old = oldtableschema.FirstOrDefault(o => o.ColumnName == f.ColumnName);
                            if (old != null)
                            {
                                if (old.FieldType !=  f.FieldType)
                                {
                                    changedColumns.Add(f);
                                }
                            }
                        });
                    
                };
            getTableChanges();
            var tableName = freshtableschema.First().EnityTable;
            StringBuilder sb=new StringBuilder();

            sb.Append(DropColumns(tableName,removedColumns));
            sb.Append(AddColumns(tableName,freshColumns));
            sb.Append(AlterColumns(tableName,changedColumns));
            return sb.ToString(); //TODO TEST THIS METHOD
        }

        private static StringBuilder DropColumns(string tableName,List<string> removedColumns)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var removedColumn in removedColumns)
            {
                sb.AppendLine(string.Format("ALTER TABLE {0} DROP COLUMN {1};", tableName, removedColumn));
            }
            return sb;
        }


        private static StringBuilder AlterColumns(string tableName, List<ColumnMap> columns)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var column in columns)
            {
                sb.AppendLine(string.Format("ALTER TABLE {0} ALTER COLUMN {1} {2};", tableName, column.ColumnName,column.GetSqlType()));
            }
            return sb;
        } 
        
        private static StringBuilder AddColumns(string tableName,List<ColumnMap> addedColumns)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var column in addedColumns)
            {
                sb.AppendLine(string.Format("ALTER TABLE {0} ADD {1} {2};", tableName, column.ColumnName,column.GetSqlType()));
            }
            return sb;
        }

        public static string GetTableInformation(string tableName)
        {
            return
                string.Format(@"select column_name,data_type from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}' ;",
                              tableName);
        }

        public static string GetTopInformationSchema()
        {
            return
                string.Format(@"SELECT  TOP (5)  *  FROM  INFORMATION_SCHEMA.TABLES");
            
        }
    }
}
