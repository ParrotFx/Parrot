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

class ConditionalRenderer extends HtmlRenderer {
    defaultChildTag: string = "div";
    rendererProvider: RendererProvider;
    elements: string[] = ["conditional"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        this.rendererProvider = rendererProvider;
        //get the local model
        //var localModel = this.getLocalModelValue(host, statement, model);
        var localModel = this.getLocalModelValue(host, statement, model);

        var statementToOutput = "default";

        if (localModel != null) {
            statementToOutput = localModel.toString();
        }

        for (var i in statement.children) {
            var child = statement.children[i];
            var value = "";

            var renderer = rendererProvider.getRenderer(child.name);
            if (renderer instanceof StringRenderer) {
                value = renderer.render(child, host, model, rendererProvider);
            } else {
                value = child.name;
            }

            if (value == statementToOutput) {
                return this.renderChildren(child, host, model, this.defaultChildTag);
            }

            for (var i in statement.children) {
                var child = statement.children[i];
                if (child.name == "default") {
                    return this.renderChildren(child, host, model, this.defaultChildTag);
                }
            }
        }
    }
}