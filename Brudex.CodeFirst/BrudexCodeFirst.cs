using System;
using System.Collections.Generic;
using System.Text;

namespace Brudex.CodeFirst
{
    public class BrudexCodeFirst
    {
        private static  string _conString;
        protected BrudexCodeFirst()
        {}
        
        protected static List<IMigration> migrations = new List<IMigration>();

        public static int GetMigrationCount()
        {
            return migrations.Count;
        }

        public static void RunMigrations(string connectionString, bool readFromConfigurationFile = false,ConnectionType dbEngine=ConnectionType.SqlServer,MigrationOptions options=MigrationOptions.MigrateOnce)
        {
            if (readFromConfigurationFile)
            {
                _conString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
            }
            else
            {
                _conString = connectionString;
            }
            var sb = new StringBuilder();

            var cf = new ConnectionFactory(dbEngine,_conString);
             
            Action doMigrations = () =>
                {
                    foreach (var migration in migrations)
                    {
                        switch (options)
                        {
                            case MigrationOptions.RecreateEveryTime:
                                 sb.Append(migration.DropExistingTable());
                                var sql = migration.GetCreateSql();
                                sb.Append(sql);
                                sb.AppendLine(migration.GetSeedSql());                              
                                break; 
                            case MigrationOptions.RecreateOnlyChangedModels:
                                if (migration.ModelChanged(cf))
                                {
                                    sb.Append(migration.DropExistingTable());
                                    sb.Append(migration.GetCreateSql());
                                    sb.AppendLine(migration.GetSeedSql()); 
                                }                                  
                                break;
                            case MigrationOptions.AlterOnModelChanges:

                                if (migration.IsFirstMigration(cf))
                                {
                                    sb.Append(migration.GetCreateSql());
                                    sb.AppendLine(migration.GetSeedSql()); 
                                }
                                else
                                {
                                    string alterStatement;
                                    if (migration.ModelChanged(cf, out alterStatement))
                                    {
                                        sb.Append(alterStatement);
                                    }
                                    
                                }                               
                                break;
                            default:
                                sb.Append(migration.GetCreateSql());
                                break;
                        }
                        
                        sb.Append("; " + Environment.NewLine);
                    }

                    cf.ExecuteCommand(sb.ToString());
                };
             
            if (options == MigrationOptions.MigrateOnce)
            {
                var tableNames = new List<string>();
                migrations.ForEach(t => tableNames.Add(t.GetTableName()));
                if (cf.IsFirstMigrations(tableNames))
                {
                    doMigrations();
                }
                else
                {
                    migrations.Clear();
                }
                
            }
            else
            {
                doMigrations();
            }
             
        } 
        
        public static void RunMigrations()
        {
          _conString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
          RunMigrations(_conString,true);

        }

        public static string GetSqlQuery()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var migration in migrations)
            {
                sb.Append(migration.GetCreateSql());
                sb.Append(Environment.NewLine + " ; " + Environment.NewLine);
            }
            return sb.ToString();
        }

         
        public static void ClearMigrations()
        {
            migrations.Clear();
        }

//        public static void AddMigration<TEntity>(IBaseMigrationActions<TEntity> migration) where TEntity : class
//        {
//            migrations.Add(migration);
//        }
        
        public static void AddMigration(IMigration migration)
        {
            migrations.Add(migration);
        }

        public static IBaseMigrationActions<TEntity> GetMigration<TEntity>(int index) where TEntity : class
        {
            return (IBaseMigrationActions<TEntity>)migrations[index];
        }

    }

    

    public enum ConnectionType
    {
        SqlServer,MySql,Oracle,Postgres
    }
}
