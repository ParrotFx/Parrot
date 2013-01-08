var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var HtmlRenderer = (function (_super) {
    __extends(HtmlRenderer, _super);
    function HtmlRenderer() {
        _super.apply(this, arguments);

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
    return HtmlRenderer;
})(BaseRenderer);
