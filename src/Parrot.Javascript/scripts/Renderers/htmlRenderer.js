var RendererProvider = (function () {
    function RendererProvider() {
        this.renderers = [];
        this.renderers.push(new HtmlRenderer());
        this.renderers.push(new StringRenderer());
    }
    RendererProvider.prototype.getRenderer = function (type) {
        for(var i in this.renderers) {
            for(var g in this.renderers[i].elements) {
                if(type == this.renderers[i].elements[g]) {
                    return this.renderers[i];
                }
            }
        }
        return this.getRenderer("*");
    };
    return RendererProvider;
})();
var ParrotDocumentView = (function () {
    function ParrotDocumentView(parrotDocument) {
        this.parrotDocument = parrotDocument;
        this.rendererProvider = new RendererProvider();
    }
    ParrotDocumentView.prototype.render = function (host, model) {
        var result = "";
        for(var i in this.parrotDocument.children) {
            var child = this.parrotDocument.children[i];
            var renderer = this.rendererProvider.getRenderer(child.name);
            result += renderer.render(child, host, model, this.rendererProvider);
        }
        return result;
    };
    return ParrotDocumentView;
})();
var HtmlRenderer = (function () {
    function HtmlRenderer() {
        this.defaultChildTag = "div";
        this.elements = [
            "*"
        ];
    }
    HtmlRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        this.rendererProvider = rendererProvider;
        var localModel = this.getLocalModelValue(host, statement, model);
        return this.createTag(statement, host, localModel);
    };
    HtmlRenderer.prototype.createTag = function (statement, host, model) {
        var tagName = statement.name == null ? this.defaultChildTag : statement.name;
        var builder = new TagBuilder(tagName);
        this.renderAttributes(host, model, statement, builder);
        var render = "";
        render += builder.toString(TagRenderMode.startTag);
        if(statement.children.length > 0) {
            render += this.renderChildren(statement, host, model, this.defaultChildTag);
        }
        render += builder.toString(TagRenderMode.endTag);
        return render;
    };
    HtmlRenderer.prototype.renderAttribute = function (attribute, host, model) {
        var renderer = new StringRenderer();
        var result = "";
        result = renderer.render(attribute.value, host, model, this.rendererProvider);
        return result;
    };
    HtmlRenderer.prototype.renderAttributes = function (host, model, statement, builder) {
        for(var i in statement.attributes) {
            var attribute = statement.attributes[i];
            if(attribute.value == null) {
                builder.mergeAttribute(attribute.key, attribute.key, true);
            } else {
                var attributeValue = this.renderAttribute(attribute, host, model);
                if(attribute.key == "class") {
                    builder.addCssClass(attributeValue);
                } else {
                    builder.mergeAttribute(attribute.key, attributeValue, true);
                }
            }
        }
    };
    HtmlRenderer.prototype.renderChildren = function (statement, host, model, defaultTag) {
        if(defaultTag == null || defaultTag.length == 0) {
            defaultTag = this.defaultChildTag;
        }
        if(Object.prototype.toString.call(model) === '[object Array]') {
            var result = "";
            for(var i in model) {
                result += this._renderChildren(statement.children, host, model[i], defaultTag);
            }
            return result;
        }
        return this._renderChildren(statement.children, host, model, defaultTag);
    };
    HtmlRenderer.prototype._renderChildren = function (children, host, model, defaultTag) {
        var result = "";
        for(var i in children) {
            var child = children[i];
            var renderer = this.rendererProvider.getRenderer(child.name);
            result += renderer.render(child, host, model, this.rendererProvider);
        }
        return result;
    };
    HtmlRenderer.prototype.getLocalModelValue = function (host, statement, model) {
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
    return HtmlRenderer;
})();
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
var NullReferenceException = (function () {
    function NullReferenceException(parameter) {
        this.parameter = parameter;
    }
    return NullReferenceException;
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
var ValueTypeResult = (function () {
    function ValueTypeResult(type, value) {
        this.type = type;
        this.value = value;
    }
    return ValueTypeResult;
})();
var TagBuilder = (function () {
    function TagBuilder(name) {
        this.name = name;
        this.innerHtml = "";
        this.attributes = [];
    }
    TagBuilder.prototype.toString = function (renderMode) {
        switch(renderMode) {
            case TagRenderMode.startTag: {
                return "<" + this.name + this.appendAttributes() + ">";

            }
            case TagRenderMode.endTag: {
                return "</" + this.name + ">";

            }
            case TagRenderMode.selfClosing: {
                return "<" + this.name + this.appendAttributes() + " />";

            }
            default: {
                return "<" + this.name + this.appendAttributes() + this.innerHtml + "</" + this.name + ">";

            }
        }
    };
    TagBuilder.prototype.appendAttributes = function () {
        var render = "";
        for(var i in this.attributes) {
            var attribute = this.attributes[i];
            var key = i;
            var value = attribute;
            if(key == "id" && value == null || value.length == 0) {
                continue;
            }
            if(value != null) {
                value = this.htmlAttributeEncode(value);
            } else {
                value = key;
            }
            render += " " + key + "=\"" + value + "\"";
        }
        return render;
    };
    TagBuilder.prototype.htmlAttributeEncode = function (value) {
        return value.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;").replace("'", "&#39;");
    };
    TagBuilder.prototype.mergeAttribute = function (key, value, replaceExisting) {
        if(key == null || key.length == 0) {
            throw new ArgumentException("key");
        }
        if(replaceExisting || !this.containsKey(this.attributes, key)) {
            this.attributes[key] = value;
        }
    };
    TagBuilder.prototype.containsKey = function (source, key) {
        for(var i in source) {
            if(i == key) {
                return true;
            }
        }
        return false;
    };
    TagBuilder.prototype.addCssClass = function (value) {
        if(this.attributes["class"] != undefined && this.attributes["class"] != null) {
            this.attributes["class"] = value + " " + this.attributes["class"];
        } else {
            this.attributes["class"] = value;
        }
    };
    return TagBuilder;
})();
var ArgumentException = (function () {
    function ArgumentException(argument) {
        this.argument = argument;
    }
    return ArgumentException;
})();
var TagRenderMode;
(function (TagRenderMode) {
    TagRenderMode._map = [];
    TagRenderMode._map[0] = "startTag";
    TagRenderMode.startTag = 0;
    TagRenderMode._map[1] = "endTag";
    TagRenderMode.endTag = 1;
    TagRenderMode._map[2] = "selfClosing";
    TagRenderMode.selfClosing = 2;
    TagRenderMode._map[3] = "normal";
    TagRenderMode.normal = 3;
})(TagRenderMode || (TagRenderMode = {}));
var StringRenderer = (function () {
    function StringRenderer() {
        this.defaultChildTag = "";
        this.elements = [
            "string"
        ];
    }
    StringRenderer.prototype.render = function (statement, host, model, rendererProvider) {
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
