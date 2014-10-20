using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExploringTypes
{
    class Program
    {
        class NestedClass
        {
            public class DoubleNestedClass
            {
                // Nested class contents ...
            }
        }

        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = null;
            try
            {
                fs = File.OpenRead(fullFilePath);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                return bytes;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

        }
        static void Main(string[] args)
        {

            var bts = GetBytesFromFile(@"C:\Users\Penrose\Desktop\file4-s.jpg");
            string base64String = Convert.ToBase64String(bts);
            File.WriteAllText(@"C:\Users\Penrose\Desktop\file4-s.txt",base64String);
//            Type guyType = typeof(Guy);
//            Console.WriteLine("{0} extends {1}",
//                guyType.FullName,
//                guyType.BaseType.FullName);
//
//            var props = guyType.GetProperties(BindingFlags.Public |
//                BindingFlags.NonPublic | BindingFlags.Instance);
//            var fields = guyType.GetFields();
//            
//            Console.WriteLine("The fields are :");
//            foreach (var fieldInfo in fields)
//            {
//                Console.WriteLine("Name : "+fieldInfo.Name+" The type is: "+fieldInfo.FieldType);
//                
//            }
//
//            Console.WriteLine("The Properties are :");
//            foreach (var prop in props)
//            {
//                Console.WriteLine("Name : "+prop.Name+" The type is: "+((prop.GetSetMethod()!=null) || (prop.GetGetMethod()!=null)));
//                
//            }


//            Type nestedClassType = typeof(NestedClass.DoubleNestedClass);
//            Console.WriteLine(nestedClassType.FullName);
//
//            List<Guy> guyList = new List<Guy>();
//            Console.WriteLine(guyList.GetType().Name);
//
//            Dictionary<string, Guy> guyDictionary = new Dictionary<string, Guy>();
//            Console.WriteLine("Get Dictionary Name: "+guyDictionary.GetType().Name);
//
//            Type t = typeof(Program);
//            Console.WriteLine("Program Name: "+t.FullName);
//
//            Type intType = typeof(int);
//            Type int32Type = typeof(Int32);
//            Console.WriteLine("{0} - {1}", intType.FullName, int32Type.FullName);
//            Console.WriteLine("Generic type definition: - {0}", guyList.GetType().GetGenericTypeDefinition());
//
//            Console.WriteLine("{0} {1}", float.MinValue, float.MaxValue);
//            Console.WriteLine("{0} {1}", int.MinValue, int.MaxValue);
//            Console.WriteLine("{0} {1}", DateTime.MinValue, DateTime.MaxValue);

            Console.ReadLine();
        }
    }

    class Guy
    {
        /// <summary>
        /// Read-only backing field for the Name property
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The name of the guy
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Read-only backing field for the Name property
        /// </summary>
        private readonly int age;

        /// <summary>
        /// The guy's age
        /// </summary>
        public int Age { get { return age; } }

        /// <summary>
        /// The number of bucks the guy has
        /// </summary>
        public int Cash { get; private set; }

        public string JustAfield;
        public string JustAProperty { get; set; }
        private string AprivateProperty { get; set; }
        private string AprivatField;
        /// <summary>
        /// The constructor sets the name, age and cash
        /// </summary>
        /// <param name="name">The name of the guy</param>
        /// <param name="age">The guy's age</param>
        /// <param name="cash">The amount of cash the guy starts with</param>
        public Guy(string name, int age, int cash)
        {
            this.name = name;
            this.age = age;
            Cash = cash;
        }

        public override string ToString()
        {
            return String.Format("{0} is {1} years old and has {2} bucks", Name, Age, Cash);
        }

        /// <summary>
        /// Give cash from my wallet
        /// </summary>
        /// <param name="amount">The amount of cash to give</param>
        /// <returns>The amount of cash I gave, or 0 if I don't have enough cash</returns>
        public int GiveCash(int amount)
        {
            if (amount <= Cash && amount > 0)
            {
                Cash -= amount;
                return amount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Receive some cash into my wallet
        /// </summary>
        /// <param name="amount">Amount to receive</param>
        /// <returns>The amount of cash received, or 0 if no cash was received</returns>
        public int ReceiveCash(int amount)
        {
            if (amount > 0)
            {
                if (amount > 0)
                {
                    Cash += amount;
                    return amount;
                }
                Console.WriteLine("{0} says: {1} isn't an amount I'll take", Name, amount);
            }
            return 0;
        }
    }
}
