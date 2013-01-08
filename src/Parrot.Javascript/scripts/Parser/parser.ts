///<reference path="stream.ts" />
///<reference path="parserError.ts" />
///<reference path="../lexer/tokenizer.ts" />
///<reference path="../lexer/tokentype.ts" />
class ParrotDocument {
    public errors: ParserError[];
    public children: Statement[];

    constructor() {
        this.errors = [];
        this.children = [];
    }
}

class Parser {
    errors: ParserError[];

    constructor() {
        this.errors = [];
    }

    parse(stream: string) {
        var document = new ParrotDocument();
        var tokenizer = new Tokenizer(stream);
        var tokens = tokenizer.tokens();
        var tokenStream = new Stream(tokens);

        var parent = this;

        this.parseStream(tokenStream, function (s) {
            for (var i in s) {
                parent.parseStatementErrors(s[i]);
                document.children.push(s[i]);
            }
        });

        document.errors = this.errors;
        
        console.log(document.children);

        return document;
    }

    parseStatementErrors(statement: Statement) {
        if (statement.errors.length > 0) {
            for (var i in statement.errors) {
                var error = statement.errors[i];
                error.index += statement.index;
                this.errors.push(error);
            }
        }
    }

    parseStream(stream: Stream, callback) {
        while (stream.peek() != null) {
            var token = stream.peek();
            switch (token.type) {
                case TokenType.stringLiteral:
                case TokenType.stringLiteralPipe:
                case TokenType.quotedStringLiteral:
                case TokenType.identifier:
                case TokenType.openBracket:
                case TokenType.openParenthesis:
                case TokenType.equal:
                case TokenType.at:
                    var statement = this.parseStatement(stream);
                    callback(statement);
                    break;
                default:
                    this.errors.push(new UnexpectedToken(token));
                    stream.next();
                    break;
                //throw new ParserException(token);
            }
        }
    }

    parseStatement(stream: Stream): Statement[] {
        var previousToken = stream.peek();
        if (previousToken == null) {
            this.errors.push(new EndOfStream());
            return [];
        }

        var tokenType = previousToken.type;
        var identifier: Token = null;

        switch (tokenType) {
            case TokenType.identifier:
                //standard identifier
                identifier = stream.next();
                break;
            case TokenType.openBracket:
            case TokenType.openParenthesis:
                //ignore these
                break;
            case TokenType.stringLiteral:
            case TokenType.stringLiteralPipe:
            case TokenType.quotedStringLiteral:
                //string statement
                identifier = stream.next();
                break;
            case TokenType.at:
                stream.getNextNoReturn();
                identifier = stream.next();
                break;
            case TokenType.equal:
                stream.getNextNoReturn();
                identifier = stream.next();
                break;
            default:
                this.errors.push(new UnexpectedToken(previousToken));
                return [];
            //throw new ParserException(stream.Peek());
        }

        var statement: Statement;
        var tail: StatementTail = null;

        var exit: bool = false;

        while (stream.peek() != null && !exit) {
            var token = stream.peek();
            if (token == null) {
                break;
            }

            switch (token.type) {
                case TokenType.openParenthesis:
                case TokenType.openBracket:
                case TokenType.openBrace:
                    tail = this.parseStatementTail(stream);
                    break;
                case TokenType.greaterThan:
                    stream.nextNoReturn();
                    tail = this.parseSingleStatementTail(stream);
                    break;
                default:
                    this.getStatementFromToken(identifier, tail, null);
                    exit = true;
                    break;
            }
        }
        statement = this.getStatementFromToken(identifier, tail, previousToken);

        var list: Statement[] = [];
        list.push(statement);

        while (stream.peek() != null) {
            if (stream.peek().type == TokenType.plus) {
                stream.nextNoReturn();
                var siblings = this.parseStatement(stream);
                for (var i in siblings) {
                    list.push(siblings[i]);
                }
            } else {
                break;
            }
        }

        return list;

    }

