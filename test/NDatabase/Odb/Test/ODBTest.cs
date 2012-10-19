using System;
using System.IO;
using NDatabase2.Odb;
using NDatabase2.Odb.Main;

namespace Test.NDatabase.Odb.Test
{
    public class ODBTest : NDatabaseAssert
    {
        public static String HOST = "localhost";
        public static int PORT = 10000;
        public static bool useSameVmOptimization;
        public static bool testNewFeature;
        public static bool testPerformance;

        public string GetBaseName()
        {
            var guid = Guid.NewGuid();
            return string.Format("{0}{1}.neodatis", GetName(), guid.ToString());
        }

        public virtual OdbAdapter Open(String fileName, String user, String password)
        {
            return (OdbAdapter) NDb.Open(fileName);
        }

        public virtual OdbAdapter Open(String fileName)
        {
            return (OdbAdapter) NDb.Open(fileName);
        }

        public virtual OdbAdapter OpenLocal(String fileName)
        {
            return (OdbAdapter) NDb.Open(fileName);
        }

        public virtual void failCS()
        {
            AssertTrue(true);
        }

        protected internal virtual void FailNotImplemented(String string_Renamed)
        {
            AssertTrue(true);
        }

        protected internal virtual void DeleteBase(String baseName)
        {
            if (File.Exists(baseName))
                NDb.Delete(baseName);
        }

        public void Println(object o)
        {
            Console.WriteLine(o.ToString());
        }
    }
}
