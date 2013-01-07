namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Renderers.Infrastructure;

    public class IfRenderer : HtmlRenderer
    {
        public IfRenderer(IHost host)
            : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "if"; }
        }

        public override void Render(Parrot.Infrastructure.IParrotWriter writer, IRendererFactory rendererFactory, Nodes.Statement statement, IDictionary<string, object> documentHost, object model)
        {
            //base.Render(writer, rendererFactory, statement, documentHost, model);
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = Host.ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            if (localModel != null && localModel is bool)
            {
                if ((bool) localModel)
                {
                    RenderChildren(writer, statement.Children, rendererFactory, documentHost, base.DefaultChildTag, model);
                }
            }
        }
    }
}
