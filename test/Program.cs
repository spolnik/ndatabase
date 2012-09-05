﻿using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;

namespace Test
{
    public class Program
    {
        [Test]
        public void test1()
        {
            //OdbConfiguration.SetDebugEnabled(true);
            OdbConfiguration.SetReconnectObjectsToSession(false);
           
            try
            {
                string file = "Test.NDatabase";
                OdbFactory.Delete(file);
                IOdb odb = OdbFactory.Open(file);
                OID oid = odb.Store(new Function("f1"));
                odb.Close();
                Console.WriteLine("Write Done!");

                odb = OdbFactory.Open(file);
                IObjects<Function> functions = odb.GetObjects<Function>();
                Console.WriteLine(" Number of functions = " + functions.Count);
                Function f = (Function) odb.GetObjectFromId(oid);
                Console.WriteLine(f.ToString());
                odb.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

        [Test]
        public void test2()
        {
            //OdbConfiguration.SetDebugEnabled(true);
            OdbConfiguration.SetReconnectObjectsToSession(false);

            try
            {
                int size = 1000;
                string file = "Test.NDatabase";
                OdbFactory.Delete(file);
                IOdb odb = OdbFactory.Open(file);
                for (int i = 0; i < size; i++)
                {
                    OID oid = odb.Store(new Function("function " + i));
                }
                odb.Close();

                odb = OdbFactory.Open(file);
                IObjects<Function> functions = odb.GetObjects<Function>();
                Console.WriteLine(" Number of functions = " + functions.Count);
                
                odb.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

        [Test]
        public void test4()
        {
            //OdbConfiguration.SetDebugEnabled(true);
            OdbConfiguration.SetReconnectObjectsToSession(false);

            try
            {
                int size = 1000;
                string file = "Test.NDatabase";
                Console.WriteLine("Oi");
                OdbFactory.Delete(file);
                IOdb odb = OdbFactory.Open(file);
                for (int i = 0; i < size; i++)
                {
                    OID oid = odb.Store(new Function("function " + i));
                }
                odb.Close();

                odb = OdbFactory.Open(file);
                IObjects<Function> functions = odb.GetObjects<Function>(new CriteriaQuery(Where.Equal("name","function 199")));
                Console.WriteLine(" Number of functions = " + functions.Count);

                odb.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }
    }
}
