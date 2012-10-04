using System;
using System.Collections.Generic;
using System.Linq;
using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    public class ConditionalRenderer : HtmlRenderer
    {
        private readonly Lazy<Dictionary<string, IRenderer>> _stringRenderers;

        public ConditionalRenderer(IHost host) : base(host)
        {
             _stringRenderers = new Lazy<Dictionary<string, IRenderer>>(LoadRenderers);
        }

        private Dictionary<string, IRenderer> LoadRenderers()
        {
            if (Host != null)
            {
                var factory = Host.DependencyResolver.Resolve<IRendererFactory>();
                return new Dictionary<string, IRenderer>
                {
                    {"string", factory.GetRenderer("string")}, //StringLiteralRenderer
                    {"output", factory.GetRenderer("output")}, //OutputRenderer
                    {"rawoutput", factory.GetRenderer("rawoutput")}, //RawRenderer
                };
            }

            return null;
        }

        public override string Render(AbstractNode node, object documentHost)
        {
            //throw new NotImplementedException();

            //check the parameters
            Statement statement = node as Statement;
            string statementToOutput = "default";
            if (statement != null)
            {
                //model value provider

                if (statement.Parameters.Count == 1)
                {
                    //value provider type
                    IModelValueProviderFactory factory = new ModelValueProviderFactory();
                    if (documentHost != null)
                    {
                        var provider = factory.Get(documentHost.GetType());
                        var value = provider.GetValue(documentHost, statement.Parameters[0].ValueType, statement.Parameters[0].Value);
                        statementToOutput = value.ToString();
                    }
                    else
                    {
                        statementToOutput = statement.Parameters[0].Value;
                    }
                    
                }

                //default child

                //switch(statement.Type)
                foreach (var child in statement.Children)
                {
                    string value = null;
                    IRenderer renderer;

                    if (child is StringLiteral)
                    {
                        //get string value
                        value = _stringRenderers.Value["string"].Render(child);
                    }
                    else if (child is RawOutput)
                    {
                        //get string value
                        value = _stringRenderers.Value["rawoutput"].Render(child);
                    }
                    else if (child is EncodedOutput)
                    {
                        //get string value
                        value = _stringRenderers.Value["output"].Render(child);
                    } 
                    else
                    {
                        value = child.Name;
                    }

                    if (value.Equals(statementToOutput, StringComparison.OrdinalIgnoreCase))
                    {
                        var rendererFactory = Host.DependencyResolver.Resolve<IRendererFactory>();
                        //render only the child
                        return RenderChildren(child, documentHost);
                    }
                }

                var childToRender = statement.Children.SingleOrDefault(s => s.Name.Equals("default", StringComparison.OrdinalIgnoreCase));

                if (childToRender != null)
                {
                    var rendererFactory = Host.DependencyResolver.Resolve<IRendererFactory>();
                    //render only the child
                    return RenderChildren(childToRender, documentHost);
                }
            }

            return null;
        }
    }
}