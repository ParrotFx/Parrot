var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./htmlRenderer.ts" />
var SelfClosingRenderer = (function (_super) {
    __extends(SelfClosingRenderer, _super);
    function SelfClosingRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "";
        this.elements = [
            "base", 
            "basefont", 
            "frame", 
            "link", 
            "meta", 
            "area", 
            "br", 
            "col", 
            "hr", 
            "img", 
            "param"
        ];
    }
    SelfClosingRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        //get the local model
        var localModel = this.getLocalModelValue(host, statement, model);
        return this.createTag(statement, host, localModel);
    };
    SelfClosingRenderer.prototype.createTag = function (statement, host, model) {
        var tagName = statement.name == null || statement.name.length == 0 ? this.defaultChildTag : statement.name;
        var builder = new TagBuilder(tagName);
        this.renderAttributes(host, model, statement, builder);
        return builder.toString(TagRenderMode.selfClosing);
    };
    return SelfClosingRenderer;
})(HtmlRenderer);
//@ sourceMappingURL=selfClosingRenderer.js.map
