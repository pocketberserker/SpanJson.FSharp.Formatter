using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Dynamic;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Reflection;
using SpanJson.Resolvers;
using SpanJson.Formatters;

namespace SpanJson.FSharp.Formatter
{
    public class FSharpResolver<TSymbol, TResolver> : IJsonFormatterResolver<TSymbol, FSharpResolver<TSymbol, TResolver>> where TResolver : IJsonFormatterResolver<TSymbol, TResolver>, new() where TSymbol : struct
    {
        public readonly TResolver Default;

        public FSharpResolver()
        {
            Default = new TResolver();
        }

        public IJsonFormatter GetFormatter(Type t)
        {
            var ti = t.GetTypeInfo();
            var resolverType = typeof(FSharpResolver<TSymbol, TResolver>);

            if (FSharpType.IsRecord(t, null))
            {
                Type formatterType;
                if (ti.IsValueType)
                {
                    formatterType = typeof(ComplexStructFormatter<,,>);
                }
                else
                {
                    formatterType = typeof(ComplexClassFormatter<,,>);
                }
                return CreateInstance(formatterType, new Type[] { t, typeof(TSymbol), resolverType });
            }
            else if (ti.IsFSharpMap())
            {
                return CreateInstance(typeof(FSharpMapFormatter<,,>), new [] { ti.GenericTypeArguments[1], typeof(TSymbol), resolverType });
            }
            else if (ti.IsFSharpList())
            {
                return CreateInstance(typeof(FSharpListFormatter<,,>), new Type[] { ti.GenericTypeArguments[0], typeof(TSymbol), resolverType });
            }
            else if (ti.IsFSharpOption())
            {
                return CreateInstance(typeof(FSharpOptionFormatter<,,>), new[] { ti.GenericTypeArguments[0], typeof(TSymbol), resolverType });
            }
            else if (ti.IsFSharpValueOption())
            {
                return CreateInstance(typeof(FSharpValueOptionFormatter<,,>), new[] { ti.GenericTypeArguments[0], typeof(TSymbol), resolverType });
            }

            return Default.GetFormatter(t);
        }

        private IJsonFormatter CreateInstance(Type genericType, Type[] genericTypeArguments, params object[] arguments)
        {
            return (IJsonFormatter)Activator.CreateInstance(genericType.MakeGenericType(genericTypeArguments), arguments);
        }

        public IJsonFormatter<T, TSymbol> GetFormatter<T>()
        {
            return (IJsonFormatter<T, TSymbol>)GetFormatter(typeof(T));
        }

        public IJsonFormatter GetFormatter(JsonMemberInfo info, Type overrideMemberType = null)
        {
            return GetFormatter(overrideMemberType ?? info.MemberType);
        }

        public JsonObjectDescription GetObjectDescription<T>()
        {
            var description = Default.GetObjectDescription<T>();

            var t = typeof(T);
            if (FSharpType.IsRecord(t, null))
            {
                var ctor = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderByDescending(a => a.GetParameters().Length)
                    .FirstOrDefault();
                var fields = FSharpType.GetRecordFields(t, null);
                var members = description.Members.Where(m => fields.Any(f => f.Name == m.MemberName)).ToArray();
                description = new JsonObjectDescription(ctor, null, members);
            }

            return description;
        }

        public JsonObjectDescription GetDynamicObjectDescription(IDynamicMetaObjectProvider provider)
        {
            return Default.GetDynamicObjectDescription(provider);
        }

        public Func<T> GetCreateFunctor<T>()
        {
            var t = typeof(T);
            var info = t.GetTypeInfo();
            if(info.IsFSharpOption())
            {
                return Expression.Lambda<Func<T>>(Expression.Constant(null)).Compile();
            }
            else if(info.IsFSharpValueOption())
            {
                return Expression.Lambda<Func<T>>(Expression.Property(null, info.GetProperty("ValueNone"))).Compile();
            }
            else if (info.IsFSharpList())
            {
                var method = Assembly.GetAssembly(typeof(FSharpList<>))
                    .GetType("Microsoft.FSharp.Collections.ListModule")
                    .GetMethod("Empty", BindingFlags.Static | BindingFlags.Public)
                    .MakeGenericMethod(info.GenericTypeArguments);
                return Expression.Lambda<Func<T>>(Expression.Call(null, method)).Compile();
            }
            else if (info.IsFSharpMap())
            {
                var method = Assembly.GetAssembly(typeof(FSharpMap<,>))
                    .GetType("Microsoft.FSharp.Collections.MapModule")
                    .GetMethod("Empty", BindingFlags.Static | BindingFlags.Public)
                    .MakeGenericMethod(info.GenericTypeArguments);
                return Expression.Lambda<Func<T>>(Expression.Call(null, method)).Compile();
            }

            return Default.GetCreateFunctor<T>();
        }

        public Func<T, TConverted> GetEnumerableConvertFunctor<T, TConverted>()
        {
            return Default.GetEnumerableConvertFunctor<T, TConverted>();
        }
    }

    public class FSharpResolver<TSymbol> : FSharpResolver<TSymbol, ExcludeNullsOriginalCaseResolver<TSymbol>> where TSymbol : struct
    { }

    internal static class ReflectionExtensions
    {
        public static bool IsFSharpOption(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FSharpOption<>);
        }

        public static bool IsFSharpValueOption(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FSharpValueOption<>);
        }

        public static bool IsFSharpList(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FSharpList<>);
        }

        public static bool IsFSharpMap(this TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FSharpMap<,>);
        }
    }
}
