// -----------------------------------------------------------------------
// <copyright file="ForeachRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ForeachRenderer : HtmlRenderer
    {
        public ForeachRenderer(IHost host)
            : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "foreach"; }
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var localModel = GetLocalModel(documentHost, statement, model);

            //Assert that we're looping over something
            IEnumerable loop = localModel as IEnumerable;
            if (loop == null)
            {
                throw new InvalidCastException("model is not IEnumerable");
            }

            //create a local copy
            IList<object> items = ToList(loop);
            
            //create locals object to handle local values to the method
            Locals locals = new Locals(documentHost);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                foreach (var child in statement.Children)
                {
                    var renderer = rendererFactory.GetRenderer(child.Name);
                    locals.Push(IteratorItem(i, items));

                    renderer.Render(writer, rendererFactory, child, documentHost, item);

                    locals.Pop();
                }
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

        private IList<object> ToList(IEnumerable loop)
        {
            var list = new List<object>();
            foreach (var item in loop)
            {
                list.Add(item);
            }

            return list;
        }
    }
}