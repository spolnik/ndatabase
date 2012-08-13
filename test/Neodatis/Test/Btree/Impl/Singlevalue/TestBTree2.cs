using System;
using System.Collections;
using NDatabase.Btree;
using NDatabase.Odb.Impl.Core.Btree;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;

namespace NeoDatis.Test.Btree.Impl.Singlevalue
{
    [TestFixture]
    public class TestBTree2 : ODBTest
    {
        internal long nbSolutions;

        internal long total;

        internal long nbFailures;

        internal long maxExecutions;

        internal string failureFileName;

        internal string successFileName;

        /// <exception cref="System.Exception"></exception>
        private void TestDelete(int[] numbers)
        {
            var btree = GetBTree(2);
            for (var i = 0; i < numbers.Length; i++)
                btree.Insert(numbers[i], "v " + numbers[i]);
            AssertEquals(numbers.Length, btree.GetSize());
            for (var i = 0; i < numbers.Length; i++)
                AssertEquals("v " + (i + 1), btree.Delete(i + 1, "v " + (i + 1)));
            AssertEquals(0, btree.GetSize());
            AssertEquals(1, btree.GetHeight());
            AssertEquals(0, btree.GetRoot().GetNbKeys());
            AssertEquals(0, btree.GetRoot().GetNbChildren());
        }

        /// <exception cref="System.Exception"></exception>
        public virtual void Te1stDeleteAll()
        {
            var time = OdbTime.GetCurrentTimeInMs();
            failureFileName = "btree-failures." + time + ".txt";
            successFileName = "btree-successes." + time + ".txt";
            var size = 10000;
            total = Factorial(size);
            var array = new int[size];
            IList elements = new ArrayList();
            for (var i = 0; i < size; i++)
                elements.Add(i + 1);
            BuildArray(array, elements, 0);
            Println(nbSolutions + " solutions");
        }

        private long Factorial(int value)
        {
            if (value == 1)
                return value;
            return value * Factorial(value - 1);
        }

        /// <exception cref="System.Exception"></exception>
        internal virtual void BuildArray(int[] array, IList elements, int currentPosition)
        {
            if (elements.Count == 0)
                Post(array);
            for (var i = 0; i < elements.Count; i++)
            {
                IList myElements = new ArrayList(elements);
                var e = (int) myElements[i];
                myElements.Remove(i);
                array[currentPosition] = e;
                BuildArray(array, myElements, currentPosition + 1);
            }
        }

        /// <exception cref="System.Exception"></exception>
        internal virtual void BuildRandomArray(int[] array, IList elements, int currentPosition, int nbPossibilites)
        {
            if (elements.Count == 0)
                Post(array);
            for (var i = 0; i < nbPossibilites && i < elements.Count && nbSolutions < maxExecutions; i++)
            {
                IList myElements = new ArrayList(elements);
                var elementToPick = (OdbRandom.GetRandomInteger() * elements.Count);
                var e = (int) myElements[elementToPick];
                myElements.Remove(elementToPick);
                array[currentPosition] = e;
                BuildRandomArray(array, myElements, currentPosition + 1, nbPossibilites);
            }
        }

        /// <exception cref="System.IO.IOException"></exception>
        private void Post(int[] array)
        {
            Console.WriteLine(DisplayUtility.ObjectArrayToString(new object[] {array}));
            TestDelete(array);

            nbSolutions++;
            if (nbSolutions % 1000 == 0)
                Println(nbSolutions + "/" + total + "  -  " + nbFailures + " failures");
        }

        private IBTree GetBTree(int degree)
        {
            return new OdbBtreeSingle();
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestDeleteAllRandom()
        {
            var time = OdbTime.GetCurrentTimeInMs();
            failureFileName = "btree-failures." + time + ".txt";
            successFileName = "btree-successes." + time + ".txt";
            var size = 1000;
            maxExecutions = 100;
            var nbPossibilities = 3;
            total = (long) Math.Pow(nbPossibilities, size);
            var array = new int[size];
            IList elements = new ArrayList();
            for (var i = 0; i < size; i++)
                elements.Add(i + 1);
            // buildArray(array, elements, 0);
            BuildRandomArray(array, elements, 0, nbPossibilities);
            Println(nbSolutions + " solutions");
            AssertEquals(0, nbFailures);
        }
    }
}
