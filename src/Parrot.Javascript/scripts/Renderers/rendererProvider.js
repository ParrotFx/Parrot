var RendererProvider = (function () {
    function RendererProvider() {
        this.renderers = [];
        this.renderers.push(new HtmlRenderer());
        this.renderers.push(new StringRenderer());
        this.renderers.push(new SelfClosingRenderer());
        this.renderers.push(new DocTypeRenderer());
    }
    RendererProvider.prototype.getRenderer = function (type) {
        for(var i in this.renderers) {
            for(var g in this.renderers[i].elements) {
                if(type == this.renderers[i].elements[g]) {
                    return this.renderers[i];
                }
            }
        }
        return this.getRenderer("*");
    };
    return RendererProvider;
})();
