using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode23.PuzzleBase;

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

        public class Grid
        {
            public Grid(List<string> lines)
            {
                Width = lines[0].Length;
                Height = lines.Count;
                Cells = new char[Width, Height];
                for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Cells[x, y] = lines[y][x];
            }

            public char[,] Cells { get; }

            public char GetChar(int x, int y)
            {
                if (x >= 0 && y >= 0 && x < Width && y < Height)
                    return Cells[x, y];
                return (char)0;
            }

            public int CountChar(char target)
            {
                int count = 0;
                for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (Cells[i, j] == target)
                        count++;
                return count;
            }

            public int Width { get; }
            public int Height { get; }

            public void Write()
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Console.Write(Cells[x, y]);
                    }
                    Console.WriteLine();
                }
            }
        }

        protected static Grid ReadLinesAsGrid(int day, bool example)
        {
            var lines = ReadLines(day, example);
            return new Grid(lines);
        }

        protected static IEnumerable<Grid> ReadLinesAsGrids(int day, bool example)
        {
            var lines = ReadLines(day, example);
            var gridLines = new List<string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    yield return new Grid(gridLines);
                    gridLines.Clear();
                }
                else
                {
                    gridLines.Add(line);
                }
            }

            yield return new Grid(gridLines);
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

        // GCF and LCM
        protected static long GCF(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        protected static long LCM(long a, long b)
        {
            return (a / GCF(a, b)) * b;
        }

        protected static long LCM(IList<long> values)
        {
            while (values.Count() > 1)
            {
                var newValue = LCM(values.First(), values.ElementAt(1));
                values = values.Skip(2).Append(newValue).ToList();
            }
            return values.First();
        }

    }
}
