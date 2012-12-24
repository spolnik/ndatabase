using System;
using System.Collections.Generic;

namespace LINQPad
{
    public interface ICustomMemberProvider
    {
        // Each of these methods must return a sequence
        // with the same number of elements:
        IEnumerable<string> GetNames();
        IEnumerable<Type> GetTypes();
        IEnumerable<object> GetValues();
    }
}