// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open ParallelisationLibrary

type Partition =
    | Low
    | High

let convertRow r =
    match r with
    | 'F' -> Low
    | 'B' -> High
    | _ -> failwith "Unexpected"

let convertCol c =
    match c with
    | 'L' -> Low
    | 'R' -> High
    | _ -> failwith "Unexpected"


let homeInOn high low sequence =
    let partition top bot hiLo =
        let mid = (top + bot) / 2
        match hiLo with
        | High -> (top,mid+1)
        | Low -> (mid-1,bot)
    
    let rec homeInOn' (hi,lo) (x::xs) =
        if xs = [] then
            match x with
            | High -> hi
            | Low -> lo
        else
            homeInOn' (partition hi lo x) xs 
    homeInOn' (high,low) sequence

let findSeatPosition (rowList,colList) =
    (homeInOn 127 0 rowList, homeInOn 7 0 colList)

let parseInputLine s =
    let tupled = (List.take 7 s,List.skip 7 s)
    (List.map convertRow (fst tupled),List.map convertCol (snd tupled))

let findMySeat seatIds =
    let first = (List.head seatIds,false)
    List.fold (fun (acc,found) x -> if found then
                                           (x,found)
                                       else
                                           if x <> (acc + 1) then
                                                (x,true)
                                           else
                                                (x,false)) (first) seatIds

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day5") |> Array.pMap (fun (s:string) -> s.Trim()) |> Array.toList
    let seatIDs = List.pMap (findSeatPosition << parseInputLine << Seq.toList) input
                    |> List.map (fun (r,c) -> printfn "Row: %d Column: %d SeatID: %d" r c ((r * 8) + c)
                                              (r * 8) + c)
    let uniqueSeatIDs = seatIDs
                        |> List.distinct
    
    printfn "Unique SeatIDs: %b" (uniqueSeatIDs = seatIDs)
    
    uniqueSeatIDs
    |> List.sort
    |> findMySeat
    |> printfn "My seatID: %A"
    0 // return an integer exit code