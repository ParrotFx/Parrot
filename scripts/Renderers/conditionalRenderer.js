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
///<reference path="./stringRenderer.ts" />
var ConditionalRenderer = (function (_super) {
    __extends(ConditionalRenderer, _super);
    function ConditionalRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "div";
        this.elements = [
            "conditional"
        ];
    }
    ConditionalRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        this.rendererProvider = rendererProvider;
        //get the local model
        //var localModel = this.getLocalModelValue(host, statement, model);
        var localModel = this.getLocalModelValue(host, statement, model);
        var statementToOutput = "default";
        if(localModel != null) {
            statementToOutput = localModel.toString();
        }
        for(var i in statement.children) {
            var child = statement.children[i];
            var value = "";
            var renderer = rendererProvider.getRenderer(child.name);
            if(renderer instanceof StringRenderer) {
                value = renderer.render(child, host, model, rendererProvider);
            } else {
                value = child.name;
            }
            if(value == statementToOutput) {
                return this.renderChildren(child, host, model, this.defaultChildTag);
            }
            for(var i in statement.children) {
                var child = statement.children[i];
                if(child.name == "default") {
                    return this.renderChildren(child, host, model, this.defaultChildTag);
                }
            }
        }
    };
    return ConditionalRenderer;
})(HtmlRenderer);
//@ sourceMappingURL=conditionalRenderer.js.map
