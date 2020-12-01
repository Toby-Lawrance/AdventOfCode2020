// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO

let generateTriples (l:'a list) =
    query {
        for x in l do
            join y in l on (true = true)
            join z in l on (true = true)
            select (x,y,z)
    }
    |> Seq.toList
    
let generatePairs (l:'a list) =
    query {
        for x in l do
            join y in l on (true = true)
            select (x,y)
    }
    |> Seq.toList

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day1a")
                |> Array.map Int32.Parse
                |> Array.toList
    
    input
    |> generatePairs
    |> List.filter (fun (x,y) -> x+y = 2020)
    |> List.map (fun (x,y) -> printfn "x,y = (%d,%d) = %d" x y  (x*y))
    |> ignore            
                
    input
    |> generateTriples
    |> List.filter (fun (x,y,z) -> x+y+z = 2020)
    |> List.map (fun (x,y,z) -> printfn "x,y,z = (%d,%d,%d) = %d" x y z (x*y*z))
    |> ignore
    0 // return an integer exit code