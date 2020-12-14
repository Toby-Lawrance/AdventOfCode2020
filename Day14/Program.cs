using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day14
{
    class Program
    {
        static Dictionary<long, long> memSpace;

        static bool processBlock(string mask, IEnumerable<(int, long)> memAllocs)
        {
            Console.WriteLine($"mask = {mask}");
            var converted = memAllocs.Select(tuple =>
            {
                Console.WriteLine($"Value: {tuple.Item2}");
                var binarised = Convert.ToString(tuple.Item2, 2).PadLeft(64, '0');
                Console.WriteLine($"Binary: {binarised}");
                var masked = binarised.Select((c, i) =>
                {
                    var m = mask[i];
                    switch (m)
                    {
                        case 'X': return c;
                        case '1': return m;
                        case '0': return m;
                        default: throw new Exception("Oh no");
                    }
                }).ToArray();
                var newString = String.Concat(masked);
                Console.WriteLine($"Masked: {newString}");
                var newLong = Convert.ToInt64(newString, 2);
                memSpace[tuple.Item1] = newLong;
                return (tuple.Item1, newString);
            }).ToArray();
            return true;
        }

        static List<string> createAddresses(string maskedAddress)
        {
            var allAddress = new List<string>();
            var i = maskedAddress.IndexOf('X');
            if (i == -1)
            {
                allAddress.Add(maskedAddress);
                return allAddress;
            }

            var prior = maskedAddress.Substring(0, i);
            var remaining = maskedAddress.Substring(i + 1);

            var result = createAddresses(remaining);
            foreach (var permutation in result)
            {
                allAddress.Add($"{prior}0{permutation}");
                allAddress.Add($"{prior}1{permutation}");
            }

            return allAddress;
        }

        static bool processBlock2(string mask, IEnumerable<(long, long)> memAllocs)
        {
            Console.WriteLine($"mask = {mask}");
            foreach (var addValTup in memAllocs)
            {
                var address = addValTup.Item1;
                var val = addValTup.Item2;
                var binarisedAddress = Convert.ToString(address, 2).PadLeft(36, '0');
                var masked = binarisedAddress.Select((c, i) =>
                {
                    var m = mask[i];
                    return m switch
                    {
                        'X' => m,
                        '1' => m,
                        '0' => c,
                        _ => throw new Exception("Oh no")
                    };
                }).ToArray();
                if (!masked.Contains('X'))
                {
                    var edgeAddress = String.Concat(masked);
                    memSpace[Convert.ToInt64(edgeAddress, 2)] = val;
                }

                var stringed = String.Concat(masked);
                var allAddress = createAddresses(stringed);
                foreach (var add in allAddress.Select(s => Convert.ToInt64(s, 2)))
                {
                    memSpace[add] = val;
                }
            }

            return true;
        }

        static void Main(string[] args)
        {
            var blockRgx = @"(?<block>(?<mask>mask = (?<bitMask>[10X]+)\s)(?<memAllocs>((mem\[\d+\] = (\d+))(\s)?)+))";
            var memRgx = @"(?<memAlloc>mem\[(?<address>\d+)\] = (?<value>\d+))";
            var input = File.ReadAllText("Z:/aoc/day14");
            memSpace = new Dictionary<long, long>();
            var blocks = Regex.Matches(input, blockRgx).Select(match =>
            {
                var mask = match.Groups["bitMask"].Value.PadLeft(36, '0');
                var memAllocs = match.Groups["memAllocs"].Value.Split('\n');
                var memMatches = memAllocs.Select(memAlloc => Regex.Match(memAlloc, memRgx))
                    .Where(match => match.Success).Select(memMatch =>
                    {
                        var address = Int64.Parse(memMatch.Groups["address"].Value);
                        var value = Int64.Parse(memMatch.Groups["value"].Value);
                        return (address, value);
                    }).ToArray();
                return (mask, memMatches);
            }).ToArray();

            var vals = blocks.Select(block => processBlock2(block.mask, block.memMatches)).ToArray();

            long total = 0;
            foreach (var kvp in memSpace)
            {
                total += kvp.Value;
            }

            Console.WriteLine($"Sum total: {total}");
        }
    }
}