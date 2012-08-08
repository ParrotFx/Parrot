// -----------------------------------------------------------------------
// <copyright file="LocalStack.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Infrastructure
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public class LocalsStack
    {
        private readonly List<Stack> _stacks;

        public LocalsStack()
        {
            _stacks = new List<Stack>();
        }

        public bool Get(string name, out object result)
        {
            for (int i = _stacks.Count - 1; i >= 0; i--)
            {
                var value = _stacks[i].Get(name);
                if (value != null)
                {
                    result = value;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public void Pop()
        {
            if (_stacks.Count > 0)
            {
                _stacks.RemoveAt(_stacks.Count - 1);
            }
        }

        public void Push(Dictionary<string, Func<string, object>> variables)
        {
            _stacks.Add(new Stack(variables));
        }
    }
}
