///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
var StringRenderer = (function () {
    function StringRenderer() {
        this.defaultChildTag = "";
        this.elements = [
            "string"
        ];
    }
    StringRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        //get the local model
        this.rendererProvider = rendererProvider;
        var result = "";
        if(statement instanceof StringLiteral) {
            var values = (statement).values;
            for(var i in values) {
                result += this.getModelValue(host, model, values[i].type, values[i].data);
            }
        } else {
            result = this.getModelValue(host, model, StringLiteralPartType.encoded, statement.name);
        }
        return result;
    };
    StringRenderer.prototype.getModelValue = function (host, model, type, data) {
        var provider = new ObjectModelValueProvider();
        var result = provider.getValue(host, model, null, data);
        if(result.result == true) {
            switch(type) {
                case StringLiteralPartType.encoded: {
                    return encodeURI(result.value);

                }
                case StringLiteralPartType.raw: {
                    return result.value;

                }
            }
        }
        return data;
    };
    return StringRenderer;
})();
//@ sourceMappingURL=stringRenderer.js.map
