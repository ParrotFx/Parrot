///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./htmlRenderer.ts" />

class SelfClosingRenderer extends HtmlRenderer {
    defaultChildTag: string = "";
    rendererProvider: RendererProvider;
    elements: string[] = ["base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        var localModel = this.getLocalModelValue(host, statement, model);

        return this.createTag(statement, host, localModel);
    }

    createTag(statement: Statement, host: any[], model: any): string {
        var tagName: string = statement.name == null || statement.name.length == 0 ? this.defaultChildTag : statement.name;
        var builder = new TagBuilder(tagName);
        this.renderAttributes(host, model, statement, builder);

        return builder.toString(TagRenderMode.selfClosing);
    }
}