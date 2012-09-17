using System;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A simple factory to build all Criterion and Expression
    /// </summary>
    /// <author>olivier s</author>
    public static class Where
    {
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The boolean value </param>
        /// <returns> The criteria </returns>
        public static ICriterion Equal(string attributeName, bool value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, int value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, short value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, byte value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, float value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, double value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, long value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, char value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Equal(string attributeName, object value)
        {
            return new EqualCriterion(attributeName, value);
        }

        public static ICriterion Iequal(string attributeName, char value)
        {
            return new EqualCriterion(attributeName, value, false);
        }

        public static ICriterion Iequal(string attributeName, object value)
        {
            return new EqualCriterion(attributeName, value, false);
        }

        /// <summary>
        ///   LIKE
        /// </summary>
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The string value </param>
        /// <returns> The criterio </returns>
        public static ICriterion Like(string attributeName, string value)
        {
            return new LikeCriterion(attributeName, value, true);
        }

        public static ICriterion Ilike(string attributeName, string value)
        {
            return new LikeCriterion(attributeName, value, false);
        }

        /// <summary>
        ///   GREATER THAN
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <returns> The criterion </returns>
        public static ICriterion Gt(string attributeName, IComparable value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, int value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, short value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, byte value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, float value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, double value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, long value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        public static ICriterion Gt(string attributeName, char value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        /// <summary>
        ///   GREATER OR EQUAL
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <returns> The criterion </returns>
        public static ICriterion Ge(string attributeName, IComparable value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, int value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, short value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, byte value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, float value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, double value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, long value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);
        }

        public static ICriterion Ge(string attributeName, char value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);
        }

        /// <summary>
        ///   LESS THAN
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="value"> </param>
        /// <returns> The criterion </returns>
        public static ICriterion Lt(string attributeName, IComparable value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, int value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, short value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, byte value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, float value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, double value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, long value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        public static ICriterion Lt(string attributeName, char value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);
        }

        /// <summary>
        ///   LESS OR EQUAL
        /// </summary>
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The value </param>
        /// <returns> The criterion </returns>
        public static ICriterion Le(string attributeName, IComparable value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, int value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, short value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, byte value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, float value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, double value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, long value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        public static ICriterion Le(string attributeName, char value)
        {
            return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);
        }

        /// <summary>
        ///   The
        /// </summary>
        /// <param name="attributeName"> The attribute name </param>
        /// <param name="value"> The value </param>
        /// <returns> The criterion </returns>
        public static ICriterion Contain(string attributeName, bool value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, int value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, short value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, byte value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, float value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, double value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, long value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, char value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion Contain(string attributeName, object value)
        {
            return new ContainsCriterion(attributeName, value);
        }

        public static ICriterion IsNull(string attributeName)
        {
            return new IsNullCriterion(attributeName);
        }

        public static ICriterion IsNotNull(string attributeName)
        {
            return new IsNotNullCriterion(attributeName);
        }

        public static ICriterion SizeEq(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeEq);
        }

        public static ICriterion SizeNe(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeNe);
        }

        public static ICriterion SizeGt(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeGt);
        }

        public static ICriterion SizeGe(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeGe);
        }

        public static ICriterion SizeLt(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeLt);
        }

        public static ICriterion SizeLe(string attributeName, int size)
        {
            return new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeLe);
        }

        public static Or Or()
        {
            return new Or();
        }

        public static And And()
        {
            return new And();
        }

        public static Not Not(ICriterion criterion)
        {
            return new Not(criterion);
        }

        public static ICriterion Get(string attributeName, Operator @operator, string value)
        {
            if (@operator == Operator.Equal)
                return new EqualCriterion(attributeName, value);

            if (@operator == Operator.Like)
                return new LikeCriterion(attributeName, value, true);

            if (@operator == Operator.GreaterOrEqual)
                return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGe);

            if (@operator == Operator.GreaterThan)
                return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeGt);

            if (@operator == Operator.LessThan)
                return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLt);

            if (@operator == Operator.LessOrEqual)
                return new ComparisonCriterion(attributeName, value, ComparisonCriterion.ComparisonTypeLe);

            if (@operator == Operator.Contain)
                return new ContainsCriterion(attributeName, value);

            throw new OdbRuntimeException(NDatabaseError.QueryUnknownOperator.AddParameter(@operator.GetName()));
        }
    }
}
