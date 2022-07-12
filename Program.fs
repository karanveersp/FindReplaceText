open Argu
open System
open System.IO
open Microsoft.Extensions.FileSystemGlobbing

open FindReplace

type CliArguments =
    | Directory_Path of path: string
    | File_Pattern of pattern: string
    | Text_Pattern of pattern: string
    | Replacement of string
    | Dryrun

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Directory_Path _ -> "specify directory containing files to match recursively"
            | File_Pattern _ -> "specify a glob pattern to match files"
            | Text_Pattern _ -> "specify a regex pattern to match text in matched files"
            | Replacement _ -> "replacement text"
            | Dryrun _ -> "perform a dry run (print updates without making them)"

let getCliArgs argv =
    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red
        )

    let parser =
        ArgumentParser.Create<CliArguments>(programName = "FindReplaceText", errorHandler = errorHandler)

    let results = parser.ParseCommandLine argv

    printfn "Got parse results %A"
    <| results.GetAllResults()

    results

[<EntryPoint>]
let main argv =
    let args = getCliArgs argv

    let dirPath = args.GetResult(Directory_Path)
    let filePattern = args.GetResult(File_Pattern)
    let textPattern = args.GetResult(Text_Pattern)
    let replacement = args.GetResult(Replacement)
    let dryrun = args.Contains Dryrun

    // Have all cli args to work with now.

    let fileMatcher = new Matcher()
    fileMatcher.AddInclude(filePattern) |> ignore

    let matchingFiles =
        fileMatcher.GetResultsInFullPath(dirPath)
        |> Seq.toList

    if matchingFiles.IsEmpty then
        printfn $"No files matching pattern '{filePattern}' under '{dirPath}'"
        0
    else
        printfn "Matching files: %s" (String.Join(", ", matchingFiles))

        matchingFiles
        |> Seq.map (fun fp -> (fp, File.ReadAllText(fp)))
        |> Seq.map (fun (fp, text) -> (fp, replace_text textPattern replacement text))
        |> Seq.filter (fun (_, updateOption) -> updateOption.IsSome)
        |> Seq.iter (fun (fp, updateOption: (string * string) option) ->
            let (matchedText, update) = updateOption.Value

            if dryrun then
                printfn $"{fp} - Would replace all occurances of '{matchedText}' with '{replacement}'"
            else
                File.WriteAllText(fp, update)
                printfn $"{fp} - Replaced all occurrances '{matchedText}' with '{replacement}'")

        0
