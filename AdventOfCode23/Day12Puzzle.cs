using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day12Puzzle : PuzzleBase
    {
        private static long _count = 0;
        private static Dictionary<string, long> _cache = new Dictionary<string, long>();
        internal static void Do(bool example, bool isPartTwo)
        {
            var lines = ReadLines(12, example);

            var totalArrangements = lines.Sum(x => ArrangementsInLine(x, isPartTwo));

            Console.WriteLine(totalArrangements);
        }

        internal static long ArrangementsInLine(string line, bool isPartTwo)
        {
            var parts = line.Split(' ');
            var rawSymbols = parts[0];
            var rawGroups = parts[1];
            if (isPartTwo)
            {
                rawSymbols = string.Join("?", rawSymbols, rawSymbols, rawSymbols, rawSymbols, rawSymbols);
                rawGroups = string.Join(",", rawGroups, rawGroups, rawGroups, rawGroups, rawGroups);
            }
            // We can shorten the recursion by replacing any sequence of dots with
            // a single dot
            while (rawSymbols.Contains(".."))
                rawSymbols = rawSymbols.Replace("..", ".");

            var symbols = rawSymbols;
            var groupSizes = rawGroups.Split(',')
                .Select(int.Parse)
                .ToArray();

            _cache.Clear();
            var count = ArrangementsInLine(symbols, groupSizes, 0, groupSizes.Sum());
            Console.WriteLine(count + " " + line);
            Console.WriteLine(_count);
            return count;
        }

        // We keep track of the sum of remaining groups, rather than repeatedly summing the lists
        // and the group we're trying to place, rather than repeatedly constructing shorter lists
        internal static long ArrangementsInLine(string symbols, int[] groupSizes, int indexOfNextGroupToPlace, int groupSum)
        {
            _count++;
            if (_cache.ContainsKey(symbols + indexOfNextGroupToPlace))
                return _cache[symbols + indexOfNextGroupToPlace];

            // We've finished placing groups - so we have a valid arrangement
            // provided that there are no remaining # that haven't been matched
            if (groupSum < symbols.ToCharArray().Count(x => x == '#'))
                return 0;
            if (groupSum == 0)
                return 1;

            // Does the first group match the head of the string?
            var firstSize = groupSizes[indexOfNextGroupToPlace];
            var matchesFirstChar = MatchesFirstChar(symbols, firstSize);
            var arrangementsMatchingFirstChar = 0l;
            if (matchesFirstChar)
            {
                var substringAfterMatch = 
                    (symbols.Length == firstSize)
                    ? "" // this group uses the string up exactly
                    : symbols.Substring(firstSize + 1); // needs a . at the end of the first group
                arrangementsMatchingFirstChar =
                    ArrangementsInLine(substringAfterMatch,
                        groupSizes,
                        indexOfNextGroupToPlace + 1,
                        groupSum - firstSize);
            }

            var arrangementsNotMatchingFirstChar = 0l;
            if (symbols.Length > 0)
            {
                // If symbols starts with '#' then no point continuing, as we'll always
                // fail to match that # if we skip past it.
                if (symbols[0] != '#')
                {
                    arrangementsNotMatchingFirstChar = 
                        ArrangementsInLine(symbols.Substring(1), groupSizes, indexOfNextGroupToPlace, groupSum);
                }
            }
            
            _cache.Add(symbols + indexOfNextGroupToPlace, arrangementsNotMatchingFirstChar + arrangementsMatchingFirstChar);
            return arrangementsMatchingFirstChar + arrangementsNotMatchingFirstChar;
        }

        internal static bool MatchesFirstChar(string symbols, int size)
        {
            // We want to place a group of (size) #, followed by . if this is not the last
            // group, at the beginning of the symbol string
            if (symbols.Length < size) return false;        // not long enough
            if (symbols.Substring(0, size).Contains('.')) return false;
            if (symbols.Length > size) 
            {
                if (symbols[size] == '#') return false;     // string continues, can't have a . after the group
            }
            return true;
        }
    }
}
