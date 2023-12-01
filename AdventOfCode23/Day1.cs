using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode23
{
    internal class Day1Puzzle : PuzzleBase
    {
        internal static void DoPartOne(bool example)
        {
            var lines = ReadLines(1, example);

            var answer = lines.Sum(line =>
                ToDigit(line.First(IsDigit)) * 10 + ToDigit(line.Last(IsDigit)));

            Console.WriteLine(answer);
        }

        internal static void DoPartTwo(bool example)
        {
            var lines = ReadLines(1, example);

            var cleanedLines = lines.Select(ReplaceWordsWithDigits);

            Console.WriteLine(string.Join("\r\n", cleanedLines));

            var answer = cleanedLines.Sum(line =>
                ToDigit(line.First(IsDigit)) * 10 + ToDigit(line.Last(IsDigit)));

            Console.WriteLine(answer);
        }

        private static bool IsDigit(char c) => c >= '0' && c <= '9';

        private static int ToDigit(char c) => c - '0';

        // Note - "eightwo" resolves as "8wo", so chained Replace() doesn't work
        private static Dictionary<string, string> wordsAndNumbers = new Dictionary<string, string>
        {
            { "one", "1" },
            { "two", "2" },
            { "three", "3" },
            { "four", "4" },
            { "five", "5" },
            { "six", "6" },
            { "seven", "7" },
            { "eight", "8" },
            { "nine", "9" },
        };
        private static string ReplaceWordsWithDigits(string text)
        {
            if (text == String.Empty) return text;

            foreach (var word in wordsAndNumbers.Keys)
            {
                if (text.StartsWith(word))
                    return wordsAndNumbers[word] + ReplaceWordsWithDigits(text.Substring(1));
            }

            return text[0] + ReplaceWordsWithDigits(text.Substring(1));
        }
    }
}
