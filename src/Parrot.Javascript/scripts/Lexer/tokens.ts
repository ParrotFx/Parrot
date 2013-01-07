/// <reference path="token.ts" />
/// <reference path="tokenType.ts" />
class AtToken extends Token {
    constructor(index: number) {
        super(index, "@", TokenType.at);
    }
}

class QuotedStringLiteralToken extends Token {
    constructor(index: number, content: string) {
        super(index, content, TokenType.quotedStringLiteral);
    }
}

class StringLiteralPipeToken extends Token {
    constructor(index: number, content: string) {
        super(index, content, TokenType.stringLiteralPipe);
    }
}

class PlusToken extends Token {
    constructor(index: number) {
        super(index, "+", TokenType.plus);
    }
}

class GreaterThanToken extends Token {
    constructor(index: number) {
        super(index, ">", TokenType.greaterThan);
    }
}

class CloseParenthesisToken extends Token {
    constructor(index: number) {
        super(index, ")", TokenType.closeParenthesis);
    }
}

class CloseBracesToken extends Token {
    constructor(index: number) {
        super(index, "}", TokenType.closeBrace);
    }
}

class OpenBracesToken extends Token {
    constructor(index: number) {
        super(index, "{", TokenType.openBrace);
    }
}

class EqualToken extends Token {
    constructor(index: number) {
        super(index, "=", TokenType.equal);
    }
}

class CloseBracketToken extends Token {
    constructor(index: number) {
        super(index, "]", TokenType.closeBracket);
    }
}

class OpenBracketToken extends Token {
    constructor(index: number) {
        super(index, "[", TokenType.openBracket);
    }
}

class OpenParenthesisToken extends Token {
    constructor(index: number) {
        super(index, "(", TokenType.openParenthesis);
    }
}

class CommaToken extends Token {
    constructor(index: number) {
        super(index, ",", TokenType.comma);
    }
}

class IdentifierToken extends Token {
    constructor(index: number, content: string, type: TokenType) {
        super(index, content, type);
    }
}

class WhitespaceToken extends Token {
    constructor(index: number, content: string, type: TokenType) {
        super(index, content, type);
    }
}