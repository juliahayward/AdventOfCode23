using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode23
{
    internal class Day9Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var lines = ReadLines(9, example);

            var totalPart1 = lines.Sum(l => ExtrapolatedValues(l).next);
            var totalPart2 = lines.Sum(l => ExtrapolatedValues(l).prev);

            Console.WriteLine("Part 1: " + totalPart1);
            Console.WriteLine("Part 2: " + totalPart2);
        }

        internal static (int prev, int next) ExtrapolatedValues(string line)
        {
            var rows = new List<List<int>>();
            var initialValues = line.Split(' ').Select(x => int.Parse(x)).ToList();
            rows.Add(initialValues);
            do
            {
                rows.Add(DeriveNextRow(rows.Last()).ToList());
            } 
            while (rows.Last().Any(x => x != 0));
            // The next value in initialValues will be the sum of all the
            // final values in the derived sequences; the previous one will
            // be similar but we're flipping signs as we work our way
            // backwards
            var predictedNext = rows.Sum(x => x.Last());
            var predictedPrev = FlipAlternateSigns(rows.Select(x => x.First())).Sum();
            return (predictedPrev, predictedNext);
        }

        internal static IEnumerable<int> DeriveNextRow(IEnumerable<int> row)
        {
            int previous = row.First();
            foreach (var val in row.Skip(1))
            {
                yield return val - previous;
                previous = val;
            }
        }

        internal static IEnumerable<int> FlipAlternateSigns(IEnumerable<int> values)
        {
            int sign = 1;
            foreach (var val in values)
            {
                yield return val * sign;
                sign *= -1;
            }
        }
    }
}
