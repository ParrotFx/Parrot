///<reference path="rendererProvider.ts" />
var ParrotDocumentView = (function () {
    function ParrotDocumentView(parrotDocument) {
        this.parrotDocument = parrotDocument;
        this.rendererProvider = new RendererProvider();
    }
    ParrotDocumentView.prototype.render = function (host, model) {
        var result = "";
        for(var i in this.parrotDocument.children) {
            var child = this.parrotDocument.children[i];
            var renderer = this.rendererProvider.getRenderer(child.name);
            result += renderer.render(child, host, model, this.rendererProvider);
        }
        return result;
    };
    return ParrotDocumentView;
})();
//@ sourceMappingURL=parrotDocumentView.js.map
