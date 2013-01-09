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
var ListRenderer = (function (_super) {
    __extends(ListRenderer, _super);
    function ListRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "li";
        this.elements = [
            "ul", 
            "ol"
        ];
    }
    ListRenderer.prototype.renderChildren = function (statement, host, model, defaultTag) {
        if(defaultTag == null || defaultTag.length == 0) {
            defaultTag = this.defaultChildTag;
        }
        if(Object.prototype.toString.call(model) === '[object Array]') {
            var result = "";
            for(var i in model) {
                var locals = new Locals(host);
                locals.push(this.iteratorItem(i, model));
                result += this._renderChildren(statement.children, host, model[i], defaultTag);
                locals.pop();
            }
            return result;
        }
        return this._renderChildren(statement.children, host, model, defaultTag);
    };
    ListRenderer.prototype.iteratorItem = function (index, items) {
        return {
            _first: index == 0,
            _last: index == items.length - 1,
            _index: index,
            _even: index % 2 == 0,
            _odd: index % 2 == 1
        };
    };
    return ListRenderer;
})(HtmlRenderer);
//@ sourceMappingURL=listRenderer.js.map
