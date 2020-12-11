// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open System.Reflection.Metadata

let fac x =
    let rec facR i acc =
        match i with
        | 0L | 1L -> acc
        | _ -> facR (i - 1L) (acc * i)
    facR x 1L

let passThroughPrint msg value =
    printfn "%s %A" msg value
    value
    
let partA adapters =
    let withExtra = List.append [0L;(List.max adapters) + 3L] adapters
    let jolts = List.sort withExtra
                |> List.windowed 2
                |> List.map (fun (l:int64 list) -> (List.last l) - (List.head l))
                |> List.groupBy id
                |> List.map (fun (i,il) -> (i,List.length il))
    
    printfn "Jolt differences:  %A" jolts
    
    if List.exists (fun (i:int64,_) -> i > 3L) jolts then
        failwith "problemo"
    
    (snd (List.find (fun (i:int64,_) -> i = 1L) jolts)) * (snd (List.find (fun (i:int64,_) -> i = 3L) jolts))

let groupsPassingPredicate pred l =
    let rec grouping (acc : int list list) = function
        | [] -> acc
        | x -> let firstGroup = List.takeWhile pred x
               let firstSkip = if List.length firstGroup > 0 then
                                    List.skip (List.length firstGroup) x
                               else
                                   List.skipWhile (not << pred) x
               grouping (List.append acc [firstGroup]) firstSkip
    grouping [] l
    |> List.filter (fun l -> l <> [])
    

let partB adapters =
    let withExtra = List.append [0L;(List.max adapters) + 3L] adapters
    let sequences = List.sort withExtra
                    |> List.pairwise
                    |> List.map (fun (x,y) -> y - x)
                    |> List.map int
                    
    printfn "Sequences: %A" sequences                
                    
    let permutes n = match n with
                     | 2 -> 2L
                     | 3 -> 4L
                     | 4 -> 7L
                     | _ -> 1L
                     
    let rec groupedSequences = groupsPassingPredicate (fun i -> i = 1) sequences
    
    printfn "Grouped sequences: %A" groupedSequences
        
    groupedSequences
    |> List.map List.length
    |> List.map permutes
    |> List.reduce (*)
    

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day10")
                |> Array.map Int64.Parse
                |> Array.toList
              
    input
    |> partA
    |> printfn "Solution part A: %d"
    
    input
    |> partB
    |> printfn "Solution part B: %d"
                
    0 // return an integer exit code