///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../Infrastructure/Locals.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./htmlRenderer.ts" />

class ListRenderer extends HtmlRenderer {
    defaultChildTag: string = "li";
    rendererProvider: RendererProvider;
    elements: string[] = ["ul", "ol"];

    renderChildren(statement: Statement, host: any[], model: any, defaultTag: string): string {
        console.log("rendering children: list renderer");
        if (defaultTag == null || defaultTag.length == 0) {
            defaultTag = this.defaultChildTag;
        }

        if (Object.prototype.toString.call(model) === '[object Array]') {
            var result: string = "";
            for (var i in model) {
                var locals = new Locals(host);
                locals.push(this.iteratorItem(i, model));
                result += this._renderChildren(statement.children, host, model[i], defaultTag);
                locals.pop();
            }
            return result;
        }

        return this._renderChildren(statement.children, host, model, defaultTag);
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
