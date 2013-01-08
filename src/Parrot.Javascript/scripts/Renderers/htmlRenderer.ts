///<reference path="../Parser/parser.ts" />

class RendererProvider {
    renderers: any[];
    constructor() {
        this.renderers = [];
        this.renderers.push(new HtmlRenderer());
        this.renderers.push(new StringRenderer());
    }

    getRenderer(type: string) : any {
        for (var i in this.renderers) {
            for (var g in this.renderers[i].elements) {
                if (type == this.renderers[i].elements[g]) {
                    return this.renderers[i];
                }
            }
        }

        return this.getRenderer("*");
    }
}

class ParrotDocumentView {
    parrotDocument: ParrotDocument;
    rendererProvider: RendererProvider;
    
    constructor(parrotDocument: ParrotDocument) {
        this.parrotDocument = parrotDocument;
        this.rendererProvider = new RendererProvider();
    }

    render(host: any[], model: any): string {
        var result: string = "";
        for (var i in this.parrotDocument.children) {
            var child = this.parrotDocument.children[i];

            var renderer = this.rendererProvider.getRenderer(child.name);

            result += renderer.render(child, host, model, this.rendererProvider);
        }

        return result;
    }
}

interface IRenderer {
    defaultChildTag: string;
    elements: string[];
    rendererProvider: RendererProvider;
    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider);
}

class HtmlRenderer implements IRenderer {
    defaultChildTag: string = "div";
    rendererProvider: RendererProvider;
    elements: string[] = ["*"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        this.rendererProvider = rendererProvider;

        var localModel = this.getLocalModelValue(host, statement, model);

        return this.createTag(statement, host, localModel);
    }

    createTag(statement: Statement, host: any[], model: any): string {
        var tagName: string = statement.name == null ? this.defaultChildTag : statement.name;

        var builder = new TagBuilder(tagName);

        this.renderAttributes(host, model, statement, builder);

        var render = "";
        render += builder.toString(TagRenderMode.startTag);

        if (statement.children.length > 0) {
            render += this.renderChildren(statement, host, model, this.defaultChildTag);
        }

        render += builder.toString(TagRenderMode.endTag);

        return render;
    }

    renderAttribute(attribute: Attribute, host: any[], model: any): string {
        var renderer = new StringRenderer();
        var result: string = "";

        result = renderer.render(attribute.value, host, model, this.rendererProvider);

        return result;
    }

    renderAttributes(host: any[], model: any, statement: Statement, builder: TagBuilder): void {
        for (var i in statement.attributes) {
            var attribute = statement.attributes[i];
            if (attribute.value == null) {
                builder.mergeAttribute(attribute.key, attribute.key, true);
            } else {
                var attributeValue = this.renderAttribute(attribute, host, model);
                if (attribute.key == "class") {
                    builder.addCssClass(attributeValue);
                } else {
                    builder.mergeAttribute(attribute.key, attributeValue, true);
                }
            }
        }
    }

    renderChildren(statement: Statement, host: any[], model: any, defaultTag: string): string {
        if (defaultTag == null || defaultTag.length == 0) {
            defaultTag = this.defaultChildTag;
        }

        if (Object.prototype.toString.call(model) === '[object Array]') {
            var result: string = "";
            for (var i in model) {
                result += this._renderChildren(statement.children, host, model[i], defaultTag);
            }
            return result;
        }

        return this._renderChildren(statement.children, host, model, defaultTag);
    }

    _renderChildren(children: Statement[], host: any[], model: any, defaultTag: string): string {
        var result: string = "";
        for (var i in children) {
            var child = children[i];

            var renderer = this.rendererProvider.getRenderer(child.name);
            result += renderer.render(child, host, model, this.rendererProvider);
        }

        return result;
    }

    getLocalModelValue(host: any[], statement: Statement, model: any): any {
        var modelValueProvider = new ObjectModelValueProvider();

        if (statement.parameters.length > 0) {
            var result = modelValueProvider.getValue(host, model, null, statement.parameters[0].value);
            if (result.result == true) {
                return result.value;
            }
        }

        if (model != null) {
            var result = modelValueProvider.getValue(host, model, ValueType.property, null);
            if (result.result == true) {
                return result.value;
            }
        }

        return model;
    }
}

class ObjectModelValueProvider {
    valueTypeProvider: ValueTypeProvider;

    constructor() {
        this.valueTypeProvider = new ValueTypeProvider();
    }

