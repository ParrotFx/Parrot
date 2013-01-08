var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="./rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./tagBuilder.ts" />
///<reference path="./baseRenderer.ts" />
///<reference path="./selfClosingRenderer.ts" />
var IfRenderer = (function (_super) {
    __extends(IfRenderer, _super);
    function IfRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "";
        this.elements = [
            "if"
        ];
    }
    IfRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        this.rendererProvider = rendererProvider;
        //get the local model
        //var localModel = this.getLocalModelValue(host, statement, model);
        var localModel = this.getLocalModelValue(host, statement, model);
        if(localModel != null && localModel == true) {
            return this.renderChildren(statement, host, model, this.defaultChildTag);
        }
    };
    return IfRenderer;
})(HtmlRenderer);
//@ sourceMappingURL=ifRenderer.js.map
