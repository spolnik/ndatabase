using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NDatabase.Reflection
{
    internal abstract class ILPattern
    {
        protected static Instruction GetLastMatchingInstruction(MatchContext context)
        {
            return context.Instruction == null ? null : context.Instruction.Previous;
        }

        public abstract void Match(MatchContext context);

        public static MatchContext Match(MethodBase method, ILPattern pattern)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (pattern == null)
                throw new ArgumentNullException("pattern");

            var instructions = (IList<Instruction>) MethodBodyReader.GetInstructions(method).AsReadOnly();
            if (instructions.Count == 0)
                throw new ArgumentException();

            var context = new MatchContext(instructions[0]);
            pattern.Match(context);
            return context;
        }

        public static ILPattern OpCode(OpCode opcode)
        {
            return new OpCodePattern(opcode);
        }

        public static ILPattern Optional(ILPattern pattern)
        {
            return new OptionalPattern(pattern);
        }

        public static ILPattern Optional(params OpCode[] opcodes)
        {
            return Optional(Sequence((from opcode in opcodes select OpCode(opcode)).ToArray<ILPattern>()));
        }

        public static ILPattern Optional(OpCode opcode)
        {
            return Optional(OpCode(opcode));
        }

        public static ILPattern Sequence(params ILPattern[] patterns)
        {
            return new SequencePattern(patterns);
        }

        public bool TryMatch(MatchContext context)
        {
            var instruction = context.Instruction;
            Match(context);

            if (context.Success)
                return true;

            context.Reset(instruction);
            return false;
        }

        #region Nested type: OpCodePattern

        private class OpCodePattern : ILPattern
        {
            private readonly OpCode _opcode;

            public OpCodePattern(OpCode opcode)
            {
                _opcode = opcode;
            }

            public override void Match(MatchContext context)
            {
                if (context.Instruction == null)
                {
                    context.Success = false;
                }
                else
                {
                    context.Success = context.Instruction.OpCode == _opcode;
                    context.Advance();
                }
            }
        }

        #endregion

        #region Nested type: OptionalPattern

        private class OptionalPattern : ILPattern
        {
            private readonly ILPattern _pattern;

            public OptionalPattern(ILPattern optional)
            {
                _pattern = optional;
            }

            public override void Match(MatchContext context)
            {
                _pattern.TryMatch(context);
            }
        }

        #endregion

        #region Nested type: SequencePattern

        private class SequencePattern : ILPattern
        {
            private readonly ILPattern[] _patterns;

            public SequencePattern(ILPattern[] patterns)
            {
                _patterns = patterns;
            }

            public override void Match(MatchContext context)
            {
                foreach (var pattern in _patterns)
                {
                    pattern.Match(context);
                    if (!context.Success)
                        return;
                }
            }
        }

        #endregion
    }
}