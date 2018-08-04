using SpanJson.Resolvers;
using SpanJson.Formatters;
using Microsoft.FSharp.Core;

namespace SpanJson.FSharp.Formatter
{
    public sealed class FSharpOptionFormatter<T, TSymbol, TResolver> : BaseFormatter, IJsonFormatter<FSharpOption<T>, TSymbol>
        where TResolver : IJsonFormatterResolver<TSymbol, TResolver>, new()  where TSymbol : struct
    {
        public static readonly FSharpOptionFormatter<T, TSymbol, TResolver> Default = new FSharpOptionFormatter<T, TSymbol, TResolver>();

        private static readonly IJsonFormatter<T, TSymbol> elementFormatter =
            StandardResolvers.GetResolver<TSymbol, TResolver>().GetFormatter<T>();

        public FSharpOption<T> Deserialize(ref JsonReader<TSymbol> reader)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            return FSharpOption<T>.Some(elementFormatter.Deserialize(ref reader));
        }

        public void Serialize(ref JsonWriter<TSymbol> writer, FSharpOption<T> value, int nestingLimit)
        {
            if (FSharpOption<T>.get_IsNone(value))
            {
                writer.WriteNull();
                return;
            }

            elementFormatter.Serialize(ref writer, value.Value, nestingLimit);
        }
    }

    public sealed class FSharpValueOptionFormatter<T, TSymbol, TResolver> : BaseFormatter, IJsonFormatter<FSharpValueOption<T>, TSymbol>
        where TResolver : IJsonFormatterResolver<TSymbol, TResolver>, new() where TSymbol : struct
    {
        public static readonly FSharpValueOptionFormatter<T, TSymbol, TResolver> Default = new FSharpValueOptionFormatter<T, TSymbol, TResolver>();

        private static readonly IJsonFormatter<T, TSymbol> valueFormatter =
            StandardResolvers.GetResolver<TSymbol, TResolver>().GetFormatter<T>();

        public FSharpValueOption<T> Deserialize(ref JsonReader<TSymbol> reader)
        {
            if (reader.ReadIsNull())
            {
                return FSharpValueOption<T>.ValueNone;
            }

            return FSharpValueOption<T>.NewValueSome(valueFormatter.Deserialize(ref reader));
        }

        public void Serialize(ref JsonWriter<TSymbol> writer, FSharpValueOption<T> value, int nestingLimit)
        {
            if (value.IsValueNone)
            {
                writer.WriteNull();
                return;
            }

            valueFormatter.Serialize(ref writer, value.Value, nestingLimit);
        }
    }
}