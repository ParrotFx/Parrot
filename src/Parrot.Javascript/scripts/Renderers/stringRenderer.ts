///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />

class StringRenderer implements IRenderer {
    defaultChildTag: string = "";
    rendererProvider: RendererProvider;
    elements: string[] = ["string"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        this.rendererProvider = rendererProvider;
        var result: string = "";
        if (statement instanceof StringLiteral) {
            var values = (<StringLiteral>statement).values
            for (var i in values) {
                result += this.getModelValue(host, model, values[i].type, values[i].data);
            }
        } else {
            result = this.getModelValue(host, model, StringLiteralPartType.encoded, statement.name);
        }

        return result;
    }

    getModelValue(host: any[], model: any, type: StringLiteralPartType, data: string): string {
        var provider = new ObjectModelValueProvider();
        var result = provider.getValue(host, model, null, data);
        if (result.result == true) {
            switch (type) {
                case StringLiteralPartType.encoded:
                    return encodeURI(result.value);
                case StringLiteralPartType.raw:
                    return result.value;
            }
        }

        return data;
    }
}