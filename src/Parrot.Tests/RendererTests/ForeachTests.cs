using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parrot.Tests.RendererTests
{
    using NUnit.Framework;

    public class ForeachTests
    {
        public class First : TestRenderingBase
        {
            [Test]
            public void TrueTest()
            {
                var model = new [] {"item1", "item2", "item3"};
                var text = @"foreach {
                                if(_first) > ""First""
                                li > @_index
                                if(_last) > ""Last""
                             }";

                var result = Render(text, model);
                Assert.AreEqual("First<li>0</li><li>1</li><li>2</li>Last", result);
            }

            [Test]
            public void UL()
            {
                var model = new[] {"item1", "item2", "item3"};
                var text = @"ul { li > @this }";

                var result = Render(text, model);
                Assert.AreEqual("<ul><li>item1</li><li>item2</li><li>item3</li></ul>", result);
            }
        }
    }
}
