using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;

namespace Dotnet
{
    [TestFixture]
    public class TestBTree
    {
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        public virtual void RunJava()
        {
            IOdb odb = null;
            var test = new ODBTest();
            try
            {
                test.DeleteBase("mydb7.neodatis");
                // Open the database
                odb = test.Open("mydb7.neodatis");
                var start0 = OdbTime.GetCurrentTimeInMs();
                var nRecords = 100000;
                for (var i = 0; i < nRecords; i++)
                {
                    var ao = new AA();
                    ao.ccc = "csdcsdc";
                    ao.ww = i;
                    odb.Store(ao);
                }
                var end0 = OdbTime.GetCurrentTimeInMs();
                IQuery query = new CriteriaQuery(typeof (AA));
                query.OrderByAsc("ww");
                var start = OdbTime.GetCurrentTimeInMs();
                var object12 = odb.GetObjects<AA>(query, false);
                while (object12.HasNext())
                {
                    var s = object12.Next();
                    var id = s.ww;
                }
                // println(id);
                var end = OdbTime.GetCurrentTimeInMs();
                test.Println("Time=" + (end - start) + " / " + (end - start0) + " / " + (end0 - start0));
            }
            finally
            {
                if (odb != null)
                {
                    // Close the database
                    odb.Close();
                }
            }
        }
    }

    internal class AA
    {
        public string ccc;

        public int ww;
    }
}
