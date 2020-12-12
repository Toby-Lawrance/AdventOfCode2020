using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day12
{
    enum Instruction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        LEFT,
        RIGHT,
        FORWARD,
        CRASH
    }

    class Ship
    {
        /* y+ == NORTH, y- == SOUTH
         * x- == WEST, x+ == EAST
         * heading in RADIANS
         */
        public long x, y;
        //Relative location of waypoint to ship
        public long wx, wy;
        
        public Ship(long x, long y, long wx,long wy)
        {
            this.x = x;
            this.y = y;
            this.wx = wx;
            this.wy = wy;
        }

        public void ExecuteInstruction(Instruction i, long amount)
        {
            Console.WriteLine($"Executing: {i}: {amount}");
            switch (i)
            {
                case Instruction.NORTH: this.wy += amount;
                    return;
                
                case Instruction.SOUTH: this.wy -= amount;
                    return;
                
                case Instruction.EAST: this.wx += amount;
                    return;

                case Instruction.WEST: this.wx -= amount;
                    return;
                
                case Instruction.LEFT:
                {
                    if(amount != 90 && amount != 180 && amount != 270) {Console.WriteLine($"Amount: {amount}");}
                    var cx = this.wx;
                    var cy = this.wy;
                    switch (amount)
                    {
                        case 90:
                        {
                            this.wx = (-cy);
                            this.wy = (cx);
                            Console.WriteLine($"(wx,wy) = ({wx},{wy})");
                            return;
                        }
                        
                        case 180:
                        {
                            this.wx = (-cx);
                            this.wy = (-cy);
                            Console.WriteLine($"(wx,wy) = ({wx},{wy})");
                            return;
                        }
                        
                        case 270:
                        {
                            this.wx = (cy);
                            this.wy = (-cx);
                            Console.WriteLine($"(wx,wy) = ({wx},{wy})");
                            return;
                        }
                        
                        default:
                            Console.WriteLine("Trying the hard way");
                            break;
                    }
                    
                    
                    /*var eDistance = Math.Sqrt((wx*wx)+(wy*wy));
                    var theta = Math.Atan((double)wy / (double)wx);
                    var turn = (amount / 180.0) * Math.PI;
                    var newPolar = (eDistance, theta - turn);
                    var nx = Math.Round(eDistance * Math.Cos(newPolar.Item2));
                    var ny = Math.Round(eDistance * Math.Sin(newPolar.Item2));
                    Console.WriteLine($"Old for Left: ({wx},{wy})");
                    Console.WriteLine($"Rotated by: {amount}");
                    Console.WriteLine($"New for Left: ({nx},{ny})");
                    this.wx = (long)nx;
                    this.wy = (long)ny;*/
                    
                    return;
                }

                case Instruction.RIGHT:
                {
                    if(amount != 90 && amount != 180 && amount != 270) {Console.WriteLine($"Amount: {amount}");}
                    var cx = this.wx;
                    var cy = this.wy;
                    switch (amount)
                    {
                        case 90:
                        {
                            this.wx = (cy);
                            this.wy = (-cx);
                            Console.WriteLine($"(wx,wy) = ({wx},{wy})");
                            return;
                        }
                        
                        case 180:
                        {
                            this.wx = (-cx);
                            this.wy = (-cy);
                            Console.WriteLine($"(wx,wy) = ({wx},{wy})");
                            return;
                        }
                        
                        case 270:
                        {
                            this.wx = (-cy);
                            this.wy = (cx);
                            Console.WriteLine($"(wx,wy) = ({wx},{wy})");
                            return;
                        }
                        
                        default:
                            Console.WriteLine("Trying the hard way");
                            break;
                    }
                    
                    
                    /*var eDistance = Math.Sqrt((wx*wx)+(wy*wy));
                    var theta = Math.Atan((double)wy / (double)wx);
                    var turn = (amount / 180.0) * Math.PI;
                    var newPolar = (eDistance, theta + turn);
                    var nx = Math.Round(eDistance * Math.Cos(newPolar.Item2));
                    var ny = Math.Round(eDistance * Math.Sin(newPolar.Item2));
                    Console.WriteLine($"Old for Right: ({wx},{wy})");
                    Console.WriteLine($"Rotated by: {amount}");
                    Console.WriteLine($"New for Right: ({nx},{ny})");
                    this.wx = (long)nx;
                    this.wy = (long)ny;*/
                    return;
                }

                case Instruction.FORWARD:
                {
                    this.x += wx*amount;
                    this.y += wy*amount;
                    Console.WriteLine($"(x,y) = ({x},{y})");
                    return;
                }
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }

        public long ManhattanDistance(long ox,long oy)
        {
            Console.WriteLine($"Currently at: ({x},{y})");
            return Math.Abs(ox - x) + Math.Abs(oy - y);
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("Z:/aoc/day12");
            var rgx = @"(?<instruction>[NSEWLRF])(?<amount>\d+)";
            var instructions = input.Select(s => Regex.Match(s, rgx)).Select(m =>
            {
                var iss = m.Groups["instruction"].Value;
                Instruction i = Instruction.CRASH;
                switch (iss)
                {
                    case "N": i = Instruction.NORTH;
                        break;
                    
                    case "S": i = Instruction.SOUTH;
                        break;
                    
                    case "E": i = Instruction.EAST;
                        break;
                    
                    case "W": i = Instruction.WEST;
                        break;
                    
                    case "L": i = Instruction.LEFT;
                        break;
                    
                    case "R": i = Instruction.RIGHT;
                        break;
                    
                    case "F": i = Instruction.FORWARD;
                        break;
                }
                var num = Int64.Parse(m.Groups["amount"].Value);
                return (i, num);
            }).ToArray();
            
            var myShip = new Ship(0,0,10,1);
            
            foreach (var instruction in instructions)
            {
                myShip.ExecuteInstruction(instruction.i,instruction.num);
            }

            var d = myShip.ManhattanDistance(0, 0);
            Console.WriteLine($"Manhattan distance from origin: {d}");
        }
    }
}