    getStatementFromToken(identifier: Token, tail: StatementTail, previousToken: Token): Statement {

        var value = identifier != null ? identifier.content : "";
        if (identifier != null) {
            switch (identifier.type) {
                case TokenType.stringLiteral:
                case TokenType.quotedStringLiteral:
                    return new StringLiteral(value, tail, identifier.index);

                case TokenType.stringLiteralPipe:
                    return new StringLiteralPipe(value.substring(1), tail, identifier.index);
            }
        }

        if (previousToken != null) {
            switch (previousToken.type) {
                case TokenType.at:
                    return new EncodedOutput(value, null, previousToken.index);
                case TokenType.equal:
                    return new RawOutput(value, null, previousToken.index);
            }
        }

        return new Statement(value, tail, identifier.index);
    }

    parseSingleStatementTail(stream: Stream) {
        var statementList = this.parseStatement(stream);
        var tail = new StatementTail();
        tail.children = statementList;

        return tail;
    }

    parseStatementTail(stream: Stream) {
        var additional: any[] = new Array(3);

        var exit: bool = false;

        while (stream.peek() != null && !exit) {
            var token = stream.peek();
            if (token == null) {
                break;
            }

            switch (token.type) {
                case TokenType.openParenthesis:
                    additional[1] = this.parseParameters(stream);
                    break;
                case TokenType.openBracket:
                    additional[0] = this.parseAttributes(stream);
                    break;
                case TokenType.greaterThan:
                    additional[2] = this.parseChild(stream);
                    break;
                case TokenType.openBrace:
                    additional[2] = this.parseChildren(stream);
                    break;
                default:
                    exit = true;
                    break;
            }
        }

        var tail = new StatementTail();
        tail.attributes = additional[0];
        tail.parameters = additional[1];
        tail.children = additional[2];

        return tail;
    }

    parseChild(stream: Stream): Statement[] {
        var child: Statement[] = [];
        stream.nextNoReturn();

        var exit: bool = false;

        while (stream.peek() != null && !exit) {
            var token = stream.peek();
            if (token == null) { break; }

            var statements = this.parseStatement(stream);
            for (var i in statements) {
                child.push(statements[i]);
            }
            exit = true;
        }

        return child;
    }

    parseChildren(stream: Stream): Statement[] {
        var statements: Statement[] = [];

        stream.nextNoReturn();
        var exit: bool = false;
        while (stream.peek() != null && !exit) {
            var token = stream.peek();
            if (token == null) { break; }

            switch (token.type) {
                case TokenType.plus:
                    break;
                case TokenType.closeBrace:
                    stream.nextNoReturn();
                    exit = true;
                    break;
                default:
                    var statements = this.parseStatement(stream);
                    for (var i in statements) {
                        statements.push(statements[i]);
                    }
                    break;
            }
        }

        return statements;
    }

    parseParameters(stream: Stream): Parameter[] {
        var list: Parameter[] = [];

        stream.nextNoReturn();

        var exit: bool = false;

        while (stream.peek() != null && !exit) {
            var token = stream.peek();
            if (token == null) { break; }

            switch (token.type) {
                case TokenType.identifier:
                case TokenType.quotedStringLiteral:
                case TokenType.stringLiteralPipe:
                    list.push(this.parseParameter(stream));
                    break;
                case TokenType.comma:
                    //another parameter - consume this
                    stream.nextNoReturn();
                    break;
                case TokenType.closeParenthesis:
                    //consume close parenthesis
                    stream.nextNoReturn();
                    exit = true;
                    break;
                default:
                    //read until )
                    this.errors.push(new UnexpectedToken(token));
                    return list;
                //throw new ParserException(token);
            }
        }

        return list;
    }

    parseParameter(stream: Stream): Parameter {
        var identifier = stream.next();
        switch (identifier.type) {
            case TokenType.stringLiteralPipe:
            case TokenType.quotedStringLiteral:
            case TokenType.stringLiteral:
            case TokenType.identifier:
                break;
            default:
                //invalid token
                this.errors.push(new UnexpectedToken(identifier));
                //throw new ParserException(identifier);
                return null;
        }

        //reduction
        return new Parameter(identifier.content);
    }

