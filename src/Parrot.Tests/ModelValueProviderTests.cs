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
            var valueType = Infrastructure.ValueType.StringLiteral;
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
            var valueType = Infrastructure.ValueType.Local;
            object property = "this";
            object model = new { Name = "Ben" };

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(model, result);
        }

        [Test]
        public void Keyword()
        {
            var valueType = Infrastructure.ValueType.Keyword;
            object property = false;
            object model = null;

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(property, result);
        }

        [Test]
        public void Property()
        {
            var valueType = Infrastructure.ValueType.Property;
            object property = "Name";
            object model = new { Name = "Ben" };

            IModelValueProvider modelValueProvider = new ObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual("Ben", result);
        }

        [Test]
        public void NestedProperty()
        {
            var valueType = Infrastructure.ValueType.Property;
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
            var valueType = Infrastructure.ValueType.StringLiteral;
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
            var valueType = Infrastructure.ValueType.Local;
            object property = "this";
            object model = new { Name = "Ben" };

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(model, result);
        }

        [Test]
        public void Keyword()
        {
            var valueType = Infrastructure.ValueType.Keyword;
            object property = false;
            object model = null;

            IModelValueProvider modelValueProvider = new ExpandoObjectModelValueProvider();
            object result = modelValueProvider.GetValue(model, valueType, property);

            Assert.AreEqual(property, result);
        }

        [Test]
        public void Property()
        {
            var valueType = Infrastructure.ValueType.Property;
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
            var valueType = Infrastructure.ValueType.Property;
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

    public class ExpandoObjectModelValueProvider: IModelValueProvider
    {
        public object GetValue(object model, ValueType valueType, object property)
        {
            switch (valueType)
            {
                case Infrastructure.ValueType.StringLiteral:
                case Infrastructure.ValueType.Keyword:
                    return property;
                case Infrastructure.ValueType.Local:
                    return model;
                case Infrastructure.ValueType.Property:
                    if (model == null)
                    {
                        throw new NullReferenceException("model");
                    }

                    var propertyValues = (IDictionary<string, object>) model;
                    var result = model;

                    var properties = property.ToString().Split(".".ToCharArray());
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var name = properties[i];
                        result = propertyValues[name];
                        propertyValues = result as IDictionary<string, object>;
                    }
                    
                    return result;
            }

            throw new InvalidOperationException("ValueType");
        }

    }

    public class ObjectModelValueProvider : IModelValueProvider
    {
        public object GetValue(object model, Infrastructure.ValueType valueType, object property)
        {
            switch (valueType)
            {
                case Infrastructure.ValueType.StringLiteral:
                case Infrastructure.ValueType.Keyword:
                    return property;
                case Infrastructure.ValueType.Local:
                    return model;
                case Infrastructure.ValueType.Property:
                    if (model == null)
                    {
                        throw new NullReferenceException("model");
                    }

                    return GetModelProperty(model, property);
            }

            throw new InvalidOperationException("ValueType");
        }

        private object GetModelProperty(object model, object property)
        {
            if (property == null)
            {
                throw new NullReferenceException("property");
            }

            var stringProperty = property.ToString();
            string[] parameters = stringProperty.Split(".".ToCharArray());

            if (model == null && parameters.Length != 1)
            {
                throw new NullReferenceException(parameters[0]);
            }

            var pi = model.GetType().GetProperty(parameters[0]);
            if (pi != null)
            {
                var tempObject = pi.GetValue(model, null);
                if (parameters.Length == 1)
                {
                    return tempObject;
                }

                return GetModelProperty(tempObject, string.Join(".", parameters.Skip(1)));
            }

            return null;
        }
    }

    public interface IModelValueProvider
    {
        object GetValue(object model, Infrastructure.ValueType valueType, object property);
    }

    public interface IModelValueProviderFactory {
        void Register(Type type, Func<IModelValueProvider> provider);
        IModelValueProvider Get(Type type);
    }

    public class ModelValueProviderFactory : IModelValueProviderFactory
    {
        readonly Dictionary<Type, Func<IModelValueProvider>> _providers;

        public ModelValueProviderFactory()
        {
            _providers = new Dictionary<Type, Func<IModelValueProvider>>();
        }

        public void Register(Type type, Func<IModelValueProvider> provider)
        {
            if (_providers.ContainsKey(type))
            {
                _providers.Remove(type);
            }

            _providers.Add(type, provider);
        }

        public IModelValueProvider Get(Type type)
        {
            if (_providers.ContainsKey(type))
            {
                return _providers[type]();
            }

            return _providers[typeof (object)]();
        }
    }

}
