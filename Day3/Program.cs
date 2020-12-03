using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("Z:/aoc/day3a");
            var mapped = input.Select(s=> s.Select(c => c == '#').ToArray()).ToArray();
            var slopes = new []{(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)};
            var result = slopes.Select((s) =>
            {
                int x = 0, y = 0, bottom = mapped.Length,trees = 0; //Top left
                int width = mapped[0].Length;
                while (y < bottom)
                {
                    var tree = mapped[y][x];
                    if (tree)
                    {
                        trees++;
                    }

                    y += s.Item2;
                    x = (x + s.Item1) % width;
                }
                Console.WriteLine($"Hit {trees} trees");
                return trees;
            }).Select(x => (long)x)
                .Aggregate(1l,(x,y) => x*y);
            Console.WriteLine($"Result: {result}");
        }
    }
}