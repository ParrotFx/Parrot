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
    public class Locals
    {
        private readonly IDictionary<string, object> _objectContainer;
        private readonly List<object> _locals;

        public const string LocalsKey = "__locals";

        public Locals(IDictionary<string, object> host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            _objectContainer = host;

            if (_objectContainer.ContainsKey(LocalsKey))
            {
                _locals = _objectContainer[LocalsKey] as List<object>;
            }
            else
            {
                _locals = new List<object>();
            }
        }

        /// <summary>
        /// Pushes a new local values container onto the stack
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Push(object value)
        {
            _locals.Add(value);

            if (_objectContainer.ContainsKey(LocalsKey))
            {
                _objectContainer.Remove(LocalsKey);
            }

            _objectContainer.Add(LocalsKey, _locals);

            return _locals.Count - 1;
        }

        /// <summary>
        /// Removes the most recently added local values container from the stack
        /// </summary>
        public void Pop()
        {
            if (_locals.Count > 0)
            {
                _locals.RemoveAt(_locals.Count - 1);
            }
        }
    }

}
