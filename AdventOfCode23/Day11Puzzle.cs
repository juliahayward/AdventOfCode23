using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day11Puzzle : PuzzleBase
    {
        internal static void Do(bool example, bool partTwo)
        {
            int expansionFactor = (partTwo) ? 1000000 : 2;

            var grid = ReadLinesAsGrid(11, example);
            var stars = new List<(int, int)>();
            for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Cells[x,y] == '#') stars.Add((x, y));
            }

            var expandingXs = new List<int>();
            for (int x=0; x < grid.Width; x++)
                if (!stars.Any(s => s.Item1 == x))
                    expandingXs.Add(x);
            var expandingYs = new List<int>();
            for (int y=0; y < grid.Height; y++)
                if (!stars.Any(s => s.Item2 == y))
                    expandingYs.Add(y);

            // Distance between star i and star j using taxicab metric - allowing
            // for expansion
            long total = 0;
            for (int i = 0; i < stars.Count - 1; i++)
            for (int j = i + 1; j < stars.Count; j++)
            {
                int xmin = Math.Min(stars.ElementAt(i).Item1, stars.ElementAt(j).Item1);
                int xmax = Math.Max(stars.ElementAt(i).Item1, stars.ElementAt(j).Item1);
                int ymin = Math.Min(stars.ElementAt(i).Item2, stars.ElementAt(j).Item2);
                int ymax = Math.Max(stars.ElementAt(i).Item2, stars.ElementAt(j).Item2);

                int taxicabDistance = (xmax - xmin) + (ymax - ymin);
                int xExpansion = expandingXs.Count(x => x > xmin && x < xmax);
                int yExpansion = expandingYs.Count(y => y > ymin && y < ymax);
                long realDistance = taxicabDistance 
                                   + (xExpansion + yExpansion) * (expansionFactor - 1);
                total += realDistance;
            }
            Console.WriteLine(total);
        }
    }
}
