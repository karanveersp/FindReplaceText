module FindReplace

open System.Text.RegularExpressions

let replace_text (pattern: string) (replacement: string) (content: string) : (string * string) option =
    let rx = new Regex(pattern)

    let rxMatch = rx.Match(content)

    if rxMatch.Success then
        let matchedText = rxMatch.Groups[1].Value
        Some(matchedText, content.Replace(matchedText, replacement))
    else
        None


module Tests =
    open NUnit.Framework

    [<Test>]
    let ``Replaces content with conversation`` () =
        let result = replace_text @"(tent)" "versation" "my content"
        Assert.True(result.IsSome)
        let (matchedString, replacement) = result.Value
        Assert.AreEqual("tent", matchedString)
        Assert.AreEqual("my conversation", replacement)


    [<Test>]
    let ``Returns None when no matches`` () =
        let result = replace_text @"(wontmatch)" "replacementText" "my content"
        Assert.True(result.IsNone)
