using System;
using SpanJson.Resolvers;
using SpanJson.Formatters;
using Microsoft.FSharp.Collections;

namespace SpanJson.FSharp.Formatter
{
    public sealed class FSharpListFormatter<T, TSymbol, TResolver> : BaseFormatter, IJsonFormatter<FSharpList<T>, TSymbol>
        where TResolver : IJsonFormatterResolver<TSymbol, TResolver>, new()  where TSymbol : struct
    {
        public static readonly FSharpListFormatter<T, TSymbol, TResolver> Default = new FSharpListFormatter<T, TSymbol, TResolver>();

        private static readonly IJsonFormatter<T[], TSymbol> innerFormatter =
            StandardResolvers.GetResolver<TSymbol, TResolver>().GetFormatter<T[]>();

        public FSharpList<T> Deserialize(ref JsonReader<TSymbol> reader)
        {
            return ListModule.OfArray(innerFormatter.Deserialize(ref reader));
        }

        public void Serialize(ref JsonWriter<TSymbol> writer, FSharpList<T> value, int nestingLimit)
        {
            innerFormatter.Serialize(ref writer, ListModule.ToArray(value), nestingLimit);
        }
    }

    public sealed class FSharpMapFormatter<T, TSymbol, TResolver> : BaseFormatter, IJsonFormatter<FSharpMap<string, T>, TSymbol>
        where TResolver : IJsonFormatterResolver<TSymbol, TResolver>, new()  where TSymbol : struct
    {
        public static readonly FSharpMapFormatter<T, TSymbol, TResolver> Default = new FSharpMapFormatter<T, TSymbol, TResolver>();

        private static readonly IJsonFormatter<Tuple<string, T>[], TSymbol> innerFormatter =
            StandardResolvers.GetResolver<TSymbol, TResolver>().GetFormatter<Tuple<string, T>[]>();

        public FSharpMap<string, T> Deserialize(ref JsonReader<TSymbol> reader)
        {
            return new FSharpMap<string, T>(innerFormatter.Deserialize(ref reader));
        }

        public void Serialize(ref JsonWriter<TSymbol> writer, FSharpMap<string, T> value, int nestingLimit)
        {
            innerFormatter.Serialize(ref writer, MapModule.ToArray(value), nestingLimit);
        }
    }
}