    parseAttributes(stream: Stream): Attribute[] {
        var attributes: Attribute[] = [];

        var token: Token = null;

        var exit: bool = false;

        while (stream.peek() != null && !exit) {
            token = stream.peek();
            if (token == null) { break; }

            switch (token.type) {
                case TokenType.identifier:
                    attributes.push(this.parseAttribute(stream));
                    break;
                case TokenType.closeBracket:
                    //consume close bracket
                    stream.nextNoReturn();
                    exit = true;
                    break;
                default:
                    //invalid token
                    this.errors.push(new AttributeIdentifierMissing(token.index));
                    //throw new ParserException(token);
                    return attributes;
            }
        }
    }

    parseAttribute(stream: Stream): Attribute {
        var identifier = stream.next();
        var equalsToken: Token = stream.peek();
        if (equalsToken != null && equalsToken.type == TokenType.equal) {
            stream.nextNoReturn();
            var valueToken: Token = stream.peek();
            if (valueToken == null) {
                //TODO: Errors.Add(stream.Next());
                this.errors.push(new UnexpectedToken(identifier));
                return new Attribute(identifier.content, null);
                //throw new ParserException(string.Format("Unexpected end of stream"));
            }

            if (valueToken.type == TokenType.closeBracket) {
                //then it's an invalid declaration
                this.errors.push(new AttributeValueMissing(valueToken.index));
            }

            var value: Statement = this.parseStatement(stream)[0];
            //force this as an attribute type
            if (value == null) {
            }
            else {
                switch (value.name) {
                    case "true":
                    case "false":
                    case "null":
                        value = new StringLiteral("\"" + value.name + "\"", null, 0);
                        break;
                }
            }

            //reduction
            return new Attribute(identifier.content, value);
        }

        //single attribute only
        return new Attribute(identifier.content, null);
    }
}

class AttributeValueMissing extends ParserError {
    constructor(index: number) {
        super();
        this.index = index;
        this.message = "Attribute must have a value";
    }
}

class AttributeIdentifierMissing extends ParserError {
    constructor(index: number) {
        super();
        this.index = index;
        this.message = "Invalid attribute name";
    }
}

class EndOfStream extends ParserError {
    constructor() {
        super();
        this.message = "Unexpected end of file.";
    }
}

class UnexpectedToken extends ParserError {
    type: TokenType;
    token: string;

    constructor(token: Token) {
        super();
        this.type = token.type;
        this.token = token.content;
        this.index = token.index;

        this.message = "Unexpected token: " + this.type;
    }
}

class Statement {
    name: string;
    parameters: Parameter[];
    attributes: Attribute[];
    children: Statement[];
    index: number;
    length: number;
    identifierParts: Identifier[];
    errors: ParserError[];

    constructor(name: string, tail: StatementTail, index: number) {
        this.index = index;
        this.parameters = [];
        this.attributes = [];
        this.children = [];
        this.identifierParts = [];
        this.errors = [];

        var container = this;

        if (this.indexOfAny(name, [".", "#", ":"]) > -1) {
            this.getIdentifierParts(name, function (part: Identifier) {
                switch (part.type) {
                    case IdentifierType.id:
                        if (part.name.length == 0) {
                            container.errors.push(new MissingIdDeclaration(part.index - 1, 1));
                        } else if (container.anyAttributes(function (a) { return a.key == "id"; })) {
                            container.errors.push(new MultipleIdDeclarations(part.name, part.index - 1, part.name.length + 1));
                        } else {
                            var literal = new StringLiteral("\"" + part.name + "\"", null, 0);
                            container.addAttribute(new Attribute("id", literal));
                        }
                        break;
                    case IdentifierType.class:
                        if (part.name.length == 0) {
                            container.errors.push(new MissingClassDeclaration(1, 1));
                        } else {
                            var literal = new StringLiteral("\"" + part.name + "\"", null, 0);
                            container.addAttribute(new Attribute("class", literal));
                        }
                        break;
                    case IdentifierType.type:
                        var literal = new StringLiteral("\"" + part.name + "\"", null, 0);
                        container.addAttribute(new Attribute("type", literal));
                        break;
                    case IdentifierType.literal:
                        container.name = part.name;
                        break;
                }

                container.identifierParts.push(part);
            });
        } else {
            container.name = name;
        }

        this.parseStatementTail(tail);
    }

