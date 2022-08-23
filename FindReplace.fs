module FindReplace

open System.Text.RegularExpressions

/// Matches the pattern in content string, and performs runs a pair option
/// where the first element is the fist capture group match in the content
/// and the second element is the entire content with the captured text replaced
/// with replacement text.
let replaceText (pattern: string) (replacement: string) (content: string) : (string * string) option =
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
        let result = replaceText @"(tent)" "versation" "my content"
        Assert.True(result.IsSome)
        let (matchedString, replacement) = result.Value
        Assert.AreEqual("tent", matchedString)
        Assert.AreEqual("my conversation", replacement)


    [<Test>]
    let ``Returns None when no matches`` () =
        let result = replaceText @"(wontmatch)" "replacementText" "my content"
        Assert.True(result.IsNone)
