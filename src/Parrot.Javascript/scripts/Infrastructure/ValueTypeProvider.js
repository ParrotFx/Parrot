///<reference path="../Parser/parser.ts" />
var ValueTypeResult = (function () {
    function ValueTypeResult(type, value) {
        this.type = type;
        this.value = value;
    }
    return ValueTypeResult;
})();
var ValueTypeProvider = (function () {
    function ValueTypeProvider() {
        this.keywordHandlers = [];
        this.keywordHandlers["this"] = function (s) {
            return new ValueTypeResult(ValueType.local, "this");
        };
        this.keywordHandlers["false"] = function (s) {
            return new ValueTypeResult(ValueType.local, false);
        };
        this.keywordHandlers["true"] = function (s) {
            return new ValueTypeResult(ValueType.local, true);
        };
        this.keywordHandlers["null"] = function (s) {
            return new ValueTypeResult(ValueType.local, null);
        };
    }
    ValueTypeProvider.prototype.getValue = function (value) {
        var result = new ValueTypeResult(ValueType.stringLiteral, null);
        if(value == null) {
            return result;
        }
        if(this.isWrappedInQuotes(value)) {
            result.type = ValueType.stringLiteral;
            result.value = value.substring(1, 1 + value.length - 2);
        } else {
            if(this.keywordHandlers[value] != null) {
                result = this.keywordHandlers[value](value);
            } else {
                result.type = ValueType.property;
                result.value = value;
            }
        }
        return result;
    };
    ValueTypeProvider.prototype.isWrappedInQuotes = function (value) {
        return (this.startsWith(value, "\"") || this.startsWith(value, "'"));
    };
    ValueTypeProvider.prototype.startsWith = function (source, value) {
        return source.length > 0 && source.charAt(0) == value;
    };
    return ValueTypeProvider;
})();
//@ sourceMappingURL=ValueTypeProvider.js.map
