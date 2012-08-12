using System;
using System.Threading;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer;
using NDatabase.Odb.Impl.Tool;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.IO;
using NUnit.Framework;

namespace Test.Odb.Test.Performance
{
    public class PerformanceTest1OnlySelect
    {
        public static int TestSize = 50000;

        public static readonly string OdbFileName = "perf-select.neodatis";

        [SetUp]
        public virtual void BuildBase()
        {
            var inMemory = true;
            // Deletes the database file
            OdbFile.DeleteFile(OdbFileName);
            long t1 = 0;
            long t2 = 0;
            long t3 = 0;
            long t4 = 0;
            long t5 = 0;
            long t6 = 0;
            long t7 = 0;
            long t77 = 0;
            long t8 = 0;
            IOdb odb = null;
            SimpleObject so = null;
            // Insert TEST_SIZE objects
            Console.Out.WriteLine("Inserting " + TestSize + " objects");
            t1 = OdbTime.GetCurrentTimeInTicks();
            odb = NDatabase.Odb.OdbFactory.Open(OdbFileName);
            for (var i = 0; i < TestSize; i++)
            {
                object o = GetSimpleObjectInstance(i);
                odb.Store(o);
                if (i % 10000 == 0)
                {
                    // System.out.println("i="+i);
                    MemoryMonitor.DisplayCurrentMemory(string.Empty + i, true);
                }
            }
            // System.out.println("Cache="+Dummy.getEngine(odb).getSession().getCache().toString());
            t2 = OdbTime.GetCurrentTimeInTicks();
            // Closes the database
            odb.Close();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestSelectSimpleObjectODB()
        {
            var t3 = OdbTime.GetCurrentTimeInTicks();
            var inMemory = true;
            Console.Out.WriteLine("Retrieving " + TestSize + " objects");
            // Reopen the database
            var odb = NDatabase.Odb.OdbFactory.Open(OdbFileName);
            // Gets the TEST_SIZE objects
            var l = odb.GetObjects<SimpleObject>(inMemory);
            Console.Out.WriteLine(l.GetType().FullName);
            var t4 = OdbTime.GetCurrentTimeInTicks();
            Console.Out.WriteLine("l.size=" + l.Count);
            var i = 0;
            while (l.HasNext())
            {
                object o = l.Next();
                if (i % 10000 == 0)
                    MemoryMonitor.DisplayCurrentMemory("select " + i, true);
                // System.out.println("Cache="+Dummy.getEngine(odb).getSession().getCache().toString());
                i++;
            }
            var t5 = OdbTime.GetCurrentTimeInTicks();
            odb.Close();
            DisplayResult("ODB " + TestSize + " SimpleObject objects ", t3, t4, t5);
            Console.Out.WriteLine("buffer Ok=" + MultiBufferedIO.NbBufferOk + " / buffer not ok =" +
                                  MultiBufferedIO.NbBufferNotOk);
            Console.Out.WriteLine("nb1=" + FileSystemInterface.NbCall1 + " / nb2 =" + FileSystemInterface.NbCall2);
        }

        private SimpleObject GetSimpleObjectInstance(int i)
        {
            var so = new SimpleObject();
            so.SetDate(new DateTime());
            so.SetDuration(i);
            so.SetName("Bonjour, comment allez vous?" + i);
            return so;
        }

        private void DisplayResult(string @string, long t1, long t2, long t3)
        {
            var s1 = " total=" + (t3 - t1);
            var s3 = " total select=" + (t3 - t1) + " -- " + "select=" + (t2 - t1) + " get=" + (t3 - t2);
            var s4 = " time/object=" + (float) (t3 - t1) / +TestSize;
            Console.Out.WriteLine(@string + s1 + " | " + s3 + " | " + s4);
        }

        /// <exception cref="System.Exception"></exception>
        public static void Main2(string[] args)
        {
            var pt = new PerformanceTest1OnlySelect();
            Thread.Sleep(20000);
            pt.TestSelectSimpleObjectODB();
        }
    }
}
