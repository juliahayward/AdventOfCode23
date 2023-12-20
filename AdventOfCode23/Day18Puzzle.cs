using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day18Puzzle : PuzzleBase
    {
        internal static void DoPartOne(bool example)
        {
            var lines = ReadLines(18, example);
            var instructions = new List<Instruction>();
            foreach (var line in lines)
            {
                var pieces = line.Split(' ');
                instructions.Add(new Instruction()
                {
                    direction = pieces[0][0],
                    distance = int.Parse(pieces[1]),
                    colour = pieces[2].Replace("(", "").Replace(")", "")
                });
            }
            // First work out what size grid we need to fit in
            var position = (x: 0, y: 0);
            var maxPosition = (x: 0, y: 0);
            var minPosition = (x: 0, y: 0);
            foreach (var instruction in instructions)
            {
                switch (instruction.direction)
                {
                    case 'U': 
                        position.y -= instruction.distance;
                        minPosition.y = Math.Min(minPosition.y, position.y);
                        break;
                    case 'D':
                        position.y += instruction.distance;
                        maxPosition.y = Math.Max(maxPosition.y, position.y);
                        break;
                    case 'L':
                        position.x -= instruction.distance;
                        minPosition.x = Math.Min(minPosition.x, position.x);
                        break;
                    case 'R':
                        position.x += instruction.distance;
                        maxPosition.x = Math.Max(maxPosition.x, position.x);
                        break;
                }
            }

            if (position.x != 0 || position.y != 0)
                throw new InvalidDataException("Didn't return to start!");
            // In the example if we start at 0, 0 we don't stray into negative territory, but
            // you can be sure the real data does!!

            int width = maxPosition.x - minPosition.x + 1;
            int height = maxPosition.y - minPosition.y + 1;
            position = (x: -minPosition.x, y: -minPosition.y);
            // This means the grid should run from 0, 0 and be big enough
            var cells = new Cell[width, height];
            for (int i=0; i<width; i++)
                for (int j=0; j<height; j++)
                    cells[i,j] = new Cell();

            cells[position.x, position.y].IsDugOut = true;
            foreach (var instruction in instructions)
            {
                for (int i = 0; i < instruction.distance; i++)
                {
                    switch (instruction.direction)
                    {
                        case 'U':
                            position.y--;
                            break;
                        case 'D':
                            position.y++;
                            break;
                        case 'L':
                            position.x--;
                            break;
                        case 'R':
                            position.x++;
                            break;
                    }
                    cells[position.x, position.y].IsDugOut = true;
                }
            }

            // Now we need to dig out the rest. Unlike a previous day we don't have
            // the nice ray-tracing approach as our digger may (almost certainly will!)
            // cross its own path. Basically any cell on the edge is "out", and repeatedly
            // sweep through seeing if neighbours are also out.
            for (int i = 0; i < width; i++)
            {
                if (!cells[i, 0].IsDugOut)
                    cells[i, 0].IsOutside = true;
                if (!cells[i, height - 1].IsDugOut)
                    cells[i, height - 1].IsOutside = true;
            }
            for (int j = 0; j < height; j++)
            {
                if (!cells[0, j].IsDugOut)
                    cells[0, j].IsOutside = true;
                if (!cells[width - 1, j].IsDugOut)
                    cells[width-1, j].IsOutside = true;
            }

            bool hasMadeProgress;
            do
            {
                hasMadeProgress = false;
                for (int i = 1; i < width - 1; i++)
                for (int j = 1; j < height - 1; j++)
                {
                    var neighbours = new[]
                    {
                        cells[i - 1, j], cells[i + 1, j], cells[i, j - 1], cells[i, j + 1]
                    };
                    if (!cells[i, j].IsOutside && !cells[i,j].IsDugOut
                        && neighbours.Any(c => c.IsOutside))
                    {
                        cells[i, j].IsOutside = true;
                        hasMadeProgress = true;
                    }
                }
            } while (hasMadeProgress);

            int countOutsideCells = 0;
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (cells[i, j].IsOutside) countOutsideCells++;
            }

            Console.WriteLine(width*height - countOutsideCells);


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(cells[x, y].IsDugOut ? '#' : 
                        (cells[x,y].IsOutside ? 'O' : '.'));
                }
                Console.WriteLine();
            }
        }

        internal static void DoPartTwo(bool example)
        {
            var lines = ReadLines(18, example);
            var instructions = new List<Instruction>();
            foreach (var line in lines)
            {
                var pieces = line.Split(' ');
                instructions.Add(new Instruction()
                {
                    direction = "RDLU"[int.Parse(pieces[2].Substring(7, 1))],
                    distance = int.Parse(pieces[2].Substring(2, 5),
                        System.Globalization.NumberStyles.HexNumber),
                });
            }

            // First work out what size grid we need to fit in. In this case, it would
            // be ludicrous to create a grid at full size... so here, we record which
            // columns and rows we dig along and then anything else can be lumped together...
            var position = (x: 0, y: 0);
            var maxPosition = (x: 0, y: 0);
            var minPosition = (x: 0, y: 0);
            var rowsDugAlong = new List<int>();
            var columnsDugAlong = new List<int>();
            foreach (var instruction in instructions)
            {
                switch (instruction.direction)
                {
                    case 'U':
                        position.y -= instruction.distance;
                        minPosition.y = Math.Min(minPosition.y, position.y);
                        columnsDugAlong.Add(position.x);
                        break;
                    case 'D':
                        position.y += instruction.distance;
                        maxPosition.y = Math.Max(maxPosition.y, position.y);
                        columnsDugAlong.Add(position.x);
                        break;
                    case 'L':
                        position.x -= instruction.distance;
                        minPosition.x = Math.Min(minPosition.x, position.x);
                        rowsDugAlong.Add(position.y);
                        break;
                    case 'R':
                        position.x += instruction.distance;
                        maxPosition.x = Math.Max(maxPosition.x, position.x);
                        rowsDugAlong.Add(position.y);
                        break;
                }
            }

            if (position.x != 0 || position.y != 0)
                throw new InvalidDataException("Didn't return to start!");
            
            columnsDugAlong.Sort();
            rowsDugAlong.Sort();
            var widthPairs = new List<(int, int, bool)>(); // start, end, isDug
            foreach (var x in columnsDugAlong.Distinct())
            {
                if (widthPairs.Any() && x > widthPairs.Last().Item2 + 1)
                    widthPairs.Add((widthPairs.Last().Item2 + 1, x-1, false));
                widthPairs.Add((x, x, false));
            }
            var heightPairs = new List<(int, int, bool)>();
            foreach (var y in rowsDugAlong.Distinct())
            {
                if (heightPairs.Any() && y > heightPairs.Last().Item2 + 1)
                    heightPairs.Add((heightPairs.Last().Item2 + 1, y - 1, false));
                heightPairs.Add((y, y, false));
            }

            // This means the grid should run from 0, 0 and be big enough
            var cells = new RangeCell[widthPairs.Count, heightPairs.Count];
            int width = widthPairs.Count;
            int height = heightPairs.Count;
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                cells[i, j] = new RangeCell()
                {
                    xmin = widthPairs[i].Item1,
                    xmax = widthPairs[i].Item2,
                    ymin = heightPairs[j].Item1,
                    ymax = heightPairs[j].Item2,
                    IsDugOut = false
                };

            // Now work out which ones were dug out. We don't need distances, we just
            // take the edge (of arbitrary length) and the next corner
            position = (0, 0);
            var indexPosition = LocatePosition(cells, position);
            cells[indexPosition.x, indexPosition.y].IsDugOut = true;
            foreach (var instruction in instructions)
            {
                switch (instruction.direction)
                {
                    case 'U':
                        var newPosition = (position.x, position.y - instruction.distance);
                        var targetIndexPosition = LocatePosition(cells, newPosition);
                        for (int j = indexPosition.y; j >= targetIndexPosition.y; j--)
                            cells[indexPosition.x, j].IsDugOut = true;
                        position = newPosition;
                        indexPosition = targetIndexPosition;
                        break;
                    case 'D':
                        newPosition = (position.x, position.y + instruction.distance);
                        targetIndexPosition = LocatePosition(cells, newPosition);
                        for (int j = indexPosition.y; j <= targetIndexPosition.y; j++)
                            cells[indexPosition.x, j].IsDugOut = true;
                        position = newPosition;
                        indexPosition = targetIndexPosition;
                        break;
                    case 'L':
                        newPosition = (position.x - instruction.distance, position.y);
                        targetIndexPosition = LocatePosition(cells, newPosition);
                        for (int i = indexPosition.x; i >= targetIndexPosition.x; i--)
                            cells[i, indexPosition.y].IsDugOut = true;
                        position = newPosition;
                        indexPosition = targetIndexPosition;
                        break;
                    case 'R':
                        newPosition = (position.x + instruction.distance, position.y);
                        targetIndexPosition = LocatePosition(cells, newPosition);
                        for (int i = indexPosition.x; i <= targetIndexPosition.x; i++)
                            cells[i, indexPosition.y].IsDugOut = true;
                        position = newPosition;
                        indexPosition = targetIndexPosition;
                        break;
                }
            }

            // Now we can do the same as part 1, but our grid squares actually represent
            // large amounts of real estate
            for (int i = 0; i < width; i++)
            {
                if (!cells[i, 0].IsDugOut)
                    cells[i, 0].IsOutside = true;
                if (!cells[i, height - 1].IsDugOut)
                    cells[i, height - 1].IsOutside = true;
            }
            for (int j = 0; j < height; j++)
            {
                if (!cells[0, j].IsDugOut)
                    cells[0, j].IsOutside = true;
                if (!cells[width - 1, j].IsDugOut)
                    cells[width - 1, j].IsOutside = true;
            }

            bool hasMadeProgress;
            do
            {
                hasMadeProgress = false;
                for (int i = 1; i < width - 1; i++)
                for (int j = 1; j < height - 1; j++)
                {
                    var neighbours = new[]
                    {
                        cells[i - 1, j], cells[i + 1, j], cells[i, j - 1], cells[i, j + 1]
                    };
                    if (!cells[i, j].IsOutside && !cells[i, j].IsDugOut
                                               && neighbours.Any(c => c.IsOutside))
                    {
                        cells[i, j].IsOutside = true;
                        hasMadeProgress = true;
                    }
                }
            } while (hasMadeProgress);

            long countOutsideCells = 0;
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (cells[i, j].IsOutside) countOutsideCells += cells[i, j].Size;
            }

            Console.WriteLine((long)(columnsDugAlong.Last() - columnsDugAlong.First() + 1) 
                              * (long)(rowsDugAlong.Last() - rowsDugAlong.First() + 1)
                              - countOutsideCells);
        }

        private static (int x, int y) LocatePosition(RangeCell[,] cells, (int x, int y) realPosition)
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j].xmax == realPosition.x && cells[i, j].ymax == realPosition.y)
                    return (i, j);
            }

            throw new ArgumentOutOfRangeException();
        }

        internal class Instruction
        {
            internal char direction;
            internal int distance;
            internal string colour;
        }

        internal class Cell
        {
            internal bool IsDugOut;
            internal bool IsOutside;
        }
        internal class RangeCell : Cell
        {
            internal int xmin, xmax, ymin, ymax;

            public override string ToString()
                => $"({xmin}-{xmax}) x ({ymin}-{ymax}), {IsDugOut}";
            internal long Size => (long)(xmax - xmin + 1) * (long)(ymax - ymin + 1);
        }
    }
}
