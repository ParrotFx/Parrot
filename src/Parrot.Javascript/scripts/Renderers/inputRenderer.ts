///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="./rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./tagBuilder.ts" />
///<reference path="./baseRenderer.ts" />
///<reference path="./selfClosingRenderer.ts" />

class InputRenderer extends SelfClosingRenderer {
    defaultChildTag: string = "";
    rendererProvider: RendererProvider;
    elements: string[] = ["input"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        //var localModel = this.getLocalModelValue(host, statement, model);
        var type = this.getType(statement, host, model);
        switch (type) {
            case "checkbox":
            case "radio":
                for (var i = 0; i < statement.attributes.length; i++) {
                    var attribute = statement.attributes[i];
                    if (attribute.key == "checked") {
                        var value = this.renderAttribute(attribute, host, model);
                        switch (value) {
                            case "true":
                                statement.attributes[i] = new Attribute(attribute.key, new StringLiteral("\"checked\"", null, 0));
                                break;
                            case "false":
                            case "null":
                                statement.attributes.splice(i, 1);
                                i -= 1;
                                break;
                        }
                    }
                }
            break;
        }

        return super.render(statement, host, model, rendererProvider);
    }

    getType(statement: Statement, host: any[], model: any) {
        for (var i = 0; i < statement.attributes.length; i++) {
            var attribute = statement.attributes[i];
            if (attribute.key == "type") {
                return this.renderAttribute(attribute, host, model);
            }
        }

        return "hidden";
    }
}