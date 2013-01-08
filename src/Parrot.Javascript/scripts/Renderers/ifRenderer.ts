///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="./rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./tagBuilder.ts" />
///<reference path="./baseRenderer.ts" />
///<reference path="./selfClosingRenderer.ts" />

class IfRenderer extends HtmlRenderer {
    defaultChildTag: string = "";
    rendererProvider: RendererProvider;
    elements: string[] = ["if"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        this.rendererProvider = rendererProvider;
        //get the local model
        //var localModel = this.getLocalModelValue(host, statement, model);
        var localModel = this.getLocalModelValue(host, statement, model);

        if (localModel != null && localModel == true) {
            return this.renderChildren(statement, host, model, this.defaultChildTag);
        }
    }
}