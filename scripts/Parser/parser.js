var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
///<reference path="stream.ts" />
///<reference path="parserError.ts" />
///<reference path="../lexer/tokenizer.ts" />
///<reference path="../lexer/tokentype.ts" />
var ParrotDocument = (function () {
    function ParrotDocument() {
        this.errors = [];
        this.children = [];
    }
    return ParrotDocument;
})();
var Parser = (function () {
    function Parser() {
        this.errors = [];
    }
    Parser.prototype.parse = function (stream) {
        try  {
            var document = new ParrotDocument();
            var tokenizer = new Tokenizer(stream);
            var tokens = tokenizer.tokens();
            var tokenStream = new Stream(tokens);
            var parent = this;
            this.parseStream(tokenStream, function (s) {
                for(var i in s) {
                    parent.parseStatementErrors(s[i]);
                    document.children.push(s[i]);
                }
            });
        } catch (e) {
            this.errors.push(e);
        }
        document.errors = this.errors;
        return document;
    };
    Parser.prototype.parseStatementErrors = function (statement) {
        if(statement.errors.length > 0) {
            for(var i in statement.errors) {
                var error = statement.errors[i];
                error.index += statement.index;
                this.errors.push(error);
            }
        }
    };
    Parser.prototype.parseStream = function (stream, callback) {
        while(stream.peek() != null) {
            var token = stream.peek();
            switch(token.type) {
                case TokenType.stringLiteral:
                case TokenType.stringLiteralPipe:
                case TokenType.quotedStringLiteral:
                case TokenType.identifier:
                case TokenType.openBracket:
                case TokenType.openParenthesis:
                case TokenType.equal:
                case TokenType.at: {
                    var statement = this.parseStatement(stream);
                    callback(statement);
                    break;

                }
                default: {
                    this.errors.push(new UnexpectedToken(token));
                    stream.next();
                    break;
                    //throw new ParserException(token);
                    
                }
            }
        }
    };
    Parser.prototype.parseStatement = function (stream) {
        var previousToken = stream.peek();
        if(previousToken == null) {
            this.errors.push(new EndOfStream());
            return [];
        }
        var tokenType = previousToken.type;
        var identifier = null;
        switch(tokenType) {
            case TokenType.identifier: {
                //standard identifier
                identifier = stream.next();
                break;

            }
            case TokenType.openBracket:
            case TokenType.openParenthesis: {
                //ignore these
                break;

            }
            case TokenType.stringLiteral:
            case TokenType.stringLiteralPipe:
            case TokenType.quotedStringLiteral: {
                //string statement
                identifier = stream.next();
                break;

            }
            case TokenType.at: {
                stream.nextNoReturn();
                identifier = stream.next();
                break;

            }
            case TokenType.equal: {
                stream.nextNoReturn();
                identifier = stream.next();
                break;

            }
            default: {
                this.errors.push(new UnexpectedToken(previousToken));
                return [];
                //throw new ParserException(stream.Peek());
                
            }
        }
        var statement;
        var tail = null;
        var exit = false;
        while(stream.peek() != null && !exit) {
            var token = stream.peek();
            if(token == null) {
                break;
            }
            switch(token.type) {
                case TokenType.openParenthesis:
                case TokenType.openBracket:
                case TokenType.openBrace: {
                    tail = this.parseStatementTail(stream);
                    break;

                }
                case TokenType.greaterThan: {
                    stream.nextNoReturn();
                    tail = this.parseSingleStatementTail(stream);
                    break;

                }
                case TokenType.stringLiteralPipe: {
                    if(!(previousToken instanceof StringLiteralPipeToken)) {
                        tail = this.parseSingleStatementTail(stream);
                        break;
                    }

                }
                default: {
                    this.getStatementFromToken(identifier, tail, null);
                    exit = true;
                    break;

                }
            }
        }
        statement = this.getStatementFromToken(identifier, tail, previousToken);
        var list = [];
        list.push(statement);
        while(stream.peek() != null) {
            if(stream.peek().type == TokenType.plus) {
                stream.nextNoReturn();
                var siblings = this.parseStatement(stream);
                for(var i in siblings) {
                    list.push(siblings[i]);
                }
            } else {
                break;
            }
        }
        return list;
    };
    Parser.prototype.getStatementFromToken = function (identifier, tail, previousToken) {
        var value = identifier != null ? identifier.content : "";
        if(identifier != null) {
            switch(identifier.type) {
                case TokenType.stringLiteral:
                case TokenType.quotedStringLiteral: {
                    return new StringLiteral(value, tail, identifier.index);

                }
                case TokenType.stringLiteralPipe: {
                    return new StringLiteralPipe(value.substring(1), tail, identifier.index);

                }
            }
        }
        if(previousToken != null) {
            switch(previousToken.type) {
                case TokenType.at: {
                    return new EncodedOutput(value, null, previousToken.index);

                }
                case TokenType.equal: {
                    return new RawOutput(value, null, previousToken.index);

                }
            }
        }
        return new Statement(value, tail, identifier.index);
    };
    Parser.prototype.parseSingleStatementTail = function (stream) {
        var statementList = this.parseStatement(stream);
        var tail = new StatementTail();
        tail.children = statementList;
        return tail;
    };
    Parser.prototype.parseStatementTail = function (stream) {
        var additional = new Array(3);
        var exit = false;
        while(stream.peek() != null && !exit) {
            var token = stream.peek();
            switch(token.type) {
                case TokenType.openParenthesis: {
                    additional[1] = this.parseParameters(stream);
                    break;

                }
                case TokenType.openBracket: {
                    additional[0] = this.parseAttributes(stream);
                    break;

                }
                case TokenType.greaterThan: {
                    additional[2] = this.parseChild(stream);
                    break;

                }
                case TokenType.openBrace: {
                    additional[2] = this.parseChildren(stream);
                    exit = true;
                    break;

                }
                default: {
                    exit = true;
                    break;

                }
            }
        }
        var tail = new StatementTail();
        tail.attributes = additional[0];
        tail.parameters = additional[1];
        tail.children = additional[2];
        return tail;
    };
    Parser.prototype.parseChild = function (stream) {
        var child = [];
        stream.nextNoReturn();
        var exit = false;
        while(stream.peek() != null && !exit) {
            var token = stream.peek();
            if(token == null) {
                break;
            }
            var statements = this.parseStatement(stream);
            for(var i in statements) {
                child.push(statements[i]);
            }
            exit = true;
        }
        return child;
    };
    Parser.prototype.parseChildren = function (stream) {
        var statements = [];
        stream.nextNoReturn();
        var exit = false;
        while(stream.peek() != null && !exit) {
            var token = stream.peek();
            if(token == null) {
                break;
            }
            switch(token.type) {
                case TokenType.plus: {
                    break;

                }
                case TokenType.closeBrace: {
                    stream.nextNoReturn();
                    exit = true;
                    break;

                }
                default: {
                    var s = this.parseStatement(stream);
                    for(var i in s) {
                        statements.push(s[i]);
                    }
                    break;

                }
            }
        }
        return statements;
    };
    Parser.prototype.parseParameters = function (stream) {
        var list = [];
        stream.nextNoReturn();
        var exit = false;
        while(stream.peek() != null && !exit) {
            var token = stream.peek();
            if(token == null) {
                break;
            }
            switch(token.type) {
                case TokenType.identifier:
                case TokenType.quotedStringLiteral:
                case TokenType.stringLiteralPipe: {
                    list.push(this.parseParameter(stream));
                    break;

                }
                case TokenType.comma: {
                    //another parameter - consume this
                    stream.nextNoReturn();
                    break;

                }
                case TokenType.closeParenthesis: {
                    //consume close parenthesis
                    stream.nextNoReturn();
                    exit = true;
                    break;

                }
                default: {
                    //read until )
                    this.errors.push(new UnexpectedToken(token));
                    return list;
                    //throw new ParserException(token);
                    
                }
            }
        }
        return list;
    };
    Parser.prototype.parseParameter = function (stream) {
        var identifier = stream.next();
        switch(identifier.type) {
            case TokenType.stringLiteralPipe:
            case TokenType.quotedStringLiteral:
            case TokenType.stringLiteral:
            case TokenType.identifier: {
                break;

            }
            default: {
                //invalid token
                this.errors.push(new UnexpectedToken(identifier));
                //throw new ParserException(identifier);
                return null;

            }
        }
        //reduction
        return new Parameter(identifier.content);
    };
    Parser.prototype.parseAttributes = function (stream) {
        stream.next();
        var attributes = [];
        var token = null;
        var exit = false;
        while(stream.peek() != null && !exit) {
            token = stream.peek();
            if(token == null) {
                break;
            }
            switch(token.type) {
                case TokenType.identifier: {
                    attributes.push(this.parseAttribute(stream));
                    break;

                }
                case TokenType.closeBracket: {
                    //consume close bracket
                    stream.nextNoReturn();
                    exit = true;
                    break;

                }
                default: {
                    //invalid token
                    this.errors.push(new AttributeIdentifierMissing(token.index));
                    //throw new ParserException(token);
                    exit = true;

                }
            }
        }
        return attributes;
    };
    Parser.prototype.parseAttribute = function (stream) {
        var identifier = stream.next();
        var equalsToken = stream.peek();
        if(equalsToken != null && equalsToken.type == TokenType.equal) {
            stream.nextNoReturn();
            var valueToken = stream.peek();
            if(valueToken == null) {
                //TODO: Errors.Add(stream.Next());
                this.errors.push(new UnexpectedToken(identifier));
                return new Attribute(identifier.content, null);
                //throw new ParserException(string.Format("Unexpected end of stream"));
                            }
            if(valueToken.type == TokenType.closeBracket) {
                //then it's an invalid declaration
                this.errors.push(new AttributeValueMissing(valueToken.index));
            }
            var value = this.parseStatement(stream)[0];
            //force this as an attribute type
            if(value == null) {
            } else {
                switch(value.name) {
                    case "true":
                    case "false":
                    case "null": {
                        value = new StringLiteral("\"" + value.name + "\"", null, 0);
                        break;

                    }
                }
            }
            //reduction
            return new Attribute(identifier.content, value);
        }
        //single attribute only
        return new Attribute(identifier.content, null);
    };
    return Parser;
})();
var AttributeValueMissing = (function (_super) {
    __extends(AttributeValueMissing, _super);
    function AttributeValueMissing(index) {
        _super.call(this);
        this.index = index;
        this.message = "Attribute must have a value";
    }
    return AttributeValueMissing;
})(ParserError);
var AttributeIdentifierMissing = (function (_super) {
    __extends(AttributeIdentifierMissing, _super);
    function AttributeIdentifierMissing(index) {
        _super.call(this);
        this.index = index;
        this.message = "Invalid attribute name";
    }
    return AttributeIdentifierMissing;
})(ParserError);
var EndOfStream = (function (_super) {
    __extends(EndOfStream, _super);
    function EndOfStream() {
        _super.call(this);
        this.message = "Unexpected end of file.";
    }
    return EndOfStream;
})(ParserError);
var UnexpectedToken = (function (_super) {
    __extends(UnexpectedToken, _super);
    function UnexpectedToken(token) {
        _super.call(this);
        this.type = token.type;
        this.token = token.content;
        this.index = token.index;
        this.message = "Unexpected token: " + this.type;
    }
    return UnexpectedToken;
})(ParserError);
var Statement = (function () {
    function Statement(name, tail, index) {
        this.index = index;
        this.parameters = [];
        this.attributes = [];
        this.children = [];
        this.identifierParts = [];
        this.errors = [];
        this.name = null;
        var container = this;
        if(this.indexOfAny(name, [
            ".", 
            "#", 
            ":"
        ]) > -1) {
            this.getIdentifierParts(name, function (part) {
                switch(part.type) {
                    case IdentifierType.id: {
                        if(part.name.length == 0) {
                            container.errors.push(new MissingIdDeclaration(part.index - 1, 1));
                        } else {
                            if(container.anyAttributes(function (a) {
                                return a.key == "id";
                            })) {
                                container.errors.push(new MultipleIdDeclarations(part.name, part.index - 1, part.name.length + 1));
                            } else {
                                var literal = new StringLiteral("\"" + part.name + "\"", null, 0);
                                container.addAttribute(new Attribute("id", literal));
                            }
                        }
                        break;

                    }
                    case IdentifierType.class: {
                        if(part.name.length == 0) {
                            container.errors.push(new MissingClassDeclaration(1, 1));
                        } else {
                            var literal = new StringLiteral("\"" + part.name + "\"", null, 0);
                            container.addAttribute(new Attribute("class", literal));
                        }
                        break;

                    }
                    case IdentifierType.type: {
                        var literal = new StringLiteral("\"" + part.name + "\"", null, 0);
                        container.addAttribute(new Attribute("type", literal));
                        break;

                    }
                    case IdentifierType.literal: {
                        container.name = part.name ? part.name : null;
                        break;

                    }
                }
                container.identifierParts.push(part);
            });
        } else {
            container.name = name;
        }
        this.parseStatementTail(tail);
    }
    Statement.prototype.anyAttributes = function (callback) {
        for(var i in this.attributes) {
            if(callback(this.attributes[i])) {
                return true;
            }
        }
        return false;
    };
    Statement.prototype.parseStatementTail = function (tail) {
        if(tail != null) {
            if(tail.parameters != null) {
                this.parameters = tail.parameters;
            }
            if(tail.attributes != null) {
                for(var i in this.attributes) {
                    tail.attributes.push(this.attributes[i]);
                }
                this.attributes = tail.attributes;
            }
            if(tail.children != null) {
                this.children = tail.children;
            }
        }
    };
    Statement.prototype.addAttribute = function (node) {
        this.attributes.push(node);
    };
    Statement.prototype.identifierTypeFromCharacter = function (character, currentType) {
        switch(character) {
            case ":": {
                return IdentifierType.type;

            }
            case "#": {
                return IdentifierType.id;

            }
            case ".": {
                return IdentifierType.class;

            }
        }
        return currentType;
    };
    Statement.prototype.getIdentifierParts = function (source, callback) {
        var index = 0;
        var partType = IdentifierType.literal;
        var nextType = IdentifierType.none;
        for(var i = 0; i < source.length; i++) {
            nextType = this.identifierTypeFromCharacter(source.charAt(i), nextType);
            if(nextType != IdentifierType.none) {
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
    };
    Statement.prototype.indexOfAny = function (source, chars) {
        for(var i in chars) {
            var index = source.indexOf(chars[i]);
            if(index != -1) {
                return index;
            }
        }
        return -1;
    };
    return Statement;
})();
var MissingIdDeclaration = (function (_super) {
    __extends(MissingIdDeclaration, _super);
    function MissingIdDeclaration(index, length) {
        _super.call(this);
        this.index = index;
        this.length = length;
        this.message = "Missing Id declaration";
    }
    return MissingIdDeclaration;
})(ParserError);
var MissingClassDeclaration = (function (_super) {
    __extends(MissingClassDeclaration, _super);
    function MissingClassDeclaration(index, length) {
        _super.call(this);
        this.index = index;
        this.length = length;
        this.message = "Missing Class declaration";
    }
    return MissingClassDeclaration;
})(ParserError);
var MultipleIdDeclarations = (function (_super) {
    __extends(MultipleIdDeclarations, _super);
    function MultipleIdDeclarations(id, index, length) {
        _super.call(this);
        this.id = id;
        this.index = index;
        this.length = length;
        this.message = "Element may not have more than one id";
    }
    return MultipleIdDeclarations;
})(ParserError);
var Parameter = (function () {
    function Parameter(value) {
        this.value = value;
    }
    return Parameter;
})();
var StatementTail = (function () {
    function StatementTail() { }
    return StatementTail;
})();
var Attribute = (function () {
    function Attribute(key, value) {
        this.key = key;
        this.value = value;
    }
    return Attribute;
})();
var Identifier = (function () {
    function Identifier() { }
    return Identifier;
})();
var IdentifierType;
(function (IdentifierType) {
    IdentifierType._map = [];
    IdentifierType._map[0] = "id";
    IdentifierType.id = 0;
    IdentifierType._map[1] = "class";
    IdentifierType.class = 1;
    IdentifierType._map[2] = "literal";
    IdentifierType.literal = 2;
    IdentifierType._map[3] = "type";
    IdentifierType.type = 3;
    IdentifierType._map[4] = "none";
    IdentifierType.none = 4;
})(IdentifierType || (IdentifierType = {}));
var ValueType;
(function (ValueType) {
    ValueType._map = [];
    ValueType._map[0] = "stringLiteral";
    ValueType.stringLiteral = 0;
    ValueType._map[1] = "property";
    ValueType.property = 1;
    ValueType._map[2] = "local";
    ValueType.local = 2;
    ValueType._map[3] = "keyword";
    ValueType.keyword = 3;
})(ValueType || (ValueType = {}));
var StringLiteralPartType;
(function (StringLiteralPartType) {
    StringLiteralPartType._map = [];
    StringLiteralPartType._map[0] = "literal";
    StringLiteralPartType.literal = 0;
    StringLiteralPartType._map[1] = "encoded";
    StringLiteralPartType.encoded = 1;
    StringLiteralPartType._map[2] = "raw";
    StringLiteralPartType.raw = 2;
})(StringLiteralPartType || (StringLiteralPartType = {}));
var StringLiteralPart = (function () {
    function StringLiteralPart(type, data, index) {
        this.type = type;
        this.data = data;
        this.index = index;
        this.length = data.length;
    }
    return StringLiteralPart;
})();
var StringLiteral = (function (_super) {
    __extends(StringLiteral, _super);
    function StringLiteral(value, tail, index) {
        _super.call(this, "string", tail, index);
        this.values = [];
        this.valueType = ValueType.stringLiteral;
        if(this.isWrappedInQuotes(value)) {
            this.valueType = ValueType.stringLiteral;
            value = value.substring(1, 1 + value.length - 2);
        } else {
            if(value == "this") {
                this.valueType = ValueType.local;
            } else {
                if(value == "null" || value == "true" || value == "false") {
                    this.valueType = ValueType.keyword;
                } else {
                    this.valueType = ValueType.property;
                }
            }
        }
        if(this.valueType == ValueType.stringLiteral) {
            this.values = this.parse(value);
        }
    }
    StringLiteral.prototype.startsWith = function (source, value) {
        return source.length > 0 && source.charAt(0) == value;
    };
    StringLiteral.prototype.isWrappedInQuotes = function (value) {
        return (this.startsWith(value, "\"") || this.startsWith(value, "'"));
    };
    StringLiteral.prototype.parse = function (source) {
        var parts = [];
        var tempCounter = 0;
        var c = new Array(source.length);
        for(var i = 0; i < source.length; i++) {
            if(source.charAt(i) == "@" || source.charAt(i) == "=") {
                var comparer = source.charAt(i);
                var comparerType = (comparer == "@" ? StringLiteralPartType.encoded : StringLiteralPartType.raw);
                i++;
                if(source.charAt(Math.min(source.length - 1, i)) == comparer) {
                    c[tempCounter++] = comparer;
                } else {
                    if(this.isIdentifierHead(source.charAt(i))) {
                        if(tempCounter > 0) {
                            parts.push(new StringLiteralPart(StringLiteralPartType.literal, c.join(""), i - tempCounter));
                        }
                        tempCounter = 0;
                        var word = "";
                        for(; i < source.length; i++ , tempCounter++) {
                            if(!this.isIdTail(source.charAt(i))) {
                                break;
                            }
                            word += source.charAt(i);
                        }
                        if(word.charAt(word.length - 1) == ".") {
                            word = word.substring(0, word.length - 1);
                            parts.push(new StringLiteralPart(comparerType, word, i - tempCounter));
                            tempCounter = 0;
                            c[tempCounter++] = ".";
                        } else {
                            parts.push(new StringLiteralPart(comparerType, word, i - tempCounter));
                            tempCounter = 0;
                        }
                        if(i < source.length) {
                            c[tempCounter++] = source.charAt(i);
                        }
                    } else {
                        c[tempCounter++] = comparer;
                        i -= 1;
                    }
                }
            } else {
                c[tempCounter++] = source.charAt(i);
            }
        }
        if(tempCounter > 0) {
            parts.push(new StringLiteralPart(StringLiteralPartType.literal, c.join(""), source.length - tempCounter));
        }
        return parts;
    };
    StringLiteral.prototype.isIdentifierHead = function (character) {
        var isLetter = /[a-zA-Z]/;
        return character.match(isLetter) || character == "_" || character == "#" || character == ".";
    };
    StringLiteral.prototype.isIdTail = function (character) {
        var isNumber = /[0-9]/;
        return character.match(isNumber) || this.isIdentifierHead(character) || character == ":" || character == "-";
    };
    return StringLiteral;
})(Statement);
var StringLiteralPipe = (function (_super) {
    __extends(StringLiteralPipe, _super);
    function StringLiteralPipe(value, tail, index) {
        _super.call(this, "\"" + value + "\"", tail, index);
    }
    return StringLiteralPipe;
})(StringLiteral);
var EncodedOutput = (function (_super) {
    __extends(EncodedOutput, _super);
    function EncodedOutput(variableName, tail, index) {
        _super.call(this, "\"@" + variableName + "\"", tail, index);
    }
    return EncodedOutput;
})(StringLiteral);
var RawOutput = (function (_super) {
    __extends(RawOutput, _super);
    function RawOutput(variableName, tail, index) {
        _super.call(this, "\"=" + variableName + "\"", tail, index);
    }
    return RawOutput;
})(StringLiteral);
//@ sourceMappingURL=parser.js.map
