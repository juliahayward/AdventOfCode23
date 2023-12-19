using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode23
{
    internal class Day16Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var grid = ReadLinesAsGrid(16, example);
            // Part 1
            var initialBeam = new Beam() { x = -1, y = 0, direction = 'E' };
            var totalCells = CountCellsIlluminated(initialBeam, grid);
            Console.WriteLine(totalCells);

            // Part 2
            var maxCells = 0;
            for (int y = 0; y < grid.Height; y++)
            {
                var beam = new Beam() { x = -1, y = y, direction = 'E' };
                maxCells = Math.Max(maxCells, CountCellsIlluminated(beam, grid));
                beam = new Beam() { x = grid.Width, y = y, direction = 'W' };
                maxCells = Math.Max(maxCells, CountCellsIlluminated(beam, grid));
            }
            for (int x = 0; x < grid.Width; x++)
            {
                var beam = new Beam() { x = x, y = -1, direction = 'S' };
                maxCells = Math.Max(maxCells, CountCellsIlluminated(beam, grid)); 
                beam = new Beam() { x = x, y = grid.Height, direction = 'N' };
                maxCells = Math.Max(maxCells, CountCellsIlluminated(beam, grid));
            }

            Console.WriteLine(maxCells);
        }

        private static int CountCellsIlluminated(Beam initialBeam, Grid grid)
        {
            var cellsVisited = new string[grid.Width, grid.Height];
            FillArray(cellsVisited);
            ;
            var currentBeams = new List<Beam>();
            currentBeams.Add(initialBeam);

            while (currentBeams.Any(x => !x.hasLeftGrid))
            {
                var beam = currentBeams.First(x => !x.hasLeftGrid);
                MoveBeam(beam, grid, currentBeams, cellsVisited);
            }

            return CountTrues(cellsVisited);
        }

        internal class Beam
        {
            internal int x, y;
            internal char direction;
            internal bool hasLeftGrid = false;
        }
        
        // It's possible that you end up in a loop because you can enter the loop via
        // the passive direction of a splitter, and end up revisiting it via the splitting
        // direction
        private static void MoveBeam(Beam beam, Grid grid, List<Beam> beams, string[,] cellsVisited)
        {
            switch (beam.direction)
            {
                case 'N':
                    beam.y -= 1;
                    if (beam.y < 0 || cellsVisited[beam.x, beam.y].Contains('N'))
                        beam.hasLeftGrid = true;
                    else
                    {
                        cellsVisited[beam.x, beam.y] += 'N';
                        if (grid.Cells[beam.x, beam.y] == '\\')
                            beam.direction = 'W';
                        if (grid.Cells[beam.x, beam.y] == '/')
                            beam.direction = 'E';
                        if (grid.Cells[beam.x, beam.y] == '-')
                        {
                            beam.direction = 'E';
                            beams.Add(SplitBeam(beam, 'W'));
                        }
                    }

                    break;
                case 'S':
                    beam.y += 1;
                    if (beam.y >= grid.Height || cellsVisited[beam.x, beam.y].Contains('S'))
                        beam.hasLeftGrid = true;
                    else
                    {
                        cellsVisited[beam.x, beam.y] += 'S';
                        if (grid.Cells[beam.x, beam.y] == '\\')
                            beam.direction = 'E';
                        if (grid.Cells[beam.x, beam.y] == '/')
                            beam.direction = 'W';
                        if (grid.Cells[beam.x, beam.y] == '-')
                        {
                            beam.direction = 'E';
                            beams.Add(SplitBeam(beam, 'W'));
                        }
                    }

                    break;
                case 'E':
                    beam.x += 1;
                    if (beam.x >= grid.Width || cellsVisited[beam.x, beam.y].Contains('E'))
                        beam.hasLeftGrid = true;
                    else
                    {
                        cellsVisited[beam.x, beam.y] += 'E';
                        if (grid.Cells[beam.x, beam.y] == '\\')
                            beam.direction = 'S';
                        if (grid.Cells[beam.x, beam.y] == '/')
                            beam.direction = 'N';
                        if (grid.Cells[beam.x, beam.y] == '|')
                        {
                            beam.direction = 'N';
                            beams.Add(SplitBeam(beam, 'S'));
                        }
                    }

                    break;
                case 'W':
                    beam.x -= 1;
                    if (beam.x < 0 || cellsVisited[beam.x, beam.y].Contains('W'))
                        beam.hasLeftGrid = true;
                    else
                    {
                        cellsVisited[beam.x, beam.y] += 'W';
                        if (grid.Cells[beam.x, beam.y] == '\\')
                            beam.direction = 'N';
                        if (grid.Cells[beam.x, beam.y] == '/')
                            beam.direction = 'S';
                        if (grid.Cells[beam.x, beam.y] == '|')
                        {
                            beam.direction = 'N';
                            beams.Add(SplitBeam(beam, 'S'));
                        }
                    }

                    break;
            }
        }

        private static Beam SplitBeam(Beam beam, char newDirection)
        {
            return new Beam { x = beam.x, y = beam.y, direction = newDirection };
        }

        private static int CountTrues(string[,] cells)
        {
            int count = 0;
            for (int x = 0; x < cells.GetLength(0); x++)
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y] != "") count++;
            }
            return count;
        }

        private static void FillArray(string[,] cells)
        {
            int count = 0;
            for (int x = 0; x < cells.GetLength(0); x++)
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x,y] = "";
            }
        }
    }
}
