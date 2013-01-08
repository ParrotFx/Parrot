///<reference path="../Parser/parser.ts" />
///<reference path="ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
var ObjectModelValueProvider = (function () {
    function ObjectModelValueProvider() {
        this.valueTypeProvider = new ValueTypeProvider();
    }
    ObjectModelValueProvider.prototype.getValue = function (host, model, type, property) {
        var valueType = type;
        if(valueType == null) {
            var temp = this.getValueType(property);
            property = temp.property;
            valueType = temp.type;
        }
        switch(valueType) {
            case ValueType.stringLiteral:
            case ValueType.keyword: {
                return {
                    value: property,
                    result: true
                };

            }
            case ValueType.local: {
                return {
                    value: model,
                    result: true
                };

            }
            case ValueType.property: {
                if(model != null) {
                    var result = this.getModelProperty(model, property);
                    if(result.result == true) {
                        return result;
                    }
                }
                if(host != null) {
                    var result = this.getModelProperty(host, property);
                    if(result.result == true) {
                        return result;
                    }
                }
                break;

            }
        }
        return {
            value: null,
            result: false
        };
    };
    ObjectModelValueProvider.prototype.getModelProperty = function (model, property) {
        if(property != null) {
            var stringProperty = property;
            var parameters = stringProperty.split(".");
            if(model == null && parameters.length != 1) {
                throw new NullReferenceException(parameters[0]);
            }
            if(parameters[0].length > 0) {
                for(var key in model) {
                    if(key == parameters[0]) {
                        var tempObject = model[key];
                        if(parameters.length == 1) {
                            return {
                                value: tempObject,
                                result: true
                            };
                        }
                        return this.getModelProperty(tempObject, parameters.slice(1).join("."));
                    }
                }
                if(model["__locals"] != undefined && model["__locals"] != null) {
                    var locals = model["__locals"];
                    for(var i = locals.length - 1; i >= 0; i--) {
                        var local = locals[i];
                        var result = this.getModelProperty(local, property);
                        if(result.result == true) {
                            return result;
                        }
                    }
                }
            } else {
                return {
                    value: model,
                    result: true
                };
            }
        }
        return {
            value: null,
            result: false
        };
    };
    ObjectModelValueProvider.prototype.getValueType = function (property) {
        if(property != null) {
            var result = this.valueTypeProvider.getValue(property);
            return {
                property: result.value,
                type: result.type
            };
        }
        return {
            property: property,
            type: ValueType.keyword
        };
    };
    return ObjectModelValueProvider;
})();
//@ sourceMappingURL=ObjectModelValueProvider.js.map
