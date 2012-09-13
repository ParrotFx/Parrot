using System;
using System.Collections.Generic;
using System.Linq;
using Parrot.Lexer;

namespace Parrot.Parser
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Stream<T> where T: class
    {
        private IEnumerable<T> _source;
        private Queue<T> _storage;
        private IEnumerator<T> _enumerator;

        public Stream(IEnumerable<T> source)
        {
            _storage = new Queue<T>();
            _source = source;
            _enumerator = source.GetEnumerator();
        }

        public T Peek()
        {
            if (!_storage.Any())
            {
                try
                {
                    _storage.Enqueue(GetNext());
                }
                catch (Exception)
                {
                    return default(T);
                }
            }

            return _storage.Peek();
        }

        public T Next()
        {
            if (_storage.Any())
            {
                return _storage.Dequeue();
            }

            return GetNext();
        }

        private T GetNext()
        {
            //should i consume whitespace here?

            while (_enumerator.MoveNext())
            {
                if ((_enumerator.Current as Token).Type != TokenType.Whitespace)
                {
                    return _enumerator.Current;
                }
            }

            return null;
        }
    }
}