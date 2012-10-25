// -----------------------------------------------------------------------
// <copyright file="PrettyRenderingTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using NUnit.Framework;
using Parrot.Infrastructure;

namespace Parrot.Tests.RendererTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PrettyRenderingTests : TestRenderingBase
    {

        private Host GetHost()
        {
            var host = new MemoryHost();
            host.DependencyResolver.Register(typeof(IParrotWriter), () => new PrettyStringWriter());

            return host;
        }

        [Test]
        public void SingleIndentation()
        {
            Assert.AreEqual("<div>\r\n\tthis is a test\r\n</div>\r\n", Render("div > :\"this is a test\"", GetHost()));
            Assert.AreEqual("<html>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n</html>\r\n", Render("html > div > :\"1\"", GetHost()));
            Assert.AreEqual("<html>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n</html>\r\n", Render("html { div { :\"1\" } div { :\"1\" } }", GetHost()));
        }

    }

    public class PrettyStringWriter : StandardWriter
    {
        private int _indentation;
        private WriteType _lastWritten = WriteType.None;

        public PrettyStringWriter() : base()
        {
            _indentation = 0;
        }

        private WriteType GetType(string value)
        {
            if (value.EndsWith("/>"))
            {
                //closing
                return WriteType.SelfClosingElement;
            }
            
            if (value.StartsWith("</") && value.EndsWith(">"))
            {
                return WriteType.ClosingElement;
            }
            
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                return WriteType.OpeningElement;
            }

            return WriteType.Literal;
        }

        private enum WriteType
        {
            None,
            SelfClosingElement,
            ClosingElement,
            OpeningElement,
            Literal
        }


        public override void Write(string value)
        {
            var elementType = 0;
            var incrementIndentation = 0;
            var addNewLineAfterWrite = false;

            //check type
            var type = GetType(value);
            switch (type)
            {
                case WriteType.SelfClosingElement:
                    incrementIndentation = 0;
                    addNewLineAfterWrite = true;
                    break;
                case WriteType.ClosingElement:
                    if (_lastWritten == WriteType.Literal)
                    {
                        base.Write("\r\n");
                    }
                    _indentation -=1;
                    addNewLineAfterWrite = true;
                    break;
                case WriteType.OpeningElement:
                    incrementIndentation = 1;
                    addNewLineAfterWrite = true;
                    break;
                case WriteType.Literal:
                    if (_lastWritten == WriteType.OpeningElement)
                    {
                        //incrementIndentation = 1;
                    }
                    break;
            }

            if (_indentation > 0 && (_lastWritten != WriteType.Literal || (_lastWritten == WriteType.Literal && type == WriteType.ClosingElement)))
            {
                base.Write(new string('\t', _indentation));
            }
            _indentation += incrementIndentation;

            base.Write(value);
            _lastWritten = type;

            if (addNewLineAfterWrite)
            {
                base.Write("\r\n");
            }


        }

    }

}
