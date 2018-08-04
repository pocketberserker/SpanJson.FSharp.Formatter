# SpanJson.FSharp.Formatter

SpanJson.FSharp.Formatter is a [SpanJson](https://github.com/Tornhoof/SpanJson) extension library for F#.

## Usage

```fsharp
open SpanJson
open SpanJson.Resolvers
open SpanJson.FSharp.Formatter

type Person = {
  Age: int
  FirstName: string
  LastName: string
  MiddleName: string option
}

let p = {
  Age = 99
  FirstName = "foo"
  LastName = "buz"
  MiddleName = Some "bar"
}

let result = JsonSerializer.Generic.Utf8.Serialize<'T, FSharpResolver<byte, ExcludeNullsOriginalCaseResolver<byte>>>(p)
let p2 = JsonSerializer.Generic.Utf8.Deserialize<'T, FSharpResolver<byte, ExcludeNullsOriginalCaseResolver<byte>>>(result)
```
