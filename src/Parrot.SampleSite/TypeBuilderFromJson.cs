namespace Parrot.SampleSite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Newtonsoft.Json.Linq;

    public static class TypeBuilderFromJson
    {
        public static Type CreateType(JObject jObject)
        {
            var dict = new Dictionary<string, object>();
            foreach (var child in jObject)
            {
                dict.Add(child.Key, child.Value);
            }
            return CompileResultType(dict);
        }

        public static Type CompileResultType(IDictionary<string, object> properties)
        {
            string typeName = "t" + Guid.NewGuid().ToString("N");

            TypeBuilder tb = GetTypeBuilder(typeName);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (var field in properties.Keys)
            {
                if (properties[field] is IDictionary<string, object>)
                {
                    CreateProperty(tb, field, CompileResultType(properties[field] as IDictionary<string, object>));
                }
                else if (properties[field] is IEnumerable<JToken> && !(properties[field] is JValue))
                {
                    CreateProperty(tb, field, CompileResultType(properties[field] as IEnumerable<JToken>));
                }
                else
                {
                    CreateProperty(tb, field, typeof (object));
                }
            }

            Type objectType = tb.CreateType();
            return objectType;
        }

        public static Type CompileResultType(IEnumerable<object> properties)
        {
            string typeName = "t" + Guid.NewGuid().ToString("N");

            TypeBuilder tb = GetTypeBuilder(typeName);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (object field in properties)
            {
                if (field is IEnumerable && !(field is JProperty))
                {
                    //return typeof(ArrayList);
                    //we need to get all the possible properties in this array and create a type from it
                    var dict = new Dictionary<string, object>();
                    foreach (var p in properties)
                    {
                        foreach (var child in p as JObject)
                        {
                            if (!dict.ContainsKey(child.Key))
                            {
                                dict.Add(child.Key, child.Value);
                            }
                        }
                    }

                    Type t = typeof (IList<>);
                    return t.MakeGenericType(CompileResultType(dict));
                }

                CreateProperty(tb, (field as JProperty).Name, typeof (object));
            }

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder(string typeName)
        {
            var an = new AssemblyName(typeName);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicTypeModule");
            TypeBuilder tb = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                null
                );

            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes
                );

            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod(
                    "set_" + propertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null,
                    new[]
                        {
                            propertyType
                        }
                    );

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}