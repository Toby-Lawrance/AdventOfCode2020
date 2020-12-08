using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("Z:/aoc/day8");
            string rgx = @"(?<instruction>.{3}) ((?<pn>\+|\-)(?<amount>\d+))";
            var instructions = input.Select(s => Regex.Match(s, rgx)).Select(m =>
            {
                var i = m.Groups["instruction"].Value;
                bool isNeg = m.Groups["pn"].Value == "-";
                int num = Int32.Parse(m.Groups["amount"].Value);
                return (i, isNeg, num);
            }).ToArray();

            var allProgramPermutations = permuteProgram(instructions);
            var working = allProgramPermutations.Select(executeProgram).Where(result => result.Item2).ToArray();
            Console.WriteLine($"Working programs: {working.Length}");
            Console.WriteLine($"Accumulator result after running: {working.First().Item1}");
        }

        static (string, bool, int)[][] permuteProgram((string, bool, int)[] instructions)
        {
            var flippableInstrucitonIndices = instructions.Select(((instruction, i) =>
            {
                if (instruction.Item1 == "jmp" || instruction.Item1 == "nop")
                {
                    return i;
                }

                return -1;
            })).Where(i => i != -1);

            return instructions.Select((instruction, i) =>
            {
                if (flippableInstrucitonIndices.Contains(i))
                {
                    if (instruction.Item1 == "jmp")
                    {
                        instruction.Item1 = "nop";
                    }
                    else
                    {
                        instruction.Item1 = "jmp";
                    }

                    var instructionsCopy = ((string,bool,int)[])instructions.Clone();
                    instructionsCopy[i] = instruction;
                    return instructionsCopy;
                }

                return null;
            }).Where(ins => ins is not null).ToArray();
        }

        static (long,bool) executeProgram((string, bool, int)[] instructions)
        {
            HashSet<int> visitedLines = new HashSet<int>();
            
            int  programCounter = 0;
            long accumulator = 0;

            while (programCounter < instructions.Length)
            {
                if (visitedLines.Contains(programCounter))
                {
                    Console.WriteLine($"Infinite loop starts at: {programCounter}");
                    return (accumulator,false);
                }
                var instruction = instructions[programCounter];
                visitedLines.Add(programCounter);

                if (instruction.Item1 == "acc")
                {
                    accumulator += (instruction.Item2 ? instruction.Item3 * -1 : instruction.Item3);
                } else if (instruction.Item1 == "jmp")
                {
                    programCounter += (instruction.Item2 ? instruction.Item3 * -1 : instruction.Item3);
                    continue;
                } else if (instruction.Item1 == "nop")
                {
                    
                }
                else
                {
                    throw new ArgumentException($"Unrecognised instruction: {instruction.Item1}");
                }
                
                programCounter++;
            }
            Console.WriteLine("Program terminated successfully");
            return (accumulator,true);
        }
    }
}