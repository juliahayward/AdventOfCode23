using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day15Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var lines = ReadLines(15, example);
            var sum = lines[0].Split(',').Sum(Hash);
            Console.WriteLine(sum);

            Dictionary<int, List<Lens>> Boxes = new Dictionary<int, List<Lens>>();
            for (int i=0; i<256; i++ )
                Boxes[i] = new List<Lens>();

            var instructions = lines[0].Split(',');
            foreach (var instruction in instructions)
            {
                if (instruction.Contains("-"))
                {
                    var label = instruction.Replace("-", "");
                    var boxNumber = Hash(label);
                    Boxes[boxNumber].RemoveAll(l => l.Label == label);
                }

                if (instruction.Contains("="))
                {
                    var label = instruction.Split('=')[0];
                    var boxNumber = Hash(label);
                    var focalLength = int.Parse(instruction.Split('=')[1]);
                    var existingLens = Boxes[boxNumber].FirstOrDefault(l => l.Label == label);
                    if (existingLens != null)
                        existingLens.FocalLength = focalLength;
                    else
                        Boxes[boxNumber].Add(new Lens {Label = label, FocalLength = focalLength});
                }
            }

            long total = 0;
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < Boxes[i].Count; j++)
                {
                    total += (i + 1) * (j + 1) * Boxes[i][j].FocalLength;
                }
            }

            Console.WriteLine(total);
        }

        internal class Lens
        {
            internal string Label;
            internal int FocalLength;
        }

        internal static int Hash(string s)
        {
            var hash = 0;
            foreach (char c in s)
            {
                hash += (int)c;
                hash *= 17;
                hash = hash % 256;
            }

            return hash;
        }
    }
}
