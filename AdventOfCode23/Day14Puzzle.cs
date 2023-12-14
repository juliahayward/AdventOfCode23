using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day14Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var grid = ReadLinesAsGrid(14, example);
            var seenKeys = new Dictionary<string, int>();

            RollStonesNorth(grid);
            // part 1 answer
            Console.WriteLine(NorthWeight(grid));
            RollStonesWest(grid);
            RollStonesSouth(grid);
            RollStonesEast(grid);
            seenKeys.Add(GetGridKey(grid), 1);
            Console.WriteLine(GetGridKey(grid));
            // Clearly we're not going to run this for 1bn iterations. 1m takes
            // about 4 seconds on the example set. If we see a repeated configuration
            // after a NWSE cycle (which we must do eventually!), then we know we're
            // stuck in a loop and can short-circuit a lot of cycles. Eg. in the example
            // data, the position after 10 NWSE cycles is the same as after 3, so we
            // know it will also be the same after 17, 24, 31, ...
            int upperLimit = 1000000000;
            for (int cycle = 2; cycle <= upperLimit; cycle++)
            {
                RollStonesNorth(grid);
                RollStonesWest(grid);
                RollStonesSouth(grid);
                RollStonesEast(grid);
                var key = GetGridKey(grid);
                if (seenKeys.ContainsKey(key))
                {
                    int cycleLoopLength = cycle - seenKeys[key];
                    Console.WriteLine("cycle " + cycle + " repeats " + seenKeys[key]);
                    var loopsToShortCircuit = (upperLimit - cycle) / cycleLoopLength;
                    upperLimit -= cycleLoopLength * loopsToShortCircuit;
                    // Don't fall into this again!
                    seenKeys.Clear();
                }
                seenKeys.Add(GetGridKey(grid), cycle);
            }

            Console.WriteLine(NorthWeight(grid));
        }

        private static void RollStonesNorth(Grid grid)
        {
            for (int x = 0; x < grid.Width; x++)
            for (int y = 1; y < grid.Height; y++)
            {
                if (grid.Cells[x, y] == 'O')
                {
                    int targetY = y;
                    for (int yy = y - 1; yy >= 0; yy--)
                    {
                        if (grid.Cells[x, yy] == '.')
                            targetY = yy;
                        else
                            break;
                    }

                    grid.Cells[x, y] = '.';
                    grid.Cells[x, targetY] = 'O';
                }
            }
        }

        private static void RollStonesWest(Grid grid)
        {
            for (int x = 1; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Cells[x, y] == 'O')
                {
                    int targetX = x;
                    for (int xx = x - 1; xx >= 0; xx--)
                    {
                        if (grid.Cells[xx, y] == '.')
                            targetX = xx;
                        else
                            break;
                    }
                    grid.Cells[x, y] = '.';
                    grid.Cells[targetX, y] = 'O';
                }
            }
        }

        private static void RollStonesSouth(Grid grid)
        {
            for (int x = 0; x < grid.Width; x++)
            for (int y = grid.Height - 2; y >= 0; y--)
            {
                if (grid.Cells[x, y] == 'O')
                {
                    int targetY = y;
                    for (int yy = y + 1; yy < grid.Height; yy++)
                    {
                        if (grid.Cells[x, yy] == '.')
                            targetY = yy;
                        else
                            break;
                    }
                    grid.Cells[x, y] = '.';
                    grid.Cells[x, targetY] = 'O';
                }
            }
        }

        private static void RollStonesEast(Grid grid)
        {
            for (int x = grid.Width - 2; x >= 0; x--)
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Cells[x, y] == 'O')
                {
                    int targetX = x;
                    for (int xx = x + 1; xx < grid.Width; xx++)
                    {
                        if (grid.Cells[xx, y] == '.')
                            targetX = xx;
                        else
                            break;
                    }
                    grid.Cells[x, y] = '.';
                    grid.Cells[targetX, y] = 'O';
                }
            }
        }
        private static int NorthWeight(Grid grid)
        {
            int total = 0;
            for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Cells[x, y] == 'O')
                    total += (grid.Height - y);
            }
            return total;
        }

        // A simple string representation of where the stones are
        private static string GetGridKey(Grid grid)
        {
            var key = new StringBuilder();
            for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Cells[x, y] == 'O')
                    key.Append((char)('0' + x)).Append((char)('0' + y));
            }
            return key.ToString();
        }
    }
}
