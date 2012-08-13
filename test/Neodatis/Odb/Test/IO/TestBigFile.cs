using System.Collections.Generic;
using System.IO;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace IO
{
	[TestFixture]
    public class TestBigFile : ODBTest
	{
		/// <exception cref="System.IO.IOException"></exception>
		[Test]
        public virtual void Test1()
		{
			System.IO.FileStream raf = new System.IO.FileStream(ODBTest.Directory
				 + "testBigFile", FileMode.OpenOrCreate);
			long l = 2 * 1024000;
			Println(l);
			raf.Seek(l, SeekOrigin.Begin);
			for (int i = 0; i < 1024000; i++)
			{
				raf.Write(new byte[] {0}, 0, 1);
			}
            raf.Write(new byte[] { 0 }, 0, 1);
			raf.Close();
		}

		private object GetUserInstance(int i)
		{
			Function login = new Function
				("login" + i);
			Function logout = new Function
				("logout" + i);
			IList<Function> list = new List<Function>();
			list.Add(login);
			list.Add(logout);
			Profile profile = new Profile
				("operator" + i, list);
			User user = new User("olivier smadja"
				 + i, "olivier@neodatis.com", profile);
			return user;
		}

		[Test]
        [Ignore("Test big file, long test")]
		public virtual void T2estBigFileWithOdb()
		{
			int size1 = 10000;
			int size2 = 1000;
			string baseName = "big-file.neodatis";
			IOdb odb = null;
		    odb = Open(baseName);
		    odb.Close();
		    int z = 0;
		    for (int i = 0; i < size1; i++)
		    {
		        odb = Open(baseName);
		        for (int j = 0; j < size2; j++)
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
			string baseName = "big-file.neodatis";
			IOdb odb = null;
			
			try
			{
				long start = OdbTime.GetCurrentTimeInMs();
				odb = Open(baseName);
				IQuery q = new CriteriaQuery(typeof(Function), Where
					.Equal("name", "login10000"));
				var functions = odb.GetObjects<Function>(q, true, 0, 1);
				System.Console.Out.WriteLine(q.GetExecutionPlan().GetDetails());
				System.Console.Out.WriteLine(functions.Count);
                Println(OdbTime.GetCurrentTimeInMs() - start + "ms");
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
		}
	}
}
