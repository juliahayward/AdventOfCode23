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
    }
}
