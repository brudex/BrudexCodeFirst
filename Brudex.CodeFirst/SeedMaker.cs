using System;
using System.Collections.Generic;
 

namespace Brudex.CodeFirst
{
    public  class SeedMaker
    {
        public static Dictionary<string, EntityVariable> CreateSeed(List<ColumnMap> columns)
        {
            var dict = new Dictionary<string, EntityVariable>();
            foreach (var column in columns)
            {
                EntityVariable ev=new EntityVariable();
                ev.FieldName = column.ColumnName;
                
                ev.FieldType = column.FieldType;
                ev.FieldValue = RandomFactory.GetRandomValue(column.ColumnName, column.FieldType);
                  
                dict[ev.FieldName] = ev;
            }

            return dict;
        }
    }

    public static class RandomFactory
    {
        private  static Random rnd=new Random(5);
        public static object GetRandomValue(string columnName, DataType dataType)
        {

            CollectionsFactory cf=new CollectionsFactory();

            object value=null;
            switch (dataType)
            {
                case DataType.Integer:
                case DataType.BigInteger:
                    value = rnd.NextInt32();
                    break;
                 
                case DataType.Bolean:
                    value = rnd.Next(2) == 0;
                    break;
                case DataType.Char:
                    value = rnd.NextChar();
                    break;
                case DataType.Date:
                    value = rnd.NextRandomDay();
                    break;
                case DataType.Decimal:
                    value = rnd.NextDecimal();
                    break;
                case DataType.String:
                    value = cf.GetRandomSeed(columnName,rnd);
                    break;
                case DataType.ByteArray:
                    PictureFiles pf=new PictureFiles();
                    int pictureNo = rnd.Next(1, 5);
                    value= pf.GetRandomPictureByteArray(pictureNo);
                    break;
                case DataType.SByte:
                    value = rnd.Next(sbyte.MaxValue);
                    break;
                case DataType.Byte:
                    value = rnd.Next(byte.MaxValue);
                    break;
                case DataType.Short:
                    value = rnd.Next(short.MaxValue);
                    break;
                case DataType.Float:
                    value = rnd.NextFloat();
                    break;
                case DataType.UniqIdentifier:
                    value = Guid.NewGuid();
                    break;
                case DataType.DateTimeOffset:
                    value = rnd.NextOffset();
                    break; 
            }

            return value;
        }

         
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string NextChar(this Random rnd)
        {           
               return _chars[rnd.Next(_chars.Length)].ToString();            
        }

        public  static DateTime NextRandomDay(this Random rnd)
        {
            DateTime start = new DateTime(1995, 1, 1);
            
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rnd.Next(range));
        }

        public static int NextInt32(this Random rnd)
        {
            unchecked
            {
                int firstBits = rnd.Next(0, 1 << 4) << 28;
                int lastBits = rnd.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

        public static decimal NextDecimal(this Random rnd)
        {
            byte scale = (byte)rnd.Next(29);
            bool sign = rnd.Next(2) == 1;
            return new decimal(rnd.NextInt32(),
                               rnd.NextInt32(),
                               rnd.NextInt32(),
                               sign,
                               scale);
        }

       public  static float NextFloat(this Random rnd)
        {
            double mantissa = (rnd.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, RandomFactory.rnd.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        private  static TimeSpan GetRandomTimeSpan()
        {
            return new TimeSpan(0, 0, 0, new Random().Next(86400));
        }

        public static DateTimeOffset NextOffset( this Random rnd)
        {
 
             TimeSpan span = GetRandomTimeSpan();
            long rand62bit = (((long)rnd.Next())<<31) + rnd.Next(); 
            DateTimeOffset offset=new DateTimeOffset(rand62bit,span) ;
            return offset;
        }
    }
}
