using System;
using System.Collections.Generic;

namespace Parrot.Lexer
{
	public static class TokenFactory
	{
		private static readonly Dictionary<char, Type> _tokenTypeByChar =
			new Dictionary<char, Type> {
				{',', typeof (CommaToken) },
				{'(', typeof(OpenParenthesisToken)},
				{')', typeof(CloseParenthesisToken)},
				{'[', typeof(OpenBracketToken)},
				{']', typeof(CloseBracketToken)},
				{'=', typeof(EqualToken)},
				{'{', typeof(OpenBracesToken)},
				{'}', typeof(CloseBracesToken)},
				{'>', typeof(GreaterThanToken)},
				{'+', typeof(PlusToken)},
				{'|', typeof(StringLiteralPipeToken)},
				{'"', typeof(QuotedStringLiteralToken)},
				{'\'', typeof(QuotedStringLiteralToken)},
				{'@', typeof(AtToken)},
				{'^', typeof(CaretToken)},
			};
		private static readonly Dictionary<TokenType, Type> _tokenTypeByTypeKey =
			new Dictionary<TokenType, Type> {
				{TokenType.Identifier, typeof (IdentifierToken) },
				{TokenType.Whitespace, typeof(WhitespaceToken)}
				
			};

		public static Token Create(char token)
		{
			return createInstance(_tokenTypeByChar[token]);
		}

		public static Token Create(TokenType type)
		{
			return createInstance(_tokenTypeByTypeKey[type]);
		}

		private static Token createInstance(Type tokenType)
		{
			return (Token)Activator.CreateInstance(tokenType);
		}
	}
}