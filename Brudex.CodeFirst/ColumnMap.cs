

namespace Brudex.CodeFirst
{
    public class ColumnMap
    {
        public string EnityName { get; set; }
        public string EnityTable { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrivate { get; set; }
        public DataType FieldType { get; set; }

        public string GetSqlType()
        {
            string sqlType = "";
            switch (FieldType)
            {
                    case DataType.BigInteger:
                    sqlType = "[bigint]";
                    break;
                    case DataType.Integer:
                    sqlType = "[int]";
                    break;
                    case DataType.Bolean:
                    sqlType = "[bit]";
                    break;
                    case DataType.Char:
                    sqlType = "[char](10)";
                    break;
                    case DataType.Date:
                    sqlType = "[datetime]";
                    break;
                    case DataType.Decimal:
                    sqlType = "[decimal](18, 0)";
                    break;
                    case DataType.String:
                    sqlType = "[nvarchar](max)";
                    break;
                    case DataType.KeyString:
                    sqlType = "[varchar](250)";
                    break;
                    case DataType.ByteArray:
                    sqlType = "[varbinary](maxlength)";
                    break;
                    case DataType.SByte:
                    case DataType.Byte:
                    sqlType = "[tinyint]";
                    break; 
                    case DataType.Short:
                    sqlType = "[smallint]";
                    break;
                    case DataType.Float:
                    sqlType = "[real]";
                    break;
                    case DataType.UniqIdentifier:
                    sqlType = "[uniqueidentifier]";
                    break; 
                     case DataType.DateTimeOffset:
                    sqlType = "[datetimeoffset](7)";
                    break; 
                    

            }
            return sqlType;
        }

        public void SetFieldType(string fieldType)
        {
            string sqlType = fieldType.ToLower();
            var dataType = DataType.String;
            switch (sqlType)
            {
                case  "bigint":
                    dataType = DataType.BigInteger;
                    
                    break;
                case "int" :
                    dataType = DataType.Integer;
                    break;
                case "bit":
                    dataType=DataType.Bolean;
                    break;
                case "char":
                    dataType=DataType.Char;
                    
                    break;
                case "datetime":
                    dataType = DataType.Date;                   
                    break;
                case "decimal":
                    dataType = DataType.Decimal;                   
                    break;
                case "nvarchar":
                    dataType = DataType.String;                 
                    break;
                case "varchar":
                    dataType = DataType.KeyString;
                    break;
                case "varbinary":
                    dataType = DataType.ByteArray;
                    break;
                case "tinyint":
                    dataType = DataType.SByte;
                    break;
                case  "smallint":
                    dataType = DataType.Short;
                    
                    break;
                case "real":
                    dataType = DataType.Float;
                    
                    break;
                case "uniqueidentifier":
                    dataType = DataType.UniqIdentifier;
                    break;
                case  "datetimeoffset":
                    dataType = DataType.DateTimeOffset;
                    
                    break;


            }
            this.FieldType= dataType;
        }
    }
}
