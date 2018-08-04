[<AutoOpen>]
module SpanJson.FSharp.Formatter.Tests.Helper

open SpanJson
open SpanJson.Resolvers
open SpanJson.FSharp.Formatter

type FSharpResolver = FSharpResolver<byte, ExcludeNullsOriginalCaseResolver<byte>>

let convert<'T> (value: 'T) =
  let serialized = JsonSerializer.Generic.Utf8.Serialize<'T, FSharpResolver>(value)
  JsonSerializer.Generic.Utf8.Deserialize<'T, FSharpResolver>(serialized)
