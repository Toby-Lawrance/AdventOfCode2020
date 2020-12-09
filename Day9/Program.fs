// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
let permutations (indexed: (int*Int64) list) =
    query {
        for (ix,x) in indexed do
        for (iy,y) in indexed do
        where (ix <> iy)
        select [x;y]
    }
    |> Seq.toList

let isValidNumber (prev) number =
    let indexed = List.mapi (fun i x -> (i,x)) prev
    let pairs = permutations indexed
    let sums = List.map List.sum pairs
    List.exists (fun x -> x = number) sums
    

let separateList preambleLength list =
    let preamble = List.take preambleLength list
    let rest = List.skip preambleLength list
    (preamble,rest)
    
let rec FindInvalid preLength list =
    let (pre,post) = separateList preLength list
    match post with
    | [] -> None
    | x::_ when not (isValidNumber pre x) -> Some x
    | _ -> FindInvalid preLength (List.tail list)
    
let FindContiguousRangeThatSums (fullList: Int64 list) (number: Int64) =
    let rec checkWindows windowSize =
        let windowed = List.windowed windowSize fullList
        let sums = List.map (fun w -> (List.sum w,w)) windowed
        let sumFound = List.exists (fun (s,w) -> s = number) sums
        let allAbove = not (List.exists (fun (s,w) -> s < number) sums)
        if allAbove then
            None
        else
            match sumFound with
            | true -> Some (snd (List.find (fun (s,_) -> s = number) sums))
            | false -> checkWindows (windowSize + 1)
    checkWindows 2    

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day9")
                |> Array.map Int64.Parse
                |> Array.toList
    
    let preambleLength = 25
    
    let invalid = input
                  |> FindInvalid preambleLength
    
    if Option.isSome invalid then
        printfn "Found: %d" invalid.Value
    
    let range = match invalid with
                | Some x -> FindContiguousRangeThatSums input x
                | _ -> None
                
    if Option.isSome range then
        printfn "Range that sums: %A" range.Value
    
    let topTail = match range with
                  | Some l -> Some (List.min l, List.max l)
                  | _ -> None
    
    match topTail with
    | Some (x,y) -> printfn "Found them: (%d,%d) = %d" x y (x + y)
    | None -> printfn "Something went wrong"        
    0 // return an integer exit code