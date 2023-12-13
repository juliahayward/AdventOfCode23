using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day13Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var grids = ReadLinesAsGrids(13, example).ToList();

            Console.WriteLine(grids.Sum(GridScore));

            Console.WriteLine(grids.Sum(GridScoreAllowingForSmudge));

        }

        internal static int GridScore(Grid grid)
        {
            var totalScore = 0;
            for (int x = 0; x < grid.Width - 1; x++)
            {
                if (IsLeftSideOfMirror(grid, x))
                    totalScore += (x + 1);      // 1-indexed in puzzle!
            }
            for (int y = 0; y < grid.Height - 1; y++)
            {
                if (IsTopSideOfMirror(grid, y))
                    totalScore += 100 * (y + 1);
            }
            return totalScore;
        }

        // Doing this separately as I'd rather not lose the early returns in part 1!
        internal static int GridScoreAllowingForSmudge(Grid grid)
        {
            var totalScore = 0;
            for (int x = 0; x < grid.Width - 1; x++)
            {
                if (IsLeftSideOfMirrorWithOneSmudge(grid, x))
                    totalScore += (x + 1);      // 1-indexed in puzzle!
            }
            for (int y = 0; y < grid.Height - 1; y++)
            {
                if (IsTopSideOfMirrorWithOneSmudge(grid, y))
                    totalScore += 100 * (y + 1);
            }
            return totalScore;
        }

        internal static bool IsLeftSideOfMirror(Grid grid, int maxx)
        {
            for (int x = maxx; x >= 0; x--)
            {
                int reflectedX = maxx + 1 + (maxx - x);
                if (reflectedX >= grid.Width) break;
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.Cells[x, y] != grid.Cells[reflectedX, y])
                        return false;
                }
            }

            return true;
        }

        internal static bool IsTopSideOfMirror(Grid grid, int maxy)
        {
            for (int y = maxy; y >= 0; y--)
            {
                int reflectedY = maxy + 1 + (maxy - y);
                if (reflectedY >= grid.Height) break;
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.Cells[x, y] != grid.Cells[x, reflectedY])
                        return false;
                }
            }

            return true;
        }

        internal static bool IsLeftSideOfMirrorWithOneSmudge(Grid grid, int maxx)
        {
            bool usedSmudge = false;
            for (int x = maxx; x >= 0; x--)
            {
                int reflectedX = maxx + 1 + (maxx - x);
                if (reflectedX >= grid.Width) break;
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.Cells[x, y] != grid.Cells[reflectedX, y])
                    {
                        if (usedSmudge) return false;
                        usedSmudge = true;
                    }
                }
            }

            return usedSmudge;
        }

        internal static bool IsTopSideOfMirrorWithOneSmudge(Grid grid, int maxy)
        {
            bool usedSmudge = false;
            for (int y = maxy; y >= 0; y--)
            {
                int reflectedY = maxy + 1 + (maxy - y);
                if (reflectedY >= grid.Height) break;
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.Cells[x, y] != grid.Cells[x, reflectedY])
                    {
                        if (usedSmudge) return false;
                        usedSmudge = true;
                    }
                }
            }

            return usedSmudge;
        }
    }
}
