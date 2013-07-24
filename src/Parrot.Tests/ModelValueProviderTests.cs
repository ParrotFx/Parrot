// -----------------------------------------------------------------------
// <copyright file="ModelValueProviderTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Parrot.Infrastructure;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class ModelValueProviderTests
    {
        //string literal
        //keyword
        //"this" (local)
        //property
        //  assert model is not null
        //  if it contains . then cycle through

        [Test]
        public void StringLiteral()
        {
            var valueType = ValueType.StringLiteral;
            object property = "this is a string literal";
            var documentHost = new Dictionary<string, object>();

            IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
            IModelValueProvider modelValueProvider = new ObjectModelValueProvider(valueTypeProvider);
            object result;
            modelValueProvider.GetValue(documentHost, null, valueType, property, out result);
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(property, result as string);
        }

        [Test]
        public void Local()
        {
            var valueType = ValueType.Local;
            object property = "this";
            object model = new { Name = "Ben" };
            var documentHost = new Dictionary<string, object>();

            IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
            IModelValueProvider modelValueProvider = new ObjectModelValueProvider(valueTypeProvider);
            object result;
            modelValueProvider.GetValue(documentHost, model, valueType, property, out result);

            Assert.AreEqual(model, result);
        }

        [Test]
        public void Keyword()
        {
            var valueType = ValueType.Keyword;
            object property = false;
            var documentHost = new Dictionary<string, object>();

            IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
            IModelValueProvider modelValueProvider = new ObjectModelValueProvider(valueTypeProvider);
            object result;
            modelValueProvider.GetValue(documentHost, null, valueType, property, out result);

            Assert.AreEqual(property, result);
        }

        [Test]
        public void Property()
        {
            var valueType = ValueType.Property;
            object property = "Name";
            object model = new { Name = "Ben" };
            var documentHost = new Dictionary<string, object>();

            IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
            IModelValueProvider modelValueProvider = new ObjectModelValueProvider(valueTypeProvider);
            object result;
            modelValueProvider.GetValue(documentHost, model, valueType, property, out result);

            Assert.AreEqual("Ben", result);
        }

        [Test]
        public void NestedProperty()
        {
            var valueType = ValueType.Property;
            object property = "Name.FirstName";
            object model = new { Name = new { FirstName = "Ben", LastName = "Dornis" } };
            var documentHost = new Dictionary<string, object>();

            IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
            IModelValueProvider modelValueProvider = new ObjectModelValueProvider(valueTypeProvider);
            object result;
            modelValueProvider.GetValue(documentHost, model, valueType, property, out result);

            Assert.AreEqual("Ben", result);
        }
    }
}