    anyAttributes(callback) {
        for (var i in this.attributes) {
            if (callback(this.attributes[i])) {
                return true;
            }
        }

        return false;
    }

    parseStatementTail(tail: StatementTail) {
        if (tail != null) {
            if (tail.parameters != null) {
                this.parameters = tail.parameters;
            }

            if (tail.attributes != null) {
                for (var i in this.attributes) {
                    tail.attributes.push(this.attributes[i]);
                }

                this.attributes = tail.attributes;
            }

            if (tail.children != null) {
                this.children = tail.children;
            }
        }
    }

    addAttribute(node: Attribute) {
        this.attributes.push(node);
    }

    identifierTypeFromCharacter(character: string, currentType: IdentifierType): IdentifierType {

        switch (character) {
            case ":":
                return IdentifierType.type;
            case "#":
                return IdentifierType.id;
            case ".":
                return IdentifierType.class;
        }

        return currentType;
    }

    getIdentifierParts(source: string, callback) {
        var index = 0;
        var partType: IdentifierType = IdentifierType.literal;
        var nextType: IdentifierType = IdentifierType.none;

        for (var i = 0; i < source.length; i++) {
            nextType = this.identifierTypeFromCharacter(source.charAt(i), nextType);

            if (nextType != IdentifierType.none) {
                var identifier = new Identifier();
                identifier.name = source.substring(index, index + (i - index));
                identifier.type = partType;
                identifier.index = index;
                identifier.length = i - index;

                callback(identifier);

                index = i + 1;
                partType = nextType;
                nextType = IdentifierType.none;
            }
        }

        var identifier = new Identifier();
        identifier.name = source.substring(index);
        identifier.type = partType;
        identifier.index = index;

        callback(identifier);
    }

    indexOfAny(source: string, chars: string[]): number {
        for (var i in chars) {
            var index: number = source.indexOf(chars[i]);
            if (index != -1) {
                return index;
            }
        }

        return -1;
    }
}

class MissingIdDeclaration extends ParserError {
    constructor(index: number, length: number) {
        super();
        this.index = index;
        this.length = length;
        this.message = "Missing Id declaration";
    }
}

class MissingClassDeclaration extends ParserError {
    constructor(index: number, length: number) {
        super();
        this.index = index;
        this.length = length;
        this.message = "Missing Class declaration";
    }
}

class MultipleIdDeclarations extends ParserError {
    id: string;

    constructor(id: string, index: number, length: number) {
        super();
        this.id = id;
        this.index = index;
        this.length = length;
        this.message = "Element may not have more than one id";
    }
}

class Parameter {
    value: string;

    constructor(value: string) {
        this.value = value;
    }
}

class StatementTail {
    parameters: Parameter[];
    attributes: Attribute[];
    children: Statement[];
}

class Attribute {
    key: string;
    value: Statement;

    constructor(key: string, value: Statement) {
        this.key = key;
        this.value = value;
    }
}

class Identifier {
    name: string;
    index: number;
    length: number;
    type: IdentifierType;
}

enum IdentifierType {
    id,
    class ,
    literal,
    type,
    none
}

enum ValueType {
    stringLiteral,
    property,
    local,
    keyword
}

enum StringLiteralPartType {
    literal,
    encoded,
    raw
}

class StringLiteralPart {
    data: string;
    type: StringLiteralPartType;
    index: number;
    length: number;

