///<reference path="./tokenType.ts" />
var Token = (function () {
    function Token(index, content, type) {
        this.index = index;
        this.content = content;
        this.type = type;
    }
    return Token;
})();
//@ sourceMappingURL=token.js.map
