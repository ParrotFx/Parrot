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
        private readonly Queue<T> _storage;
        private readonly IEnumerator<T> _enumerator;
        

        public Stream(IEnumerable<T> source)
        {
            _storage = new Queue<T>();
            _enumerator = source.GetEnumerator();
        }

        public T Peek()
        {
            if (_storage.Count == 0)
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

        public void NextNoReturn()
        {
            if (_storage.Count > 0)
            {
                _storage.Dequeue();
                return;
            }

            GetNextNoReturn();
        }

        public void GetNextNoReturn()
        {
            while (_enumerator.MoveNext())
            {
                if ((_enumerator.Current as Token).Type != TokenType.Whitespace)
                {
                    break;
                }
            }
        }

        public T Next()
        {
            if (_storage.Count > 0)
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