using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test.VO.Arraycollectionmap;

namespace Test.Odb.Test.Arraycollectionmap
{
    [TestFixture]
    public class TestArray : ODBTest
    {
        [Test]
        public virtual void TestArray1()
        {
            IOdb odb = null;
            try
            {
                DeleteBase("array1.neodatis");
                odb = Open("array1.neodatis");
                decimal nb = odb.Count(new CriteriaQuery(typeof (PlayerWithArray)));
                var player = new PlayerWithArray("kiko");
                player.AddGame("volley-ball");
                player.AddGame("squash");
                player.AddGame("tennis");
                player.AddGame("ping-pong");
                odb.Store(player);
                odb.Close();
                odb = Open("array1.neodatis");
                var l = odb.GetObjects<PlayerWithArray>(true);
                AssertEquals(nb + 1, l.Count);
                // gets first player
                var player2 = l.GetFirst();
                AssertEquals(player.ToString(), player2.ToString());
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                DeleteBase("array1.neodatis");
            }
        }

        [Test]
        public virtual void TestArray2()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                DeleteBase("array2.neodatis");
                odb = Open("array2.neodatis");
                var intArray = new int[size];
                for (var i = 0; i < size; i++)
                    intArray[i] = i;
                var owna = new ObjectWithNativeArrayOfInt("t1", intArray);
                odb.Store(owna);
                odb.Close();
                odb = Open("array2.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfInt>();
                var owna2 = l.GetFirst();
                AssertEquals(owna.GetName(), owna2.GetName());
                for (var i = 0; i < size; i++)
                    AssertEquals(owna.GetNumbers()[i], owna2.GetNumbers()[i]);
                odb.Close();
                odb = null;
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                DeleteBase("array2.neodatis");
            }
        }

