var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
///<reference path="../Parser/parser.ts" />
///<reference path="./irenderer.ts" />
///<reference path="rendererProvider.ts" />
///<reference path="../Infrastructure/ObjectModelValueProvider.ts" />
///<reference path="../Infrastructure/ValueTypeProvider.ts" />
///<reference path="../Infrastructure/Locals.ts" />
///<reference path="../exceptions.ts" />
///<reference path="./htmlRenderer.ts" />
var ForEachRenderer = (function (_super) {
    __extends(ForEachRenderer, _super);
    function ForEachRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "li";
        this.elements = [
            "foreach"
        ];
    }
    ForEachRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        //get the local model
        this.rendererProvider = rendererProvider;
        var localModel = this.getLocalModelValue(host, statement, model);
        if(Object.prototype.toString.call(model) === '[object Array]') {
            var result = "";
            for(var i in model) {
                var locals = new Locals(host);
                locals.push(this.iteratorItem(i, model));
                result += this._renderChildren(statement.children, host, model[i], this.defaultChildTag);
                locals.pop();
            }
            return result;
        }
        return "";
    };
    ForEachRenderer.prototype.iteratorItem = function (index, items) {
        return {
            _first: index == 0,
            _last: index == items.length - 1,
            _index: index,
            _even: index % 2 == 0,
            _odd: index % 2 == 1
        };
    };
    return ForEachRenderer;
})(HtmlRenderer);
//@ sourceMappingURL=foreachRenderer.js.map
