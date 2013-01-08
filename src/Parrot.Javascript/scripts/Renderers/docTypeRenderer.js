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
var DocTypeRenderer = (function (_super) {
    __extends(DocTypeRenderer, _super);
    function DocTypeRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "div";
        this.elements = [
            "doctype"
        ];
    }
    DocTypeRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        var value = "html";
        if(statement.parameters.length > 0) {
            var localModel = this.getLocalModelValue(host, statement, model);
            value = localModel;
        }
        return "<!DOCTYPE " + value + ">";
    };
    return DocTypeRenderer;
})(BaseRenderer);
//@ sourceMappingURL=docTypeRenderer.js.map
