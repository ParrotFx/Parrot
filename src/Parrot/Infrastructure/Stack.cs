using System;
using System.Collections.Generic;

namespace Parrot.Infrastructure
{
    public class Stack
    {
        private readonly Dictionary<string, Func<string, object>> _variables;

        public Stack(Dictionary<string, Func<string, object>> variables)
        {
            _variables = variables;
        }

        public object Get(string name)
        {
            return _variables.ContainsKey(name)
                   ? _variables[name](name)
                   : null;
        }
    }
}