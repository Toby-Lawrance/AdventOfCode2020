// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open System.Text.RegularExpressions
open ParallelisationLibrary

let RuleHolds x (h,r) =
    match r with
    | None -> false
    | Some rule -> List.exists (fun (i,c) -> c = x) rule
    
let extractHeader (h,r) =
    h
        
let bagContains x rules =
    let relevantRule = List.find (fun r -> (extractHeader r) = x) rules
    match relevantRule with
    | (_,r) -> r
        
let expandBagContents numBags =
    match numBags with
    | None -> []
    | Some bagList -> List.map (fun (i,c) -> List.replicate i c) bagList |> List.concat
    
let GetAllBagContents x rules =
    let rec GetAllBagContents' y =
        let contents = expandBagContents (bagContains y rules)
        match contents with
        | [] -> []
        | l -> List.map (fun b -> b::(GetAllBagContents' b) ) l
               |> List.concat
    GetAllBagContents' x
    
        
let EventuallyContain x allRules =
    let starts = List.filter (RuleHolds x) allRules
    let setToList (s) = Set.foldBack (fun se sacc -> se::sacc) s []
    let rec setUnfold (s) =
        let newSet = setToList s
                     |> List.map (fun h -> List.filter (RuleHolds h) allRules)
                     |> List.concat
                     |> List.map extractHeader
                     |> Set.ofList
                     |> Set.union s
        if newSet = s then
            newSet
        else
            setUnfold newSet
    
    starts
    |> List.map extractHeader
    |> Set.ofList
    |> setUnfold

let parseRuleHeader (s:string) =
    s.Replace("bags"," ").Trim()
    
let parseBags s =
    let parseBag (m:Match) =
        let isNone = (m.Groups.Item "None").Success
        if isNone then
            (0,"")
        else
            let num = (m.Groups.Item "num").Value |> int
            let bag = (m.Groups.Item "bag").Value
            (num,bag)
    let rgx = "(?<num>\d+ )?((((?<None>no other)|(?<bag>[a-z]+ [a-z]+)) bag(s)?))"
    let matches = Regex.Matches(s,rgx) |> Seq.toList
    let bags = List.map parseBag matches
    if (List.exists (fun (n,c) -> n = 0) bags) then
        None
    else
        Some bags
        

[<EntryPoint>]
let main argv =
    let input = File.ReadAllLines("Z:/aoc/day7") |> Array.toList
    
    let rules = input
                |> List.map (fun (s:string) -> s.Split([|"contain"|],StringSplitOptions.RemoveEmptyEntries) |> Array.toList)
                |> List.map (fun ss -> List.map (fun (s:string) -> s.Trim()) ss)
                |> List.map (fun l -> (parseRuleHeader (List.head l),parseBags (List.head (List.tail l))))
                //|> List.mapi (fun i a -> printfn "Rule %d: %A" i a
                //                         a)
    let allEventuallyContainGold = EventuallyContain "shiny gold" rules
    
    allEventuallyContainGold
    |> Set.count
    |> printfn "%d rules which can eventually hold a shiny gold bag"
    
    let startColour = "shiny gold"
    let contents = GetAllBagContents startColour rules
    
    contents
    |> printfn "%s contains %A other bags" startColour
    
    contents
    |> List.length
    |> printfn "%s contains %d other bags" startColour
    
    0 // return an integer exit code