using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AdventOfCode23
{
    internal class Day3Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var lines = ReadLines(3, example);
            bool isInNumber = false;
            // startX, endX, Y, value
            var numbersInGrid = new List<(int, int, int, int)>();
            int currentNumber = 0, startX = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if (!isInNumber && IsDigit(lines[i][j]))
                    {
                        isInNumber = true;
                        startX = j;
                        currentNumber = ToDigit(lines[i][j]);
                    }
                    else if (IsDigit(lines[i][j]))
                    {
                        currentNumber = 10 * currentNumber + ToDigit(lines[i][j]);
                    }
                    else if (isInNumber && !IsDigit(lines[i][j]))
                    {
                        isInNumber = false;
                        numbersInGrid.Add((startX, j - 1, i, currentNumber));
                    }
                }

                if (isInNumber)
                {
                    isInNumber = false;
                    numbersInGrid.Add((startX, lines[i].Length, i, currentNumber));
                }
            }
            // Annoyingly, it's ambiguous as to whether - counts as negative numbers,
            // a symbol, or both. Fortunately I guessed right - all part numbers are
            // positive and - is a symbol

            // Now we've spotted the numbers, scan around each one for a symbol
            var totalPartNumbers = numbersInGrid.Where(x => IsAPartNumber(x, lines)).Sum(x => x.Item4);
            Console.WriteLine("Part 1 answer: " + totalPartNumbers);

            var stars = new List<(int, int)>();
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if (SafeCharAt(lines, j, i) == '*')
                        stars.Add((j, i));
                }
            }

            var totalRatios = 0;
            foreach (var star in stars)
            {
                var numbersNextToStar = numbersInGrid.Where(n => IsNextToStar(n, star));
                if (numbersNextToStar.Count() == 2)
                    totalRatios += numbersNextToStar.First().Item4 *
                                   numbersNextToStar.Last().Item4;
            }

            Console.WriteLine("Part 2 answer: " + totalRatios);
        }

        private static bool IsAPartNumber((int, int, int, int) number, List<string> lines)
        {
            // search including the number itself for simplicity
            for (int x = number.Item1 - 1; x <= number.Item2 + 1; x++)
                for (int y = number.Item3 - 1; y <= number.Item3 + 1; y++)
                    if (IsSymbol(SafeCharAt(lines, x, y)))
                    return true;
            return false;
        }

        private static bool IsNextToStar((int, int, int, int) number, (int, int) star)
        {
            return (star.Item2 >= number.Item3 - 1 && star.Item2 <= number.Item3 + 1 
                 && star.Item1 >= number.Item1 - 1 && star.Item1 <= number.Item2 + 1);
        }

        private static bool IsSymbol(char? c)
            => c != null && c != '.' && (c < '0' || c > '9');
    }
}