        [Test]
        public virtual void TestArray3()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                DeleteBase("array3.neodatis");
                odb = Open("array3.neodatis");
                var array = new short[size];
                for (var i = 0; i < size; i++)
                    array[i] = (short) i;
                var owna = new ObjectWithNativeArrayOfShort("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array3.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfShort>();
                var owna2 = l.GetFirst();
                AssertEquals(owna.GetName(), owna2.GetName());
                for (var i = 0; i < size; i++)
                    AssertEquals(owna.GetNumbers()[i], owna2.GetNumbers()[i]);
                odb.Close();
                odb = null;
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                DeleteBase("array3.neodatis");
            }
        }

        [Test]
        public virtual void TestArray4()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                odb = Open("array5.neodatis");
                var array = new Decimal[size];
                for (var i = 0; i < size; i++)
                    array[i] = new Decimal(((double) i) * 78954545 / 89);
                var owna = new ObjectWithNativeArrayOfBigDecimal("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array5.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                var owna2 = l.GetFirst();
                AssertEquals(owna.GetName(), owna2.GetName());
                for (var i = 0; i < size; i++)
                    AssertEquals(owna.GetNumbers()[i], owna2.GetNumbers()[i]);
                odb.Close();
                odb = null;
                DeleteBase("array5.neodatis");
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
        }

        /// <summary>
        ///   Test update for array when the number of elements remains the
        ///   same
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestArray5()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                AbstractObjectWriter.ResetNbUpdates();
                DeleteBase("array7.neodatis");
                odb = Open("array7.neodatis");
                var array = new Decimal[size];
                for (var i = 0; i < size; i++)
                    array[i] = new Decimal(((double) i) * 78954545 / 89);
                var owna = new ObjectWithNativeArrayOfBigDecimal("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array7.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                var owna2 = l.GetFirst();
                owna2.SetNumber(0, new Decimal(1));
                odb.Store(owna2);
                odb.Close();
                odb = Open("array7.neodatis");
                l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                var o = l.GetFirst();
                AssertEquals(owna2.GetNumber(0), o.GetNumber(0));
                AssertEquals(owna2.GetNumber(1), o.GetNumber(1));
                AssertEquals(1, AbstractObjectWriter.GetNbNormalUpdates());

                odb.Close();
                DeleteBase("array7.neodatis");
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
        }

        /// <summary>
        ///   Test update for array when the number of elements remains the
        ///   same
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestArray6()
        {
            IOdb odb = null;
            var size = 2;
            try
            {
                AbstractObjectWriter.ResetNbUpdates();
                DeleteBase("array8.neodatis");
                odb = Open("array8.neodatis");
                var array = new int[size];
                for (var i = 0; i < size; i++)
                    array[i] = i;
                var owna = new ObjectWithNativeArrayOfInt("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array8.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfInt>();
                var owna2 = l.GetFirst();
                owna2.SetNumber(0, 1);
                odb.Store(owna2);
                odb.Close();
                odb = Open("array8.neodatis");
                l = odb.GetObjects<ObjectWithNativeArrayOfInt>();
                var o = l.GetFirst();
                AssertEquals(1, o.GetNumber(0));
                AssertEquals(1, o.GetNumber(1));
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                DeleteBase("array8.neodatis");
            }
        }

        /// <summary>
        ///   Test update for array when the number of elements remains the
        ///   same,but updating the second array element
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestArray61()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                AbstractObjectWriter.ResetNbUpdates();
                DeleteBase("array9.neodatis");
                odb = Open("array9.neodatis");
                var array = new int[size];
                for (var i = 0; i < size; i++)
                    array[i] = i;
                var owna = new ObjectWithNativeArrayOfInt("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array9.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfInt>();
                var owna2 = l.GetFirst();
                owna2.SetNumber(1, 78);
                odb.Store(owna2);
                odb.Close();
                odb = Open("array9.neodatis");
                l = odb.GetObjects<ObjectWithNativeArrayOfInt>();
                var o = l.GetFirst();
                AssertEquals(0, o.GetNumber(0));
                AssertEquals(78, o.GetNumber(1));
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                DeleteBase("array9.neodatis");
            }
        }

        /// <summary>
        ///   Increasing array size
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestArray6UpdateIncreasingArraySize()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                DeleteBase("array10.neodatis");
                odb = Open("array10.neodatis");
                var array = new Decimal[size];
                var array2 = new Decimal[size + 1];
                for (var i = 0; i < size; i++)
                {
                    array[i] = new Decimal(((double) i) * 78954545 / 89);
                    array2[i] = new Decimal(((double) i) * 78954545 / 89);
                }
                array2[size] = new Decimal(100);
                var owna = new ObjectWithNativeArrayOfBigDecimal("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array10.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                var owna2 = l.GetFirst();
                owna2.SetNumbers(array2);
                odb.Store(owna2);
                odb.Close();
                odb = Open("array10.neodatis");
                l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                var o = l.GetFirst();
                AssertEquals(size + 1, o.GetNumbers().Length);
                AssertEquals(new Decimal(100), o.GetNumber(size));
                AssertEquals(owna2.GetNumber(1), o.GetNumber(1));
                odb.Close();
                DeleteBase("array10.neodatis");
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
        }

        [Test]
        public virtual void TestArrayOfDate()
        {
            IOdb odb = null;
            var size = 50;
            try
            {
                DeleteBase("array6.neodatis");
                odb = Open("array6.neodatis");
                var array = new DateTime[size];
                var now = new DateTime();
                for (var i = 0; i < size; i++)
                    array[i] = new DateTime(now.Millisecond + i);
                var owna = new ObjectWithNativeArrayOfDate("t1", array);
                odb.Store(owna);
                odb.Close();
                odb = Open("array6.neodatis");
                var l = odb.GetObjects<ObjectWithNativeArrayOfDate>();
                var owna2 = l.GetFirst();
                AssertEquals(owna.GetName(), owna2.GetName());
                for (var i = 0; i < size; i++)
                    AssertEquals(owna.GetNumbers()[i], owna2.GetNumbers()[i]);
                odb.Close();
                odb = null;

                DeleteBase("array6.neodatis");
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
        }

        [Test]
        public virtual void TestArrayQuery()
        {
            IOdb odb = null;
            try
            {
                DeleteBase("array4.neodatis");
                odb = Open("array4.neodatis");
                decimal nb = odb.Count(new CriteriaQuery(typeof (PlayerWithArray)));
                var player = new PlayerWithArray("kiko");
                player.AddGame("volley-ball");
                player.AddGame("squash");
                player.AddGame("tennis");
                player.AddGame("ping-pong");
                odb.Store(player);
                odb.Close();
                odb = Open("array4.neodatis");
                var l = odb.GetObjects<PlayerWithArray>(new CriteriaQuery(Where.Contain("games", "tennis")));
                AssertEquals(nb + 1, l.Count);
            }
            catch (Exception e)
            {
                if (odb != null)
                {
                    odb.Rollback();
                    odb = null;
                }
                throw;
            }
            finally
            {
                if (odb != null)
                    odb.Close();
                DeleteBase("array4.neodatis");
            }
        }

        /// <summary>
        ///   Decreasing array size
        /// </summary>
        /// <exception cref="System.Exception">System.Exception</exception>
        [Test]
        public virtual void TestArrayUpdateDecreasingArraySize()
        {
            var size = 50;

            DeleteBase("array11.neodatis");

            var array = new Decimal[size];
            var array2 = new Decimal[size + 1];
            for (var i = 0; i < size; i++)
            {
                array[i] = new Decimal(((double) i) * 78954545 / 89);
                array2[i] = new Decimal(((double) i) * 78954545 / 89);
            }
            array[size - 1] = new Decimal(99);
            array2[size] = new Decimal(100);
            var owna = new ObjectWithNativeArrayOfBigDecimal("t1", array2);

            using (var odb = Open("array11.neodatis"))
            {
                odb.Store(owna);
            }

            IObjects<ObjectWithNativeArrayOfBigDecimal> l;
            ObjectWithNativeArrayOfBigDecimal owna2;
            using (var odb = Open("array11.neodatis"))
            {
                l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                owna2 = l.GetFirst();
                owna2.SetNumbers(array);
                odb.Store(owna2);
            }

            using (var odb = Open("array11.neodatis"))
            {
                l = odb.GetObjects<ObjectWithNativeArrayOfBigDecimal>();
                var o = l.GetFirst();
                AssertEquals(size, o.GetNumbers().Length);
                AssertEquals(new Decimal(99), o.GetNumber(size - 1));
                AssertEquals(owna2.GetNumber(1), o.GetNumber(1));
            }

            DeleteBase("array11.neodatis");
        }
    }
}
