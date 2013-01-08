///<reference path="rendererProvider.ts" />
class ParrotDocumentView {
    parrotDocument: ParrotDocument;
    rendererProvider: RendererProvider;
    
    constructor(parrotDocument: ParrotDocument) {
        this.parrotDocument = parrotDocument;
        this.rendererProvider = new RendererProvider();
    }

    render(host: any[], model: any): string {
        var result: string = "";
        for (var i in this.parrotDocument.children) {
            var child = this.parrotDocument.children[i];

            var renderer = this.rendererProvider.getRenderer(child.name);

            result += renderer.render(child, host, model, this.rendererProvider);
        }

        return result;
    }
}