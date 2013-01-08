/// <reference path="../lexer/token.ts"/>
/// <reference path="../lexer/tokenType.ts"/>
class Stream {
    private _list: Token[];
    private _index: number;
    private _count: number;

    constructor(source: Token[]) { 
        this._list = source;
        this._count = source.length;
        this._index = -1;
    }

    peek() : Token {
        var temp = this._index + 1;
        while (temp < this._count) {
            if (this._list[temp].type != TokenType.whitespace) {
                return this._list[temp];
            }

            temp++;
        }

        return null;
    }
    
    getNextNoReturn() {
        this._index++;
        while (this._index < this._count && this._list[this._index].type == TokenType.whitespace) {
            this._index++;
        }
    }

    nextNoReturn() {
        this.getNextNoReturn();
    }

    next() : Token {
        this._index++;
        while (this._index < this._count) {
            if (this._list[this._index].type != TokenType.whitespace) {
                return this._list[this._index];
            }

            this._index++;
        }

        return null;
    }
}