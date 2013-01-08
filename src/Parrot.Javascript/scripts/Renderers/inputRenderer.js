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
var InputRenderer = (function (_super) {
    __extends(InputRenderer, _super);
    function InputRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "";
        this.elements = [
            "input"
        ];
    }
    InputRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        //get the local model
        //var localModel = this.getLocalModelValue(host, statement, model);
        var type = this.getType(statement, host, model);
        switch(type) {
            case "checkbox":
            case "radio": {
                for(var i = 0; i < statement.attributes.length; i++) {
                    var attribute = statement.attributes[i];
                    if(attribute.key == "checked") {
                        var value = this.renderAttribute(attribute, host, model);
                        switch(value) {
                            case "true": {
                                statement.attributes[i] = new Attribute(attribute.key, new StringLiteral("\"checked\"", null, 0));
                                break;

                            }
                            case "false":
                            case "null": {
                                statement.attributes.splice(i, 1);
                                i -= 1;
                                break;

                            }
                        }
                    }
                }
                break;

            }
        }
        return _super.prototype.render.call(this, statement, host, model, rendererProvider);
    };
    InputRenderer.prototype.getType = function (statement, host, model) {
        for(var i = 0; i < statement.attributes.length; i++) {
            var attribute = statement.attributes[i];
            if(attribute.key == "type") {
                return this.renderAttribute(attribute, host, model);
            }
        }
        return "hidden";
    };
    return InputRenderer;
})(SelfClosingRenderer);
//@ sourceMappingURL=inputRenderer.js.map
