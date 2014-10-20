using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
 
namespace Brudex.CodeFirst
{
    public static class TypeHelpers
    {
        private static Dictionary<Type, DataType> _supportedTypes = new Dictionary<Type, DataType>
            {
                {typeof (byte),DataType.Byte},
                {typeof (sbyte),DataType.SByte},
                {typeof (short),DataType.Short},
                {typeof (ushort),DataType.Short},
                {typeof (int),DataType.Integer},
                {typeof (uint),DataType.Integer},
                {typeof (long),DataType.BigInteger},
                {typeof (ulong),DataType.BigInteger},
                {typeof (float),DataType.Float},
                {typeof (double),DataType.Decimal},
                {typeof (decimal),DataType.Decimal},
                {typeof (bool),DataType.Bolean},
                {typeof (string),DataType.String},
                {typeof (char),DataType.Char},
                {typeof (Guid),DataType.UniqIdentifier},
                {typeof (DateTime),DataType.Date},
                {typeof (DateTimeOffset),DataType.DateTimeOffset},
                {typeof (byte[]),DataType.ByteArray},
                {typeof (Enum),DataType.Short}
            };
        public static string GetTableName<T>()
        {
            Type t = typeof (T);
            return t.Name;
        }

        public static ColumnMap GetColumn<T>(Expression<Func<T, object>> lambda)
        {
            var  memberInfo = GetProperty(lambda) ;
            ColumnMap column=new ColumnMap();
            column.EnityName = GetTableName<T>();
            column.ColumnName = memberInfo.Name;
            Type t = memberInfo.GetFType();
            var datatype = DataType.Integer;
            if (!IsSupportedType(t, out datatype))
            {
                throw new ArgumentException("Property specified is not of native type");
            }
            column.FieldType = datatype;
            return column;
        }

        public static List<ColumnMap> GetFields<T>(bool includePrivate, string tableName,bool includeNonPrimitives )
        {
            var columns = new List<ColumnMap>();
            var fields = GetPropertiesAndfields<T>();
            var entityType = typeof (T);

            foreach (var v in fields)
            {
                if (v.IsPrivate &&   !includePrivate)
                {
                    continue;
                }
                var datatype = DataType.Integer;
                 if (!IsSupportedType(v.type, out datatype))
                 {
                     if (includeNonPrimitives)
                     {
                         datatype=DataType.String;
                     }
                     else
                     {
                         continue;
                     }
                     
                 }
                var col=new ColumnMap();
                col.ColumnName = v.Name;
                col.EnityTable = tableName;
                col.EnityName = entityType.Name;
                col.FieldType = datatype;
                col.IsPrivate = v.IsPrivate;
                columns.Add(col);
            }
            
            return columns;
        }

        private static List<VariablInfo> GetPropertiesAndfields<T>()
        {
            Type mytype = typeof(T);
            var fields = mytype.GetFields(BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance );
            List<VariablInfo> varInfos=new List<VariablInfo>(); 
            
            var properties = mytype.GetProperties(BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance );

             
            foreach (var field in fields)
            {
                
                 if(!field.Name.Contains("<"))
               {
                    var f = new VariablInfo();
                     f.Name = field.Name;
                     f.type = field.FieldType;
                    f.IsPrivate = field.IsPrivate;
                    varInfos.Add(f);
                }
                
            }
            foreach (var property in properties)
            {
                var f = new VariablInfo();
                f.Name = property.Name;
                f.type = property.PropertyType;
                f.IsPrivate = !((property.GetSetMethod() != null) || (property.GetGetMethod() != null));
                varInfos.Add(f);
            }
            return varInfos;
        }

        public static MemberInfo GetProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }
        

        private static bool IsSupportedType(Type type,out DataType dataType)
        {
            return _supportedTypes.TryGetValue(type, out dataType);
        }

        public static List<MemberInfo> GetMembers<T>(BindingFlags  b = BindingFlags.Public |
                      BindingFlags.Instance)
        {
            return GetMembers(typeof(T), b);
        }

        public static List<MemberInfo> GetMembers(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(type.GetFields(bindingAttr));
            targetMembers.AddRange(type.GetProperties(bindingAttr));
            return targetMembers;
        }

        public static Type GetFType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
            }
        }

        private class VariablInfo
        {
            public string Name { get; set; }
            public bool IsPrivate { get; set; }
            public bool IsPrimitive { get; set; }
            public Type type { get; set; }
        }



        // some logic borrowed from James Newton-King, http://www.newtonsoft.com
        public static void SetValue(this MemberInfo member, object property, object value)
        {
            if (member.MemberType == MemberTypes.Property)
                ((PropertyInfo)member).SetValue(property, value, null);
            else if (member.MemberType == MemberTypes.Field)
                ((FieldInfo)member).SetValue(property, value);
            else
                throw new Exception("Property must be of type FieldInfo or PropertyInfo");
        }

        public static object GetValue(this MemberInfo member, object property)
        {
            if (member.MemberType == MemberTypes.Property)
                return ((PropertyInfo)member).GetValue(property, null);
            else if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).GetValue(property);
            else
                throw new Exception("Property must be of type FieldInfo or PropertyInfo");
        }

        

        public static Dictionary<string,EntityVariable>  GetTypeValues<TEntity>(TEntity entity)
        {
            Dictionary<string, EntityVariable> variables = new Dictionary<string, EntityVariable>();

            List<MemberInfo> memberInfos = GetMembers<TEntity>();
            foreach (var memberInfo in memberInfos)
            {
                EntityVariable v=new EntityVariable();
                v.FieldName = memberInfo.Name;
                DataType dt=DataType.Integer;
                if (_supportedTypes.TryGetValue(memberInfo.GetFType(), out dt))
                {
                    v.FieldType = dt;
                }
                v.FieldValue = memberInfo.GetValue(entity);
                 variables[v.FieldName]=v;
            }
            return variables;
        }
    }
}
