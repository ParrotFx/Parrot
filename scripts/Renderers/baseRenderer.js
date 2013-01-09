///<reference path="../Parser/parser.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
var BaseRenderer = (function () {
    function BaseRenderer() { }
    BaseRenderer.prototype.getlocal = function () {
    };
    BaseRenderer.prototype.getLocalModelValue = function (host, statement, model) {
        var modelValueProvider = new ObjectModelValueProvider();
        if(statement.parameters.length > 0) {
            var result = modelValueProvider.getValue(host, model, null, statement.parameters[0].value);
            if(result.result == true) {
                return result.value;
            }
        }
        if(model != null) {
            var result = modelValueProvider.getValue(host, model, ValueType.property, null);
            if(result.result == true) {
                return result.value;
            }
        }
        return model;
    };
    return BaseRenderer;
})();
//@ sourceMappingURL=baseRenderer.js.map
