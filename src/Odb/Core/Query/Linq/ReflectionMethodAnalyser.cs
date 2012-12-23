using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace NDatabase2.Odb.Core.Query.Linq
{
    internal class ReflectionMethodAnalyser
    {
        private static readonly ConcurrentDictionary<MethodInfo, FieldInfo> FieldCache =
            new ConcurrentDictionary<MethodInfo, FieldInfo>();

        private static readonly ILPattern GetterPattern =
            ILPattern.Sequence(
                BackingField(),
                ILPattern.Optional(
                    OpCodes.Stloc_0,
                    OpCodes.Br_S,
                    OpCodes.Ldloc_0),
                ILPattern.OpCode(OpCodes.Ret));

        private readonly MethodInfo _method;

        private ReflectionMethodAnalyser(MethodInfo method)
        {
            _method = method;
        }

        #region IMethodAnalyser Members

        public void Run(QueryBuilderRecorder recorder)
        {
            RecordField(recorder, GetBackingField(_method));
        }

        #endregion

        private static ILPattern BackingField()
        {
            return new BackingFieldPattern();
        }

        private static MatchContext MatchGetter(MethodBase method)
        {
            return ILPattern.Match(method, GetterPattern);
        }

        private static void RecordField(QueryBuilderRecorder recorder, FieldInfo field)
        {
            recorder.Add(ctx =>
                             {
                                 ctx.Descend(field.Name);
                                 ctx.PushDescendigFieldEnumType(field.FieldType.IsEnum ? field.FieldType : null);
                             });
        }

        private static FieldInfo GetBackingField(MethodInfo method)
        {
            return FieldCache.GetOrAdd(method, ResolveBackingField);
        }

        private static FieldInfo ResolveBackingField(MethodInfo method)
        {
            var context = MatchGetter(method);
            if (!context.IsMatch)
                throw new LinqQueryException("Analysed method is not a simple getter");

            return GetFieldFromContext(context);
        }

        private static FieldInfo GetFieldFromContext(MatchContext context)
        {
            object data;
            if (!context.TryGetData(BackingFieldPattern.BackingFieldKey, out data))
                throw new NotSupportedException();

            return (FieldInfo) data;
        }

        public static ReflectionMethodAnalyser FromMethod(MethodInfo method)
        {
            return new ReflectionMethodAnalyser(method);
        }

        #region Nested type: BackingFieldPattern

        private class BackingFieldPattern : ILPattern
        {
            public static readonly object BackingFieldKey = new object();

            private static readonly ILPattern Pattern = Sequence(
                Optional(OpCodes.Nop),
                OpCode(OpCodes.Ldarg_0),
                OpCode(OpCodes.Ldfld));

            public override void Match(MatchContext context)
            {
                Pattern.Match(context);
                if (!context.IsMatch) 
                    return;

                var match = GetLastMatchingInstruction(context);
                var field = (FieldInfo) match.Operand;
                context.AddData(BackingFieldKey, field);
            }
        }

        #endregion
    }
}
