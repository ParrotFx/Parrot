///<reference path="tokens.ts" />
class Tokenizer {
    _tokens: Token[];
    _currentIndex: number;
    _source: string;

    constructor(source: string) {
        this._source = source;
        this._currentIndex = 0;
    }

    hasAvailableTokens() : bool {
        return this._currentIndex < this._source.length;
    }

    peek() : string {
        var character = this._source.charAt(this._currentIndex);
        return character;
    }

    consume() : string {
        if (this._currentIndex >= this._source.length) {
            throw new EndOfStreamException();
        }

        var character = this._source.charAt(this._currentIndex);
        this._currentIndex++;
        return character;
    }

    consumeIdentifier() : string {
        var identifier = "";
        var character = this.peek();
        while (this.isIdTail(character)) {
            this.consume();
            identifier += character;
            character = this.peek();
        }

        return identifier;
    }

    consumeWhitespace() : string {
        var whitespace = "";
        var character = this.peek();
        while (this.isWhitespace(character)) {
            this.consume();
            whitespace += character;
            character = this.peek();
        }

        return whitespace;
    }

    consumeUntilNewLine(): string {
        var result = "";
        var character = this.peek();
        while (!this.isNewLine(character)) {
            this.consume();
            result += character;
            character = this.peek();
        }

        return result;
    }

    consumeQuotedStringLiteral(quote : string): string {
        var result = this.consume();;
        var character = this.peek();
        while (character != quote) {
            this.consume();
            result += character;
            character = this.peek();
        }

        result += this.consume();
        return result;
    }

    isNewLine(character: string) {
        return character == "\r" || character == "\n"
    }

    getNextToken() : Token {
        if (this.hasAvailableTokens()) {
            var currentCharacter = this.peek();
            
            if (this.isIdentifierHead(currentCharacter)) {
                var token = new IdentifierToken(this._currentIndex, this.consumeIdentifier(), TokenType.identifier);
                return token;
            }

            if (this.isWhitespace(currentCharacter)) {
                return new WhitespaceToken(this._currentIndex, this.consumeWhitespace(), TokenType.whitespace);
            }
            
            switch (currentCharacter) {
                case ',': //this is for the future
                    this.consume();
                    return new CommaToken(this._currentIndex);
                case '(': //parameter list start
                    this.consume();
                    return new OpenParenthesisToken(this._currentIndex);
                case ')': //parameter list end
                    this.consume();
                    return new CloseParenthesisToken(this._currentIndex);
                case '[': //attribute list start
                    this.consume();
                    return new OpenBracketToken(this._currentIndex);
                case ']': //attribute list end
                    this.consume();
                    return new CloseBracketToken(this._currentIndex);
                case '=': //attribute assignment, raw output
                    this.consume();
                    return new EqualToken(this._currentIndex);
                case '{': //child block start
                    this.consume();
                    return new OpenBracesToken(this._currentIndex);
                case '}': //child block end
                    this.consume();
                    return new CloseBracesToken(this._currentIndex);
                case '>': //child assignment
                    this.consume();
                    return new GreaterThanToken(this._currentIndex);
                case '+': //sibling assignment
                    this.consume();
                    return new PlusToken(this._currentIndex);
                case '|': //string literal pipe
                    return new StringLiteralPipeToken(this._currentIndex, this.consumeUntilNewLine());
                case '"': //quoted string literal
                    return new QuotedStringLiteralToken(this._currentIndex, this.consumeQuotedStringLiteral("\""));
                case '\'': //quoted string literal
                    return new QuotedStringLiteralToken(this._currentIndex, this.consumeQuotedStringLiteral("\'"));
                case '@': //Encoded output
                    this.consume();
                    return new AtToken(this._currentIndex);
                case '\0':
                    return null;
                default:
                    throw new UnexpectedTokenException("Unexpected token: "+currentCharacter);
            }
        }

        return null;
    }
    
    isWhitespace(character: string) : bool {
        return character == "\r" || character == "\n" || character == " " || character == "\f" || character == "\t" || character == "\u000B";
    }

    isIdentifierHead(character: string) : bool {
         var isLetter = /[a-zA-Z]/;
         return character.match(isLetter) || character == "_" || character == "#" || character == ".";
    }

    isIdTail(character: string) : bool {
         var isNumber = /[0-9]/;
         return character.match(isNumber) || this.isIdentifierHead(character) || character == ":" || character == "-";
    }

    tokenize(): Token[] {
        var token: Token;
        var _tokens : Token[] = [];
        while ((token = this.getNextToken()) != null) {
            _tokens.push(token);
        }
        
        return _tokens;
    }

    tokens(): Token[] {
        return this.tokenize();
    }
}

class EndOfStreamException {
}

class UnexpectedTokenException {
    message: string;
    constructor(message: string) {
        this.message = message;
    }
}