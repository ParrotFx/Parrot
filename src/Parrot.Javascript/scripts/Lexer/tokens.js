var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
/// <reference path="token.ts" />
/// <reference path="tokenType.ts" />
var AtToken = (function (_super) {
    __extends(AtToken, _super);
    function AtToken(index) {
        _super.call(this, index, "@", TokenType.at);
    }
    return AtToken;
})(Token);
var QuotedStringLiteralToken = (function (_super) {
    __extends(QuotedStringLiteralToken, _super);
    function QuotedStringLiteralToken(index, content) {
        _super.call(this, index, content, TokenType.quotedStringLiteral);
    }
    return QuotedStringLiteralToken;
})(Token);
var StringLiteralPipeToken = (function (_super) {
    __extends(StringLiteralPipeToken, _super);
    function StringLiteralPipeToken(index, content) {
        _super.call(this, index, content, TokenType.stringLiteralPipe);
    }
    return StringLiteralPipeToken;
})(Token);
var PlusToken = (function (_super) {
    __extends(PlusToken, _super);
    function PlusToken(index) {
        _super.call(this, index, "+", TokenType.plus);
    }
    return PlusToken;
})(Token);
var GreaterThanToken = (function (_super) {
    __extends(GreaterThanToken, _super);
    function GreaterThanToken(index) {
        _super.call(this, index, ">", TokenType.greaterThan);
    }
    return GreaterThanToken;
})(Token);
var CloseParenthesisToken = (function (_super) {
    __extends(CloseParenthesisToken, _super);
    function CloseParenthesisToken(index) {
        _super.call(this, index, ")", TokenType.closeParenthesis);
    }
    return CloseParenthesisToken;
})(Token);
var CloseBracesToken = (function (_super) {
    __extends(CloseBracesToken, _super);
    function CloseBracesToken(index) {
        _super.call(this, index, "}", TokenType.closeBrace);
    }
    return CloseBracesToken;
})(Token);
var OpenBracesToken = (function (_super) {
    __extends(OpenBracesToken, _super);
    function OpenBracesToken(index) {
        _super.call(this, index, "{", TokenType.openBrace);
    }
    return OpenBracesToken;
})(Token);
var EqualToken = (function (_super) {
    __extends(EqualToken, _super);
    function EqualToken(index) {
        _super.call(this, index, "=", TokenType.equal);
    }
    return EqualToken;
})(Token);
var CloseBracketToken = (function (_super) {
    __extends(CloseBracketToken, _super);
    function CloseBracketToken(index) {
        _super.call(this, index, "]", TokenType.closeBracket);
    }
    return CloseBracketToken;
})(Token);
var OpenBracketToken = (function (_super) {
    __extends(OpenBracketToken, _super);
    function OpenBracketToken(index) {
        _super.call(this, index, "[", TokenType.openBracket);
    }
    return OpenBracketToken;
})(Token);
var OpenParenthesisToken = (function (_super) {
    __extends(OpenParenthesisToken, _super);
    function OpenParenthesisToken(index) {
        _super.call(this, index, "(", TokenType.openParenthesis);
    }
    return OpenParenthesisToken;
})(Token);
var CommaToken = (function (_super) {
    __extends(CommaToken, _super);
    function CommaToken(index) {
        _super.call(this, index, ",", TokenType.comma);
    }
    return CommaToken;
})(Token);
var IdentifierToken = (function (_super) {
    __extends(IdentifierToken, _super);
    function IdentifierToken(index, content, type) {
        _super.call(this, index, content, type);
    }
    return IdentifierToken;
})(Token);
var WhitespaceToken = (function (_super) {
    __extends(WhitespaceToken, _super);
    function WhitespaceToken(index, content, type) {
        _super.call(this, index, content, type);
    }
    return WhitespaceToken;
})(Token);
//@ sourceMappingURL=tokens.js.map
