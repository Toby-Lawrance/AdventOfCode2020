// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open ParallelisationLibrary

let findContestWin timeTable =
    let isWinning t =
        let rec checkSubsequent l t2 =
            if l = [] then
                true
            else
                match List.head l with
                | None -> checkSubsequent (List.tail l) (t2+1L)
                | Some x when (t2 % x) = 0L -> checkSubsequent (List.tail l) (t2+1L)
                | _ -> false
        match List.head timeTable with
        | None -> checkSubsequent timeTable t
        | Some x when (t % x) = 0L -> checkSubsequent timeTable t
        | _ -> false
    let rec advanceTime adv t =
        let winningTime = isWinning t
        if winningTime then
            t
        else
            advanceTime adv (t+adv)
    let firstOne = List.head timeTable
    match firstOne with
    | Some t -> advanceTime t (t)
    | None -> advanceTime 1L 1L
    
let findContestNonBrute timeTable =
    let lcm x y = x * y
    let rec advanceTime t adv l =
        if l = [] then
            t - adv
        else
            let (index,n) = List.head l
            if (t + index) % n = 0L then
                let newAdv = lcm adv n
                advanceTime (t+newAdv) newAdv (List.tail l)
            else
                advanceTime (t+adv) adv l
    advanceTime 1L 1L timeTable

let findFirstBus startTime timeTable =
    let rec advanceTime t =
        let nowBus = List.filter (fun bt -> (t % bt) = 0) timeTable
        match nowBus with
        | [] -> advanceTime (t+1)
        | x::[] -> (t,x)
        | _ -> failwith "Multiple buses? Maybe problem"
    advanceTime startTime

let processTimeTable2 l =
    List.indexed l
    |> List.filter (fun (i,x) -> x <> "x")
    |> List.map (fun (i,x ) -> (int64 i,int64 x))

let processTimeTable1 l =
    List.filter (fun s -> s <> "x") l
    |> List.map int

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day13")
    let firstNum = (Array.head input) |> int
    let secondList = input.[1].Split(',') |> Array.toList
    
    printfn "Num: %i" firstNum
    printfn "Timetable: %A" secondList
    
    let numericalTimetable = secondList
                             |> processTimeTable1
                             
    let bus = findFirstBus firstNum numericalTimetable
    
    bus
    |> printfn "First bus %A"
    
    printfn "Solution part 1: %i" (((fst bus) - firstNum) * (snd bus))
    
    let winTime = findContestNonBrute (processTimeTable2 secondList)
    
    printfn "Solution part2: %i" winTime
    
    0 // return an integer exit code