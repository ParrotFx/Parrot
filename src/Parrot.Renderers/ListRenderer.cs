// -----------------------------------------------------------------------
// <copyright file="ListRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ListRenderer : HtmlRenderer
    {
        public ListRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get
            {
                yield return "ul";
                yield return "ol";
            }
        }

        public override string DefaultChildTag
        {
            get { return "li"; }
        }

        private IList<object> ToList(IEnumerable loop)
        {
            var list = new List<object>();
            foreach (var item in loop)
            {
                list.Add(item);
            }

            return list;
        }

        public override void RenderChildren(IParrotWriter writer, Nodes.Statement statement, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, string defaultTag = null)
        {
            if (string.IsNullOrEmpty(defaultTag))
            {
                defaultTag = DefaultChildTag;
            }

            //get model from parameter
            if (statement.Parameters != null && statement.Parameters.Count == 1)
            {
                var localModel = GetLocalModel(documentHost, statement, model);

                if (localModel is IEnumerable)
                {
                    //create locals object to handle local values to the method
                    Locals locals = new Locals(documentHost);

                    IList<object> items = ToList(model as IEnumerable);
                    for (int i = 0; i < items.Count; i++)
                    {
                        var localItem = items[i];
                        locals.Push(IteratorItem(i, items));

                        base.RenderChildren(writer, statement.Children, rendererFactory, documentHost, defaultTag, localItem);

                        locals.Pop();
                    }
                }
            }
            else
            {
                base.RenderChildren(writer, statement.Children, rendererFactory, documentHost, defaultTag, model);
            }
        }

        private static object IteratorItem(int index, IList<object> items)
        {
            return new
            {
                _first = index == 0,
                _last = index == items.Count - 1,
                _index = index,
                _even = index % 2 == 0,
                _odd = index % 2 == 1
            };
        }

    }
}