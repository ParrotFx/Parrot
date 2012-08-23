// -----------------------------------------------------------------------
// <copyright file="ModelValueProviderTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using NUnit.Framework;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Renderers.Infrastructure;
    using ValueType = Infrastructure.ValueType;

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
            object model = null;

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(property, result as string);
        }

        [Test]
        public void Local()
        {
            var valueType = ValueType.Local;
            object property = "this";
            object model = new { Name = "Ben" };

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(model, result);
        }

        [Test]
        public void Keyword()
        {
            var valueType = ValueType.Keyword;
            object property = false;
            object model = null;

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(property, result);
        }

        [Test]
        public void Property()
        {
            var valueType = ValueType.Property;
            object property = "Name";
            object model = new { Name = "Ben" };

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual("Ben", result);
        }

        [Test]
        public void NestedProperty()
        {
            var valueType = ValueType.Property;
            object property = "Name.FirstName";
            object model = new { Name = new { FirstName = "Ben", LastName = "Dornis" } };

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual("Ben", result);
        }
    }

    [TestFixture]
    public class ExpandoObjectModelValueProviderTests
    {

        [Test]
        public void ExpandoObjectStringLiteral()
        {
            var valueType = ValueType.StringLiteral;
            dynamic property = "this is a string literal";
            object model = null;

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(property, result as string);
        }

        [Test]
        public void Local()
        {
            var valueType = ValueType.Local;
            object property = "this";
            object model = new { Name = "Ben" };

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(model, result);
        }

        [Test]
        public void Keyword()
        {
            var valueType = ValueType.Keyword;
            object property = false;
            object model = null;

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(property, result);
        }

        [Test]
        public void Property()
        {
            var valueType = ValueType.Property;
            object property = "Name";
            dynamic model = new ExpandoObject();
            model.Name = new ExpandoObject();
            model.Name = "Ben";

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual("Ben", result);
        }

        [Test]
        public void NestedProperty()
        {
            var valueType = ValueType.Property;
            object property = "Name.FirstName";
            dynamic model = new ExpandoObject();
            model.Name = new ExpandoObject();
            model.Name.FirstName = "Ben";
            model.Name.LastName = "Dornis";

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual("Ben", result);
        }

    }

}
