///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="./rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./tagBuilder.ts" />
///<reference path="./baseRenderer.ts" />

class HtmlRenderer extends BaseRenderer implements IRenderer {
    defaultChildTag: string = "div";
    rendererProvider: RendererProvider;
    elements: string[] = ["*"];

    render(statement: Statement, host: any[], model: any, rendererProvider: RendererProvider): string {
        //get the local model
        this.rendererProvider = rendererProvider;

        var localModel = this.getLocalModelValue(host, statement, model);

        return this.createTag(statement, host, localModel);
    }

    createTag(statement: Statement, host: any[], model: any): string {
        var tagName: string = statement.name == null ? this.defaultChildTag : statement.name;

        var builder = new TagBuilder(tagName);

        this.renderAttributes(host, model, statement, builder);

        var render = "";
        render += builder.toString(TagRenderMode.startTag);

        if (statement.children.length > 0) {
            render += this.renderChildren(statement, host, model, this.defaultChildTag);
        }

        render += builder.toString(TagRenderMode.endTag);

        return render;
    }

    renderAttribute(attribute: Attribute, host: any[], model: any): string {
        var renderer = new StringRenderer();
        var result: string = "";

        result = renderer.render(attribute.value, host, model, this.rendererProvider);

        return result;
    }

    renderAttributes(host: any[], model: any, statement: Statement, builder: TagBuilder): void {
        for (var i in statement.attributes) {
            var attribute = statement.attributes[i];
            if (attribute.value == null) {
                builder.mergeAttribute(attribute.key, attribute.key, true);
            } else {
                var attributeValue = this.renderAttribute(attribute, host, model);
                if (attribute.key == "class") {
                    builder.addCssClass(attributeValue);
                } else {
                    builder.mergeAttribute(attribute.key, attributeValue, true);
                }
            }
        }
    }

    renderChildren(statement: Statement, host: any[], model: any, defaultTag: string): string {
        if (defaultTag == null || defaultTag.length == 0) {
            defaultTag = this.defaultChildTag;
        }

        if (Object.prototype.toString.call(model) === '[object Array]') {
            var result: string = "";
            for (var i in model) {
                result += this._renderChildren(statement.children, host, model[i], defaultTag);
            }
            return result;
        }

        return this._renderChildren(statement.children, host, model, defaultTag);
    }

    _renderChildren(children: Statement[], host: any[], model: any, defaultTag: string): string {
        var result: string = "";
        for (var i in children) {
            var child = children[i];

            var renderer = this.rendererProvider.getRenderer(child.name);
            result += renderer.render(child, host, model, this.rendererProvider);
        }

        return result;
    }
}