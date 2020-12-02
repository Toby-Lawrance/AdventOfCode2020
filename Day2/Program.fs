// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open System.Text.RegularExpressions
open ParallelisationLibrary

let validPassword (min,max,letter,pass) =
    let instances = String.length (String.filter (fun c -> c = letter) pass)
    min <= instances && instances <= max
    
let part2ValidPassword (first,second,letter,pass:string) =
    let isFirst = ((String.length pass) >= first) && ((pass.Chars (first-1)) = letter)
    let isSecond = ((String.length pass) >= second) && ((pass.Chars (second-1)) = letter)
    isFirst <> isSecond

let extractFromMatch (mat: Match) =
    let min = (mat.Groups.Item "min").Value
    let max = (mat.Groups.Item "max").Value
    let letter = (mat.Groups.Item "letter").Value.Chars 0
    let password = (mat.Groups.Item "password").Value
    (Int32.Parse min),(Int32.Parse max),(letter),(password)

[<EntryPoint>]
let main argv =
    let regexString = "(?<min>\d+)\-(?<max>\d+)\s(?<letter>[a-z]+)\:\s(?<password>[a-z]+)"
    let matcher l = Regex.Match(l,regexString)
    let input = File.ReadAllLines("Z:/aoc/day2a")
    
    input
    |> Array.pMap matcher
    |> Array.pMap extractFromMatch
    |> Array.filter part2ValidPassword
    |> Array.length
    |> printfn "Number of valid passwords: %d"
    
    0 // return an integer exit code