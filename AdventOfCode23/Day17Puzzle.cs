using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdventOfCode23.PuzzleBase;

namespace AdventOfCode23
{
    internal class Day17Puzzle : PuzzleBase
    {
        static Dictionary<string, int> _cache = new Dictionary<string, int>();
        internal static void Do(bool example)
        {
            var grid = ReadLinesAsGrid(17, example);
            // Note that the optimal path may loop back on itself given
            // length constraints. The state we're in includes the (x,y) we're
            // at, the direction we're facing, and the number of steps we have
            // already taken in that direction.
            var heatLoss = FindPathWithMinimumHeatLoss(0, 0, ' ', 0, grid);
            Console.WriteLine(heatLoss);
        }

        private static int FindPathWithMinimumHeatLoss(int x, int y, char facing, int stepsUsed, Grid grid)
        {
            if (_cache.ContainsKey(x + "," + y + "," + facing + "," + stepsUsed))
                return _cache[x + "," + y + "," + facing + "," + stepsUsed];

            // is west an option?
            int? optionW = null;
            if (x > 0 && !(facing == 'W' && stepsUsed == 3))
            {
                int newStepsUsed = (facing == 'W') ? stepsUsed + 1 : 1;
                optionW = (grid.Cells[x - 1, y] - '0')
                    + FindPathWithMinimumHeatLoss(x -1, y, 'W', stepsUsed, grid);
            }
            int? optionN = null;
            if (y > 0 && !(facing == 'N' && stepsUsed == 3))
            {
                int newStepsUsed = (facing == 'N') ? stepsUsed + 1 : 1;
                optionN = (grid.Cells[x, y - 1] - '0')
                          + FindPathWithMinimumHeatLoss(x, y - 1, 'N', stepsUsed, grid);
            }
            int? optionS = null;
            if (y < grid.Height - 1 && !(facing == 'S' && stepsUsed == 3))
            {
                int newStepsUsed = (facing == 'S') ? stepsUsed + 1 : 1;
                optionS = (grid.Cells[x, y + 1] - '0')
                          + FindPathWithMinimumHeatLoss(x, y + 1, 'S', stepsUsed, grid);
            }

            int? optionE = null;
            if (x < grid.Width - 1 && !(facing == 'E' && stepsUsed == 3))
            {
                int newStepsUsed = (facing == 'E') ? stepsUsed + 1 : 1;
                optionS = (grid.Cells[x + 1, y] - '0')
                          + FindPathWithMinimumHeatLoss(x + 1, y, 'E', stepsUsed, grid);
            }

            var min = new [] { optionE, optionS, optionW, optionN }
                .Where(i => i != null).Select(i => i.Value).Min();

            _cache.Add(x + "," + y + "," + facing + "," + stepsUsed, min);
            return min;
        }
    }
}
