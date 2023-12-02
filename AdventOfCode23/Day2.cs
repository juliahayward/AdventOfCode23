using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day2Puzzle : PuzzleBase
    {
        internal static void DoPartOne(bool example)
        {
            var lines = ReadLines(2, example);
            var total = 0;
            var maximums = new Dictionary<string, int> { { "red", 12 }, { "green", 13 }, { "blue", 14} };
            // Each line represents a game...
            foreach (var line in lines)
            {
                bool gameIsPossible = true;
                var parts = line.Split(new [] {' ', ',', ';', ':'}, StringSplitOptions.RemoveEmptyEntries);
                // parts[0] will be "Game"
                var gameId = int.Parse(parts[1]);
                // Now iterate through the rest in pairs
                for (int i = 2; i < parts.Length; i += 2)
                {
                    var amount = int.Parse(parts[i]);
                    var colour = parts[i + 1];
                    if (amount > maximums[colour])
                    {
                        gameIsPossible = false;
                        break;
                    }
                }

                if (gameIsPossible) total += gameId;
            }
            Console.WriteLine(total);
        }

        internal static void DoPartTwo(bool example)
        {
            var lines = ReadLines(2, example);
            var total = 0;
            // Each line represents a game...
            foreach (var line in lines)
            {
                var maximums = new Dictionary<string, int> { { "red", 0 }, { "green", 0 }, { "blue", 0 } };

                var parts = line.Split(new[] { ' ', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
                // parts[0] will be "Game"
                var gameId = int.Parse(parts[1]);
                // Now iterate through the rest in pairs
                for (int i = 2; i < parts.Length; i += 2)
                {
                    var amount = int.Parse(parts[i]);
                    var colour = parts[i + 1];
                    maximums[colour] = Math.Max(maximums[colour], amount);
                }

                var gamePower = maximums["red"] * maximums["green"] * maximums["blue"];
                total += gamePower;
            }
            Console.WriteLine(total);
        }
    }
}
