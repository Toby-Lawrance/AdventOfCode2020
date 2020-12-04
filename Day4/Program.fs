// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open System.Text.RegularExpressions
open ParallelisationLibrary

let validByr v =
    if String.length v <> 4 then
        false
    else
        match Int32.TryParse v with
        | true, num -> num >= 1920 && num <= 2002
        | false, _ -> false
        
let validIyr v =
    if String.length v <> 4 then
        false
    else
        match Int32.TryParse v with
        | true, num -> num >= 2010 && num <= 2020
        | false, _ -> false
        
let validEyr v =
    if String.length v <> 4 then
        false
    else
        match Int32.TryParse v with
        | true, num -> num >= 2020 && num <= 2030
        | false, _ -> false

let validHgt v =
    let rgx = "(\d+)(cm|in)"
    let m = Regex.Match(v,rgx)
    if not (m.Success) then
        false
    else
        let tup = ((m.Groups.Item 1).Value |> int),(m.Groups.Item 2).Value
        match tup with
        | num,"cm" -> num >= 150 && num <= 193
        | num,"in" -> num >= 59 && num <= 76
        | _,_ -> false

let validHcl v =
    let rgx = "#[\da-f]{6}"
    Regex.Match(v,rgx).Success

let validEcl v =
    let valid = ["amb";"blu";"brn";"gry";"grn";"hzl";"oth"]
    List.contains v valid

let validPid v =
    if String.length v <> 9 then
        false
    else
        match Int32.TryParse v with
        | true,num -> true
        | false,_ -> false

let KeyToVerifier k v =
    let keys = [("byr",validByr);("iyr",validIyr);("eyr",validEyr);("hgt",validHgt);("hcl",validHcl);("ecl",validEcl);("pid",validPid);]
    let kvp = List.find (fun (ke,v) -> k = ke) keys
    (snd kvp) v

let validPassport (pass: string) =
    let keyVal = "(.+):(.+)"
    let kvp = pass.Split(' ') |> Array.pMap (fun s -> Regex.Match(s.Trim(),keyVal))
    let neededFields = List.sort ["byr";"iyr";"eyr";"hgt";"hcl";"ecl";"pid"]
    printfn "%s" pass
    let keys = kvp
                |> Array.map (fun (m:Match) -> ((m.Groups.Item 1).Value),((m.Groups.Item 2).Value))
                |> Array.filter (fun (s,v) -> s <> "cid")
                |> Array.filter (fun (s,v) -> s <> "")
                |> Array.filter (fun (s,v) -> KeyToVerifier s v)
                |> Array.sort
                |> Array.toList
                |> List.map (fun (s,v) -> (printfn "%s:%s" s v)
                                          s)
    keys = neededFields

[<EntryPoint>]
let main argv =
    let blocks = (File.ReadAllText("Z:/aoc/day4")).Split([|"\n\n"|],StringSplitOptions.None)
    
    blocks
    |> Array.pMap (fun b -> Regex.Replace(b,"(\n)+"," "))
    |> Array.pMap (fun s -> Regex.Replace(s,"(\ )+"," "))
    |> Array.filter validPassport
    |> Array.length
    |> printfn "%d valid passports"
    0 // return an integer exit code