var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DocTypeRenderer = (function (_super) {
    __extends(DocTypeRenderer, _super);
    function DocTypeRenderer() {
        _super.apply(this, arguments);

        this.defaultChildTag = "div";
        this.elements = [
            "doctype"
        ];
    }
    DocTypeRenderer.prototype.render = function (statement, host, model, rendererProvider) {
        var value = "html";
        if(statement.parameters.length > 0) {
            var localModel = this.getLocalModelValue(host, statement, model);
            value = localModel;
        }
        console.log(value);
        return "<!DOCTYPE " + value + ">";
    };
    return DocTypeRenderer;
})(BaseRenderer);
