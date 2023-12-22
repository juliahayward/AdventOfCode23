using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day21Puzzle : PuzzleBase
    {
        internal static void DoPart1(bool example)
        {
            var grid = ReadLinesAsGrid(21, example);
            int numberOfSweeps = (example) ? 6 : 64;
            for (int sweep = 1; sweep <= numberOfSweeps; sweep++)
            {
                SweepGrid(grid, sweep);
            }

            // In 64 steps we'll be on an E (or S) by chessboard parity - so don't count O's
            int countReachable = grid.CountChar('E') + grid.CountChar('S');

            Console.WriteLine(countReachable);
        }

        private static void SweepGrid(Grid grid, int sweep)
        {
            // Write O into cells if they are an ODD distance away from
            // the start by taxicab metric, E if EVEN
            var startChar = 'S';
            var reachableChar = (sweep % 2 == 0) ? 'E' : 'O';
            var previousChar = (sweep % 2 == 0) ? 'O' : 'E';
            List<(int, int)> cellsToUpdate = new List<(int, int)> (); // Don't change anything until we've done this pass
            for (int i = 0; i < grid.Width; i++)
            for (int j = 0; j < grid.Height; j++)
            {
                if (grid.GetChar(i, j) != '.') // garden
                    continue;
                var neighbours = new[]
                {
                    grid.GetChar(i + 1, j),
                    grid.GetChar(i, j + 1),
                    grid.GetChar(i - 1, j),
                    grid.GetChar(i, j - 1)
                };
                if (neighbours.Any(n => n == previousChar || n == startChar))
                    cellsToUpdate.Add((i, j));
            }

            foreach (var (i, j) in cellsToUpdate)
               grid.Cells[i, j] = reachableChar;
        }

        /* For part 2, note that the repeating grid has . all around the edge. These make
         a grid of completely blank strips. So, if we're heading for a distant tile, we can 
        get to the edge of this one, zap along the blank strips, then walk our way into the
        final one. There's also a clear path from S in each primary direction in the real
        data (which ironically makes it easier than the demo!). This means I can ignore all 
        problems with navigating #'s until the final tile in each path.
        
        The only part that isn't immediately obvious is heading to a distant tile that's
        (eg) due east of us - is it quicker to go through the middle of the tiles or head
        out to a blank strip, zap due eastwards, then head back in? That probably requires
        checking the maximum number of deviations introduced trying to cross the tile 
        */

        internal static void DoPart2(bool example) // My logic here doesn't apply to the example
        {
            var grid = ReadLinesAsGrid(21, false);
            // Grid is 11x11 or 131x131, and S is in the middle.  

            int totalNumberOfSteps = (example) ? 5000 : 26501365; // call this T
            /*
             * So, imagining a grid of tiles, I can get to every tile where |t_x|+|t_y| <= 202300.
             *
             * Tile (t_x, t_y) runs from (-65+131*t_x, -65+131*t_y) to (65+131*t_x, 65+131*t_y)
             *
             * The western edge of tile (202300, 0) is (T-130, 0) - so I can reach most of it.
             * If I start from the midpoint of the westernmost edge, I have 130 steps left.
             *
             * The south-western corner of tile (202300, 1) is (T-130, 66) - so I can reach about 1/8 of it.
             * and I can't reach any further away tiles. Similar arguments apply to the other
             * compass directions. I have 64 steps left.
             *
             * The south-western corner of tile (202299, 1) is (T-261, 66) - so I can reach about 7/8 of it.
             * and I can't reach any further away tiles. Similar arguments apply to the other
             * compass directions. I have 195 steps left.
             *
             *
             * These tiles fit together in a star like this: E meaning a full tile where I can finish on a cell with 
             * even parity relative to the centre, O likewise but odd (I must take an odd total number of steps even
             * if I circle around the start), > etc I enter at a midpoint of a side, and Tt are triangles where
             * I enter at a corner; T with 195 steps and t with 64 
             *
             *    t^t
             *   tTETt
             *  tTEOETt
             * tTEOEOETt     So for an order 3 star (0, 3 is the last full tile eastwards) I get 1 of each >
             * <EOEOEOE>                                                                         4 = (order + 1) of each t
             * tTEOEOETt                                                                         3 = (order) of each T
             *  tTEOETt                                                                         16 = (order+1)^2 of E
             *   tTETt                                                                           9 = (order)^2 of O
             *    tVt
             */

            // Just keep clearing out the grid for each form of tile!
            for (int i = 1; i <= 240; i++) SweepGrid(grid, i);
            int countFullTileEvenParity = grid.CountChar('E') + grid.CountChar('S');
            int countFullTileOddParity = grid.CountChar('O');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[0, 65] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 130; i++) SweepGrid(grid, i);
            var easternMostTile = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[130, 65] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 130; i++) SweepGrid(grid, i);
            var westernMostTile = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[65, 0] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 130; i++) SweepGrid(grid, i);
            var northernMostTile = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[65, 130] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 130; i++) SweepGrid(grid, i);
            var southernMostTile = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[0, 130] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 64; i++) SweepGrid(grid, i);
            var sw64 = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[0, 0] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 64; i++) SweepGrid(grid, i);
            var nw64 = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[130, 130] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 64; i++) SweepGrid(grid, i);
            var se64 = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[130, 0] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 64; i++) SweepGrid(grid, i);
            var ne64 = grid.CountChar('E') + grid.CountChar('S');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[0, 130] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 195; i++) SweepGrid(grid, i);
            var sw195 = grid.CountChar('O');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[0, 0] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 195; i++) SweepGrid(grid, i);
            var nw195 = grid.CountChar('O');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[130, 130] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 195; i++) SweepGrid(grid, i);
            var se195 = grid.CountChar('O');

            grid = ReadLinesAsGrid(21, false);
            grid.Cells[130, 0] = 'S';
            grid.Cells[65, 65] = '.';
            for (int i = 1; i <= 195; i++) SweepGrid(grid, i);
            var ne195 = grid.CountChar('O');

            long order = 202299;

            long total = order * order * countFullTileOddParity
                         + (order+1) * (order + 1) * countFullTileEvenParity
                         + (order + 1) * (ne64 + nw64 + se64 + sw64)
                         + order * (ne195 + nw195 + se195 + sw195)
                         + (easternMostTile + southernMostTile + northernMostTile + westernMostTile);

            Console.WriteLine(total);
            // 630661863455116 !!
        }
    }
}
