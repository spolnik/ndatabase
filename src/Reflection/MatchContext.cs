using System.Collections.Generic;

namespace NDatabase.Reflection
{
    internal sealed class MatchContext
    {
        private readonly Dictionary<object, object> _data = new Dictionary<object, object>();
        private Instruction _instruction;
        private bool _success;

        internal MatchContext(Instruction instruction)
        {
            Reset(instruction);
        }

        public bool IsMatch
        {
            get { return _success; }
            set { _success = true; }
        }

        public Instruction Instruction
        {
            get { return _instruction; }
            set { _instruction = value; }
        }

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        public void AddData(object key, object value)
        {
            _data.Add(key, value);
        }

        internal void Advance()
        {
            _instruction = _instruction.Next;
        }

        internal void Reset(Instruction instruction)
        {
            _instruction = instruction;
            _success = true;
        }

        public bool TryGetData(object key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }
    }
}