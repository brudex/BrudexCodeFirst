 

namespace Brudex.CodeFirst
{
    public interface IMigration
    {
        string GetCreateSql();
        string GetForeignKeySql();
        string GetSeedSql();
        string DropExistingTable();//Drop if exists
        bool ModelChanged(ConnectionFactory connection, out string alterStatementSql);
        bool ModelChanged(ConnectionFactory connection);
        bool IsFirstMigration(ConnectionFactory connection);
    }

    
}
