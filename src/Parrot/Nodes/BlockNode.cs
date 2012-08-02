using Newtonsoft.Json.Linq;

namespace Parrot.Nodes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Parrot;
    using Parrot.Nodes;

    public class BlockNode : AbstractNode
    {
        public ParameterNodeList Parameters { get; private set; }
        public string BlockName { get; private set; }
        public AttributeNodeList Attributes { get; private set; }
        public List<BlockNode> Children { get; private set; }

        protected BlockNode(string blockName)
        {
            Attributes = new AttributeNodeList();
            Children = new List<BlockNode>();
            Parameters = new ParameterNodeList();

            //required bullshit
            if (blockName.Contains("."))
            {
                var values = blockName.Split(".".ToCharArray());
                BlockName = values[0];
                foreach (var value in values.Skip(1))
                {
                    Attributes.Add(new AttributeNode("class", value));
                }
            }
            else
            {
                BlockName = blockName;
            }
        
        }

        private void AddAttribute(AttributeNode node)
        {
            if (node.Key == "id")
            {
                if (node.Value != null && node.Value.Contains("."))
                {
                    var values = node.Value.Split(".".ToCharArray());
                    foreach (var value in values.Skip(1))
                    {
                        Attributes.Add(new AttributeNode("class", value));
                    }

                    Attributes.Add(new AttributeNode(node.Key, values[0]));
                    return;
                }
            }

            Attributes.Add(node);
        }

        public BlockNode(string blockName, AttributeNodeList attributes, ParameterNodeList parameterNodes) : this(blockName)
        {
            if (attributes != null)
            {
                foreach (var attributeNode in attributes)
                {
                    AddAttribute(attributeNode);
                }
            }

            if (parameterNodes != null)
            {
                Parameters = parameterNodes;
            }
        }

        public BlockNode(string blockName, AttributeNodeList attributes, ParameterNodeList parameterNodes, BlockNodeList children)
            : this(blockName, attributes, parameterNodes)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    Children.Add(child);
                }
            }
        }

        public BlockNode(string blockName, AttributeNodeList attributes, ParameterNodeList parameterNodes, BlockNode childNode)
            : this(blockName, attributes, parameterNodes)
        {
            Children.Add(childNode);
        }

        public BlockNode(string blockName, AttributeNodeList attributes, ParameterNodeList parameterNodes, IEnumerable<BlockNode> children)
            : this(blockName, attributes, parameterNodes)
        {

            if (children != null)
            {
                foreach (var child in children)
                {
                    Children.Add(child);
                }
            }
        }

        public override bool IsTerminal
        {
            get { return false; }
        }

        //public override string ToString()
        //{
        //    //get the parameter name
        //    object localModel = Model;

        //    if (Parameters != null && Parameters.Any())
        //    {
        //        Parameters.First().SetModel(localModel);

        //        localModel = (Parameters.First() as ParameterNode).GetValue();
        //    }

        //    TagBuilder builder = new TagBuilder(BlockName);
        //    foreach (var attribute in Attributes.Cast<AttributeNode>())
        //    {
        //        attribute.SetModel(localModel);

        //        if (attribute.Key == "class")
        //        {
        //            builder.AddCssClass(attribute.GetValue());
        //        }
        //        else
        //        {
        //            builder.MergeAttribute(attribute.Key, attribute.GetValue(), true);
        //        }
        //    }

        //    if (Children.Any())
        //    {
        //        if (localModel is IEnumerable)
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            foreach (object item in localModel as IEnumerable)
        //            {
        //                var localItem = item;
        //                sb.Append(string.Join("\r\n", Children.Select(c => c.SetModel(localItem))));
        //            }
        //            builder.InnerHtml += sb.ToString();
        //        }
        //        else
        //        {
        //            builder.InnerHtml = string.Join("\r\n", Children.Select(c => c.SetModel(localModel)));
        //        }
        //    }
        //    else
        //    {
        //        if (Parameters != null && Parameters.Any())
        //        {
        //            builder.InnerHtml = localModel != null ? localModel.ToString() : "";
        //        }
        //    }

        //    //check for self closing
        //    if (selfClosing.Contains(builder.TagName.ToLower()))
        //    {
        //        return builder.ToString(TagRenderMode.SelfClosing);
        //    }

        //    return builder.ToString();
        //}
    }
}