    getValue(host: any[], model: any, type: ValueType, property: any) {
        var valueType = type;
        if (valueType == null) {
            var temp = this.getValueType(property);
            property = temp.property;
            valueType = temp.type;
        }

        switch (valueType) {
            case ValueType.stringLiteral:
            case ValueType.keyword:
                return {
                    value: property,
                    result: true
                };
            case ValueType.local:
                return {
                    value: model,
                    result: true
                };
            case ValueType.property:
                if (model != null) {
                    var result = this.getModelProperty(model, property);
                    if (result.result == true) {
                        return result;
                    }
                }

                if (host != null) {
                    var result = this.getModelProperty(host, property);
                    if (result.result == true) {
                        return result;
                    }
                }
                break;
        }

        return {
            value: null,
            result: false
        };
    }

    getModelProperty(model: any, property: any) {
        if (property != null) {
            var stringProperty = property;
            var parameters: string[] = stringProperty.split(".");

            if (model == null && parameters.length != 1) {
                throw new NullReferenceException(parameters[0]);
            }

            if (parameters[0].length > 0) {
                for (var key in model) {
                    if (key == parameters[0]) {
                        var tempObject = model[key];
                        if (parameters.length == 1) {
                            return {
                                value: tempObject,
                                result: true
                            }
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
    }

    getValueType(property: any) {
        if (property != null) {
            var result = this.valueTypeProvider.getValue(property);
            return {
                property: result.value,
                type: result.type
            };
        }

        return {
            property: property,
            type: ValueType.keyword
        }
    }
}

class NullReferenceException {
    parameter: string;
    constructor(parameter: string) {
        this.parameter = parameter;
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

class ValueTypeResult {
    type: ValueType;
    value: any;
    constructor(type: ValueType, value: any) {
        this.type = type;
        this.value = value;
    }
}

class TagBuilder {
    name: string;
    innerHtml: string;
    attributes: any[];

    constructor(name: string) {
        this.name = name;
        this.innerHtml = "";
        this.attributes = [];
    }

    toString(renderMode: TagRenderMode): string {
        switch (renderMode) {
            case TagRenderMode.startTag:
                return "<" + this.name + this.appendAttributes() + ">";
            case TagRenderMode.endTag:
                return "</" + this.name + ">";
            case TagRenderMode.selfClosing:
                return "<" + this.name + this.appendAttributes() + " />";
            default:
                return "<" + this.name + this.appendAttributes() + this.innerHtml + "</" + this.name + ">";
        }
    }

    appendAttributes(): string {
        var render: string = "";
        for (var i in this.attributes) {
            var attribute = this.attributes[i];
            var key: string = i;
            var value = attribute;

            if (key == "id" && value == null || value.length == 0) {
                continue;
            }

            if (value != null) {
                value = this.htmlAttributeEncode(value);
            } else {
                value = key;
            }

            render += " " + key + "=\"" + value + "\"";
        }

        return render;
    }

    htmlAttributeEncode(value: string): string {
        return value.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;").replace("'", "&#39;");
    }

    mergeAttribute(key: string, value: string, replaceExisting: bool) {
        if (key == null || key.length == 0) {
            throw new ArgumentException("key");
        }

        if (replaceExisting || !this.containsKey(this.attributes, key)) {
            this.attributes[key] = value;
        }
    }

    containsKey(source: any[], key: string) {
        for (var i in source) {
            if (i == key) {
                return true;
            }
        }

        return false;
    }

    addCssClass(value: string): void {
        if (this.attributes["class"] != undefined && this.attributes["class"] != null) {
            this.attributes["class"] = value + " " + this.attributes["class"];
        } else {
            this.attributes["class"] = value;
        }
    }
}

class ArgumentException {
    argument: string;
    constructor(argument: string) {
        this.argument = argument;
    }
}

enum TagRenderMode {
    startTag,
    endTag,
    selfClosing,
    normal
}

class StringRenderer implements IRenderer {
    defaultChildTag: string = "";
    rendererProvider: RendererProvider;
    elements: string[] = ["string"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        this.rendererProvider = rendererProvider;
        var result: string = "";
        if (statement instanceof StringLiteral) {
            var values = (<StringLiteral>statement).values
            for (var i in values) {
                result += this.getModelValue(host, model, values[i].type, values[i].data);
            }
        } else {
            result = this.getModelValue(host, model, StringLiteralPartType.encoded, statement.name);
        }

        return result;
    }

    getModelValue(host: any[], model: any, type: StringLiteralPartType, data: string): string {
        var provider = new ObjectModelValueProvider();
        var result = provider.getValue(host, model, null, data);
        if (result.result == true) {
            switch (type) {
                case StringLiteralPartType.encoded:
                    return encodeURI(result.value);
                case StringLiteralPartType.raw:
                    return result.value;
            }
        }

        return data;
    }
}