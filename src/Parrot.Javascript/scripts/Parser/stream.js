/// <reference path="../lexer/token.ts"/>
/// <reference path="../lexer/tokenType.ts"/>
var Stream = (function () {
    function Stream(source) {
        this._list = source;
        this._count = source.length;
        this._index = -1;
    }
    Stream.prototype.peek = function () {
        var temp = this._index + 1;
        while(temp < this._count) {
            if(this._list[temp].type != TokenType.whitespace) {
                return this._list[temp];
            }
            temp++;
        }
        return null;
    };
    Stream.prototype.getNextNoReturn = function () {
        this._index++;
        while(this._index < this._count && this._list[this._index].type == TokenType.whitespace) {
            this._index++;
        }
    };
    Stream.prototype.nextNoReturn = function () {
        this.getNextNoReturn();
    };
    Stream.prototype.next = function () {
        this._index++;
        while(this._index < this._count) {
            if(this._list[this._index].type != TokenType.whitespace) {
                return this._list[this._index];
            }
            this._index++;
        }
        return null;
    };
    return Stream;
})();
//@ sourceMappingURL=stream.js.map
