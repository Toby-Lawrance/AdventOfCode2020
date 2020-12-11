// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open System.Reflection.Metadata

type floorSpace =
    | Unoccupied
    | Occupied
    | Floor


let listOfListsToArray ll =
    List.map List.toArray ll
    |> List.toArray
  

let radicalCountOccupied (allInput : floorSpace [][]) (x,y) =
    let (upX,upY) = (Array.length allInput,Array.length allInput.[0])
    let allAround = [
                     (-1,-1);(0,-1);(+1,-1)
                     (-1, 0);        (+1,0)
                     (-1,+1);(0,+1);(+1,+1)
                    ]
    let rec checkDirection (dx,dy) (px,py) =
        let (nx,ny) = (px+dx,py+dy)
        if (nx < 0) || (nx >= upX) || (ny < 0) || (ny >= upY) then
            false
        else
            match allInput.[nx].[ny] with
            | Unoccupied -> false
            | Occupied -> true
            | Floor -> checkDirection (dx,dy) (nx,ny)
    List.filter (fun d -> checkDirection d (x,y)) allAround
    |> List.length

let countOccupied (allInput : floorSpace [] []) (x,y) =
    let allAround = [
                     (x-1,y-1);(x,y-1);(x+1,y-1)
                     (x-1,  y);        (x+1,  y)
                     (x-1,y+1);(x,y+1);(x+1,y+1)
                    ]
    List.filter (fun (cx,cy) -> let square = allInput.[cx].[cy]
                                square = Occupied) allAround
    |> List.length
    
let applyRules (state: floorSpace [] []) space (x,y) =
    let currently = space
    let around = countOccupied state (x,y)
    match currently with
    | Unoccupied -> if around = 0 then Occupied else Unoccupied
    | Occupied -> if around >= 4 then Unoccupied else Occupied
    | x -> x
    
let applyRadicalRules (state: floorSpace [] []) space (x,y) =
    let currently = space
    let around = radicalCountOccupied state (x,y)
    match currently with
    | Unoccupied -> if around = 0 then Occupied else Unoccupied
    | Occupied -> if around >= 5 then Unoccupied else Occupied
    | x -> x    
 
let rec runSim ruleApplier (initial : floorSpace [][]) =
    let newState = Array.mapi (fun x row -> Array.mapi (fun y space -> 
                                                                       if space <> Floor then
                                                                            ruleApplier initial space (x,y)
                                                                       else
                                                                            space) row ) initial
    if newState = initial then
        newState
    else
        runSim ruleApplier newState
      
let mapper c =
    match c with
    | 'L' -> Unoccupied
    | '#' -> Occupied
    | '.' -> Floor

let inputToLists input =
    Array.map (fun s -> (Seq.toList s) |> List.map mapper) input
    |> Array.toList
    
let padInput input =
    let firstRowReference = List.head input
    let headerRow = List.replicate (List.length firstRowReference + 2) Floor
    let widened = List.map (fun row -> List.append (Floor::row) ([Floor])) input
    List.append (headerRow::widened) ([headerRow])

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day11")
    
    let floored = input
                  |> inputToLists
                  //|> padInput

    
    
    printfn "Floored:\n %A" floored
    
    let stable = floored
                 |> listOfListsToArray
                 |> runSim applyRadicalRules
    
    stable
    |> Array.map (fun row -> Array.filter (fun space -> space = Occupied) row)
    |> Array.concat
    |> Array.length
    |> printfn "%d occupied seats"
    
    //printfn "Stable:\n %A" stable
    
    0 // return an integer exit code