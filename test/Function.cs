using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    class Function
    {
        private string name;

        public Function(string name)
        {
            this.name = name;
        }
        public override string ToString()
        {
            return name;
        }
    }
}
