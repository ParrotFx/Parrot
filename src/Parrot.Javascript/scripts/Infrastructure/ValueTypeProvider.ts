///<reference path="../Parser/parser.ts" />
class ValueTypeResult {
    type: ValueType;
    value: any;
    constructor(type: ValueType, value: any) {
        this.type = type;
        this.value = value;
    }
}

class ValueTypeProvider {
    keywordHandlers: any[];

    constructor() {
        this.keywordHandlers = [];
        this.keywordHandlers["this"] = function (s) { return new ValueTypeResult(ValueType.local, "this"); };
        this.keywordHandlers["false"] = function (s) { return new ValueTypeResult(ValueType.local, false); };
        this.keywordHandlers["true"] = function (s) { return new ValueTypeResult(ValueType.local, true); };
        this.keywordHandlers["null"] = function (s) { return new ValueTypeResult(ValueType.local, null); };
    }

    getValue(value: string) {
        var result = new ValueTypeResult(ValueType.stringLiteral, null);
        if (value == null) {
            return result;
        }

        if (this.isWrappedInQuotes(value)) {
            result.type = ValueType.stringLiteral;
            result.value = value.substring(1, 1 + value.length - 2);
        } else {
            if (this.keywordHandlers[value] != null) {
                result = this.keywordHandlers[value](value);
            } else {
                result.type = ValueType.property;
                result.value = value;
            }
        }

        return result;
    }

    isWrappedInQuotes(value: string): bool {
        return (this.startsWith(value, "\"") || this.startsWith(value, "'"));
    }

    startsWith(source: string, value: string): bool {
        return source.length > 0 && source.charAt(0) == value;
    }

}