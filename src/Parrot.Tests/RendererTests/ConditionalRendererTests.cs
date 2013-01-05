// -----------------------------------------------------------------------
// <copyright file="ConditionalRendererTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests.RendererTests
{
    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ConditionalRendererTests
    {
        //true/false
        public class TrueFalseTests : TestRenderingBase
        {
            [Test]
            public void TrueTest()
            {
                var text = @"conditional(true) {
                                true > 'true-part'
                                false > 'false-part'
                            }";

                var result = Render(text);
                Assert.AreEqual("true-part", result);
            }

            [Test]
            public void FalseTest()
            {
                var text = @"conditional(false) {
                                true > 'true-part'
                                false > 'false-part'
                            }";

                var result = Render(text);
                Assert.AreEqual("false-part", result);
            }

            [Test]
            public void StringTypeTest()
            {
                var text = @"conditional(Name) { 
                                'ben' > 'your name is ben' 
                                'not-ben' > 'your name is not ben'
                                default > 'this is the default'
                            }";

                var result = Render(text, new {Name = "ben"});
                Assert.AreEqual("your name is ben", result);
            }

            [Test]
            public void StringTypeTest2()
            {
                var text = @"conditional(Name) { 
                                'ben' > 'your name is ben' 
                                'not-ben' > 'your name is not ben'
                                default > 'this is the default'
                            }";

                var result = Render(text, new {Name = "not-ben"});
                Assert.AreEqual("your name is not ben", result);
            }

            [Test]
            public void StringTypeTestWithDefault()
            {
                var text = @"conditional(Name) { 
                                'ben' > 'your name is ben' 
                                'not-ben' > 'your name is not ben'
                                default > 'this is the default'
                            }";

                var result = Render(text, new {Name = "should render default as it exists"});
                Assert.AreEqual("this is the default", result);
            }
        }
    }
}