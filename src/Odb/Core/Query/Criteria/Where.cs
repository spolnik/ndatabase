using System;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A simple factory to build all Criterion and Expression
    /// </summary>
    public static class Where
    {
        public static IConstraint InvariantEqual(string attributeName, string value)
        {
            return EqualCriterion<string>.CreateInvartiantStringEqualCriterion(attributeName, value, false);
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

        public static IConstraint InvariantLike(string attributeName, string value)
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
        public static IConstraint Contain<T>(string attributeName, T value)
        {
            return new ContainsCriterion<T>(attributeName, value);
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
