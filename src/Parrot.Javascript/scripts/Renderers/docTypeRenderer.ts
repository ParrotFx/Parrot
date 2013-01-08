///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="./rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./tagBuilder.ts" />
///<reference path="./baseRenderer.ts" />

class DocTypeRenderer extends BaseRenderer implements IRenderer {
    defaultChildTag: string = "div";
    rendererProvider: RendererProvider;
    elements: string[] = ["doctype"];
    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        var value: string = "html";

        if (statement.parameters.length > 0) {
            var localModel = this.getLocalModelValue(host, statement, model);
            value = localModel;
        }

        return "<!DOCTYPE " + value + ">";
    }
}