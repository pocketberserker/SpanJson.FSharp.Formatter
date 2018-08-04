module SpanJson.FSharp.Formatter.Tests.OptionTest

open Xunit

module Option =

  [<Fact>]
  let some () =

    let input = Some 1
    let actual = convert input
    Assert.Equal(input, actual)

  [<Fact>]
  let none () =

    let input: int option = None
    let actual = convert input
    Assert.Equal(input, actual)

module ValueOption =

  [<Fact>]
  let some () =

    let input = ValueSome 1
    let actual = convert input
    Assert.Equal(input, actual)

  [<Fact>]
  let none () =

    let input: int voption = ValueNone
    let actual = convert input
    Assert.Equal(input, actual)