    constructor(type: StringLiteralPartType, data: string, index: number) {
        this.type = type;
        this.data = data;
        this.index = index;
        this.length = data.length;
    }
}

class StringLiteral extends Statement {
    values: StringLiteralPart[];
    valueType: ValueType;

    constructor(value: string, tail: StatementTail, index: number) {
        super("string", tail, index);

        this.values = [];
        this.valueType = ValueType.stringLiteral;

        if (this.isWrappedInQuotes(value)) {
            this.valueType = ValueType.stringLiteral;
            value = value.substring(1, 1 + value.length - 2);
        } else if (value == "this") {
            this.valueType = ValueType.local;
        } else if (value == "null" || value == "true" || value == "false") {
            this.valueType = ValueType.keyword;
        } else {
            this.valueType = ValueType.property;
        }

        if (this.valueType == ValueType.stringLiteral) {
            this.values = this.parse(value);
        }
    }

    startsWith(source: string, value: string): bool {
        return source.length > 0 && source.charAt(0) == value;
    }

    isWrappedInQuotes(value: string): bool {
        return (this.startsWith(value, "\"") || this.startsWith(value, "'"));
    }

    parse(source: string): StringLiteralPart[] {
        var parts: StringLiteralPart[] = [];
        var tempCounter: number = 0;
        var c: string[] = new Array(source.length);

        for (var i: number = 0; i < source.length; i++) {
            if (source.charAt(i) == "@" || source.charAt(i) == "=") {
                var comparer: string = source.charAt(i);
                var comparerType: StringLiteralPartType = (comparer == "@" ? StringLiteralPartType.encoded : StringLiteralPartType.raw);

                i++;

                if (source.charAt(Math.min(source.length - 1, i)) == comparer) {
                    c[tempCounter++] = comparer;
                }
                else if (this.isIdentifierHead(source.charAt(i))) {
                    if (tempCounter > 0) {
                        parts.push(new StringLiteralPart(StringLiteralPartType.literal, c.join(""), i - tempCounter));
                    }

                    tempCounter = 0;
                    var word: string = "";
                    for (; i < source.length; i++, tempCounter++) {
                        if (!this.isIdTail(source.charAt(i))) {
                            break;
                        }

                        word += source.charAt(i);
                    }

                    if (word.charAt(word.length - 1) == ".") {
                        word = word.substring(0, word.length - 1);
                        parts.push(new StringLiteralPart(comparerType, word, i - tempCounter));
                        tempCounter = 0;
                        c[tempCounter++] = ".";
                    }
                    else {
                        parts.push(new StringLiteralPart(comparerType, word, i - tempCounter));
                        tempCounter = 0;
                    }

                    if (i < source.length) {
                        c[tempCounter++] = source.charAt(i);
                    }

                } else {
                    c[tempCounter++] = comparer;
                    i -= 1;
                }
            } else {
                c[tempCounter++] = source.charAt(i);
            }
        }

        if (tempCounter > 0) {
            parts.push(new StringLiteralPart(StringLiteralPartType.literal, c.join(""), source.length - tempCounter));
        }

        return parts;
    }

    isIdentifierHead(character: string): bool {
        var isLetter = /[a-zA-Z]/;
        return character.match(isLetter) || character == "_" || character == "#" || character == ".";
    }

    isIdTail(character: string): bool {
        var isNumber = /[0-9]/;
        return character.match(isNumber) || this.isIdentifierHead(character) || character == ":" || character == "-";
    }

}

class StringLiteralPipe extends StringLiteral {
    constructor(value: string, tail: StatementTail, index: number) {
        super(value, tail, index);
    }
}

class EncodedOutput extends StringLiteral {
    constructor(variableName: string, tail: StatementTail, index: number) {
        super("\"@" + variableName + "\"", tail, index);
    }
}

class RawOutput extends StringLiteral {
    constructor(variableName: string, tail: StatementTail, index: number) {
        super("\"=" + variableName + "\"", tail, index);
    }
}