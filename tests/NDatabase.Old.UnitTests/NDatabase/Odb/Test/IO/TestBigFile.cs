using System;
using System.Collections.Generic;
using System.IO;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.IO
{
    [TestFixture]
    public class TestBigFile : ODBTest
    {
        private object GetUserInstance(int i)
        {
            var login = new VO.Login.Function("login" + i);
            var logout = new VO.Login.Function("logout" + i);
            IList<VO.Login.Function> list = new List<VO.Login.Function>();
            list.Add(login);
            list.Add(logout);
            var profile = new Profile("operator" + i, list);
            var user = new User("olivier smadja" + i, "olivier@neodatis.com", profile);
            return user;
        }

        [Test]
        [Ignore("Test big file, long test")]
        public virtual void T2estBigFileWithOdb()
        {
            var size1 = 10000;
            var size2 = 1000;
            var baseName = "big-file.ndb";
            IOdb odb = null;
            odb = Open(baseName);
            odb.Close();
            var z = 0;
            for (var i = 0; i < size1; i++)
            {
                odb = Open(baseName);
                for (var j = 0; j < size2; j++)
                {
                    odb.Store(GetUserInstance(j));
                    z++;
                }
                odb.Close();
                Println(i + "/" + size1 + " " + z + " objects");
            }
        }

        [Test]
        [Ignore("Test big file, long test")]
        public virtual void T2estBigFileWithOdbSelect()
        {
            var baseName = "big-file.ndb";
            IOdb odb = null;

            try
            {
                var start = OdbTime.GetCurrentTimeInMs();
                odb = Open(baseName);
                IQuery q = odb.Query<VO.Login.Function>();
                q.Descend("name").Constrain((object) "login10000").Equal();
                var functions = q.Execute<VO.Login.Function>(true, 0, 1);
                Console.Out.WriteLine(((IInternalQuery)q).GetExecutionPlan().GetDetails());
                Console.Out.WriteLine(functions.Count);
                Println(OdbTime.GetCurrentTimeInMs() - start + "ms");
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void Test1()
        {
            var raf = new FileStream("testBigFile", FileMode.OpenOrCreate);
            long l = 2 * 1024000;
            Println(l);
            raf.Seek(l, SeekOrigin.Begin);
            for (var i = 0; i < 1024000; i++)
                raf.Write(new byte[] {0}, 0, 1);
            raf.Write(new byte[] {0}, 0, 1);
            raf.Close();
        }
    }
}
