// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open ParallelisationLibrary

let union (seqList: seq<'a list>) =
    let twoUnion lx ly =
        match ly with
        | [] -> lx
        | _ -> (List.filter (fun x -> List.contains x lx) ly) @ (List.filter (fun x -> List.contains x ly) lx)
               |> List.distinct
    Seq.reduce twoUnion seqList

let personQuestions (l:string) =
    l.Trim() |> Seq.toList |> List.distinct

let groupQuestions people =
    Array.pMap personQuestions people
    |> Array.toList
    |> union
    |> List.length

[<EntryPoint>]
let main argv =
    let groups = (File.ReadAllText("Z:/aoc/day6")).Split([|"\n\n"|],StringSplitOptions.RemoveEmptyEntries)
    
    groups
    |> Array.pMap (fun (s:string) -> s.Split('\n',StringSplitOptions.RemoveEmptyEntries))
    |> Array.toList
    |> List.pMap groupQuestions
    |> List.reduce (+)
    |> printfn "Sum total: %d"
    0 // return an integer exit code