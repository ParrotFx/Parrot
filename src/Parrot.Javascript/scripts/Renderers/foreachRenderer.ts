///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../Infrastructure/Locals.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./htmlRenderer.ts" />

class ForEachRenderer extends HtmlRenderer {
    defaultChildTag: string = "li";
    rendererProvider: RendererProvider;
    elements: string[] = ["foreach"];
    
    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        this.rendererProvider = rendererProvider;

        var localModel = this.getLocalModelValue(host, statement, model);

        if (Object.prototype.toString.call(model) === '[object Array]') {
            var result: string = "";
            for (var i in model) {
                var locals = new Locals(host);
                locals.push(this.iteratorItem(i, model));
                result += this._renderChildren(statement.children, host, model[i], this.defaultChildTag);
                locals.pop();
            }
            return result;
        }

        return "";
    }

    iteratorItem(index: any, items: any[]): any {
        return {
            _first: index == 0,
            _last: index == items.length - 1,
            _index: index,
            _even: index % 2 == 0,
            _odd: index % 2 == 1
        }
    }
}