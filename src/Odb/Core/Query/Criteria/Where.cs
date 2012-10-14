using System;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A simple factory to build all Criterion and Expression
    /// </summary>
    public static class Where
    {
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The boolean value </param>
        /// <returns> The criteria </returns>
        public static IConstraint Equal(string attributeName, bool value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, int value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, short value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, byte value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, float value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, double value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, long value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, char value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Equal(string attributeName, object value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static IConstraint Iequal(string attributeName, char value)
        {
            return new EqualCriterion(attributeName, value, false);
        }

        public static IConstraint Iequal(string attributeName, object value)
        {
            return new EqualCriterion(attributeName, value, false);
        }

        /// <summary>
        ///   LIKE
        /// </summary>
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The string value </param>
        /// <returns> The criterio </returns>
        public static IConstraint Like(string attributeName, string value)
        {
            return new LikeCriterion(attributeName, value, true);
        }

        public static IConstraint Ilike(string attributeName, string value)
        {
            return new LikeCriterion(attributeName, value, false);
        }

        /// <summary>
        ///   GREATER THAN
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <returns> The criterion </returns>
        public static IConstraint GreaterThan<T>(string attributeName, T value) where T : IComparable
        {
            return new ComparisonCriterion<T>(attributeName, value, ComparisonCirerion.ComparisonTypeGt);
        }

        /// <summary>
        ///   GREATER OR EQUAL
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <returns> The criterion </returns>
        public static IConstraint GreaterOrEqual<T>(string attributeName, T value) where T : IComparable
        {
            return new ComparisonCriterion<T>(attributeName, value, ComparisonCirerion.ComparisonTypeGe);
        }

        /// <summary>
        ///   LESS THAN
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <returns> The criterion </returns>
        public static IConstraint LessThan<T>(string attributeName, T value) where T : IComparable
        {
            return new ComparisonCriterion<T>(attributeName, value, ComparisonCirerion.ComparisonTypeLt);
        }

        /// <summary>
        ///   LESS OR EQUAL
        /// </summary>
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The value </param>
        /// <returns> The criterion </returns>
        public static IConstraint LessOrEqual<T>(string attributeName, T value) where T : IComparable
        {
            return new ComparisonCriterion<T>(attributeName, value, ComparisonCirerion.ComparisonTypeLe);
        }

        /// <summary>
        ///   The
        /// </summary>
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The value </param>
        /// <returns> The criterion </returns>
        public static IConstraint Contain(string attributeName, bool value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, int value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, short value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, byte value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, float value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, double value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, long value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, char value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint Contain(string attributeName, object value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static IConstraint IsNull(string attributeName)
        {
            return new IsNullCriterion(attributeName);
        }

        public static IConstraint IsNotNull(string attributeName)
        {
            return new IsNotNullCriterion(attributeName);
        }

        public static IConstraint SizeEq(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeEq);
        }

        public static IConstraint SizeNe(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeNe);
        }

        public static IConstraint SizeGt(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeGt);
        }

        public static IConstraint SizeGe(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeGe);
        }

        public static IConstraint SizeLt(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeLt);
        }

        public static IConstraint SizeLe(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeLe);
        }
    }
}
