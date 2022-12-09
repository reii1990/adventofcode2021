using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        private const string Folder = "C:\\Users\\dan.torberg\\Downloads\\adventOfCode2021";

        static void Main(string[] args)
        {
            var result = GetResult(args[0]);

            Console.WriteLine($"Result: {result}");
        }

        private static double GetResult(string puzzle)
        {
            Console.WriteLine($"Running puzzle: {puzzle}");
            var input = new string[0];
            try
            {
                input = File.ReadAllLines($"{Folder}\\{puzzle}.txt");
            }
            catch { }

            switch (puzzle)
            {
                case "input1":
                    return One(input);
                case "input2":
                    return Two(input);
                case "input3":
                    return Three(input);
                case "input4":
                    return Four(input);
                case "input5":
                    return Five(input);
                case "input6":
                    return Six(input);
                case "input7":
                    return Seven(input);
                default:
                    return -1;
            }
        }

        private static double Seven(string[] lines)
        {
            var initial = lines.First().Split(",").Select(value => int.Parse(value)).GroupBy(value => value).ToDictionary(g => g.Key, g => g.Count());
            return -1;
        }

        private static double Six(string[] lines)
        {
            var initial = lines.First().Split(",").Select(value => int.Parse(value)).GroupBy(value => value).ToDictionary(g => g.Key, g => g.Count());
            var buckets = Enumerable.Range(0, 9).ToDictionary(b => b, b => initial.TryGetValue(b, out var value) ? value : 0d);

            for (int day = 0;day < 256; day++)
            {
                var spawns = buckets[0];
                for(var i = 0; i < 8; i++)
                {
                    buckets[i] = buckets[i + 1];
                }
                buckets[6] += spawns;
                buckets[8] = spawns;

            }
            return buckets.Sum(b => b.Value);
        }

        private static int Five(string[] lines)
        {
            var res = 0;
            var grid = new Dictionary<int, Dictionary<int, int>>();
            foreach (var line in lines)
            {
                var points = line.Split(" -> ");
                var point1 = points[0].Split(",");
                var point2 = points[1].Split(",");
                var x1 = int.Parse(point1[0]);
                var y1 = int.Parse(point1[1]);
                var x2 = int.Parse(point2[0]);
                var y2 = int.Parse(point2[1]);
             
                var x = x1;
                var y = y1;
                while ((x1 < x2 && x <= x2) || (x1 > x2 && x >= x2) || (y1 < y2 && y <= y2) || (y1 > y2 && y >= y2))
                {
                    if (!grid.TryGetValue(x, out var ys))
                    {
                        ys = new Dictionary<int, int>();
                        grid.Add(x, ys);
                    }
                    if (!ys.TryGetValue(y, out var count))
                    {
                        ys.Add(y, 0);
                    }
                    if (++ys[y] == 2)
                    {
                        res++;
                    }

                    if (x1 < x2)
                    {
                        x++;
                    }
                    if (x1 > x2)
                    {
                        x--;
                    }
                    if (y1 < y2)
                    {
                        y++;
                    }
                    if (y1 > y2)
                    {
                        y--;
                    }
                }
            }
            return res;
        }

        private static bool Evaluate(List<int> board, bool rows)
        {
            for (int i = 0; i < 5; i++)
            {
                var hits = 0;
                for (int ii = 0; ii < 5; ii++)
                {
                    if (rows)
                    {
                        if (board[(i * 5) + ii] != -1)
                        {
                            break;
                        }
                    } 
                    else
                    {
                        if (board[i + (ii * 5)] != -1)
                        {
                            break;
                        }
                    }
                    hits++;
                }
                if (hits == 5)
                { 
                    return true;
                }
            }
            return false;
        }

        private static int Four(string[] lines)
        {
            var calls = lines[0].Split(",").Select(v => int.Parse(v)).ToArray();

            var bestBoard = new List<int>();
            var bestBoardCalls = int.MaxValue;

            var worstBoard = new List<int>();
            var worstBoardCalls = int.MinValue;

            var currentBoardIndexes = new List<int>();
            var currentBoardValues = new Dictionary<int, int>();
            foreach (var line in lines.Skip(2))
            {
                if (line == string.Empty)
                {
                    for(var i = 0; i < calls.Length; i++)
                    {
                        if (currentBoardValues.TryGetValue(calls[i], out var index))
                        {
                            currentBoardIndexes[index] = -1;

                            if (Evaluate(currentBoardIndexes, true) || Evaluate(currentBoardIndexes, false))
                            {
                                if (bestBoardCalls > i)
                                {
                                    bestBoardCalls = i;
                                    bestBoard = currentBoardIndexes;
                                }
                                if (worstBoardCalls < i)
                                {
                                    worstBoardCalls = i;
                                    worstBoard = currentBoardIndexes;
                                }

                                break;
                            }
                        }
                    }

                    currentBoardIndexes = new List<int>();
                    currentBoardValues = new Dictionary<int, int>();
                    continue;
                }
                foreach (var val in line.Split(" "))
                {
                    if (val == string.Empty)
                    {
                        continue;
                    }
                    currentBoardIndexes.Add(int.Parse(val));
                    currentBoardValues.Add(int.Parse(val), currentBoardValues.Count);
                }
            }

            var part1result = bestBoard.Where(value => value != -1).Sum(value => value) * calls[bestBoardCalls];
            var part2result = worstBoard.Where(value => value != -1).Sum(value => value) * calls[worstBoardCalls];
            return part2result;
        }

        private static dynamic GetDistribution(List<string> lines, int pos)
        {
            var zeroLines = new List<string>();
            var oneLines = new List<string>();

            foreach (var line in lines)
            {
                if (line[pos] - '0' == 0)
                {
                    zeroLines.Add(line);
                }
                else
                {
                    oneLines.Add(line);
                }
            }
            return zeroLines.Count > oneLines.Count
                    ? new { least = oneLines, most = zeroLines }
                    : new { least = zeroLines, most = oneLines };
        }

        private static int Three(string[] lines)
        {
            var most = new List<string>(lines);
            var least = new List<string>(lines);
            
            for (int i = 0; i < 12; i++)
            {
                most = most.Count > 1 ? GetDistribution(most, i).most : most;
                least = least.Count > 1 ? GetDistribution(least, i).least : least;
            }

            var oxygenGen = Convert.ToInt32(most.First(), 2);
            var co2Scrubber = Convert.ToInt32(least.First(), 2);

            return oxygenGen * co2Scrubber;
        }

        private static int Two(string[] lines)
        {
            var depth = 0;
            var horizontal = 0;
            var aim = 0;
            foreach (var line in lines)
            {
                var parts = line.Split(" ");
                var command = parts[0];
                var param = int.Parse(parts[1]);

                switch(command)
                {
                    case "forward":
                        horizontal += param;
                        depth += aim * param;
                        break;
                    case "down":
                        aim += param;
                        break;
                    case "up":
                        aim -= param;
                        break;
                    default:
                        throw new InvalidDataException();

                }
            }
            return depth * horizontal;
        }

        private static int One(string[] lines)
        {
            int count = 0;
            for (int i = 3; i < lines.Length; i++)
            {
                var prev = int.Parse(lines[i - 3]) + int.Parse(lines[i - 2]) + int.Parse(lines[i - 1]);
                var current = int.Parse(lines[i - 2]) + int.Parse(lines[i - 1]) + int.Parse(lines[i]);
                if (current > prev)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
