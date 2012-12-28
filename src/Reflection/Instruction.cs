using System.Reflection.Emit;
using System.Text;

namespace NDatabase.Reflection
{
    internal sealed class Instruction
    {
        private readonly int _offset;
        private OpCode _opcode;
        private object _operand;

        internal Instruction(int offset, OpCode opcode)
        {
            _offset = offset;
            _opcode = opcode;
        }

        public Instruction Next { get; internal set; }

        public int Offset
        {
            get { return _offset; }
        }

        public OpCode OpCode
        {
            get { return _opcode; }
        }

        public object Operand
        {
            get { return _operand; }
            internal set { _operand = value; }
        }

        public Instruction Previous { get; internal set; }

        public int Size
        {
            get
            {
                var size = _opcode.Size;
                switch (_opcode.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                        return (size + 4);

                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        return (size + 8);

                    case OperandType.InlineNone:
                    case OperandType.InlinePhi:
                    case ((OperandType) 8):
                    case OperandType.InlineSig:
                        return size;

                    case OperandType.InlineSwitch:
                        return (size + ((1 + ((int[]) _operand).Length)*4));

                    case OperandType.InlineVar:
                        return (size + 2);

                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        size++;
                        return size;
                }
                return size;
            }
        }

        private static void AppendLabel(StringBuilder builder, Instruction instruction)
        {
            builder.Append("IL_");
            builder.Append(instruction._offset.ToString("x4"));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            AppendLabel(builder, this);
            builder.Append(':');
            builder.Append(' ');
            builder.Append(_opcode.Name);
            if (_operand != null)
            {
                builder.Append(' ');
                switch (_opcode.OperandType)
                {
                    case OperandType.InlineString:
                        builder.Append('"');
                        builder.Append(_operand);
                        builder.Append('"');
                        break;

                    case OperandType.InlineSwitch:
                        {
                            var operand = (Instruction[]) _operand;
                            for (var i = 0; i < operand.Length; i++)
                            {
                                if (i > 0)
                                {
                                    builder.Append(',');
                                }
                                AppendLabel(builder, operand[i]);
                            }
                            break;
                        }
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        AppendLabel(builder, (Instruction) _operand);
                        break;

                    default:
                        builder.Append(_operand);
                        break;
                }
            }
            return builder.ToString();
        }
    }
}