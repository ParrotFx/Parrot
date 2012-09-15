using System;
using System.Collections.Generic;
using System.Linq;
using Parrot.Lexer;

namespace Parrot.Parser
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Stream
    {
        private readonly IList<Token> _list;
        private int _index;
        private readonly int _count;

        public Stream(IList<Token> source)
        {
            _list = source;
            _index = -1;
            _count = source.Count;
        }

        public Token Peek()
        {
            var temp = _index + 1;
            while (temp < _count)
            {
                if (_list[temp].Type != TokenType.Whitespace)
                {
                    return _list[temp];
                }

                temp++;
            }

            return null;
        }

        public void NextNoReturn()
        {
            GetNextNoReturn();
        }

        public void GetNextNoReturn()
        {
            _index++;
            while (_index < _count && _list[_index].Type == TokenType.Whitespace)
            {
                _index++;
            }
        }

        public Token Next()
        {
            _index ++;
            while (_index < _count)
            {
                if (_list[_index].Type != TokenType.Whitespace)
                {
                    return _list[_index];
                }

                _index++;
            }

            return null;
        }
    }
}