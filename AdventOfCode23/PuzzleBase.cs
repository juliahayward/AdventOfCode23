using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class PuzzleBase
    {
        protected static List<string> ReadLines(int day, bool example)
        {
            var lines = new List<string>();
            var exampleName = (example) ? "-example" : "";
            using (var reader = new StreamReader(new FileStream(
                       $"..\\..\\..\\RawData\\{day}{exampleName}.txt", FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return lines;
        }

        protected static bool IsDigit(char c) => c >= '0' && c <= '9';

        protected static int ToDigit(char c) => c - '0';

        // Does out-of-bounds checking
        protected static char? SafeCharAt(List<string> lines, int x, int y)
        {
            if (y < 0 || y >= lines.Count) return null;
            if (x < 0 || x >= lines[y].Length) return null;
            return lines[y][x];
        }
    }
}
