using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day10Puzzle : PuzzleBase
    {
        private static char[,] _chars;
        private static List<(int, int)> _loopCells = new List<(int, int)>();

        // note after the event - yes, (x, y) is back to front :)
        private static char CharAt((int, int)pos) => _chars[pos.Item1, pos.Item2];

        private static int _width;
        private static int _height;
        internal static void Do(bool example)
        {
            var lines = ReadLines(10, example).ToArray();
            _width = lines[0].Length;
            _height = lines.Length;
            _chars = new char[_width, _height];
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                _chars[x,y] = lines[x][y];


            var start = GetStart();
            // We don't know if our start direction is valid - we'll head off
            // and see if the loop closes, and if we hit a dead end we'll throw. Two
            // directions should work - which will let us substitute S for the right
            // loop character
            string successfulInitialDirections = "";
            foreach (var direction in new[] { 'N', 'S', 'E', 'W' })
            {
                try
                {
                    _loopCells = GetLoopCells(start, direction);
                    Console.WriteLine("Loop length: " + _loopCells.Count);
                    // The loop will have an even length by chessboard parity
                    Console.WriteLine("Furthest point: " + (_loopCells.Count / 2));
                    successfulInitialDirections += direction;
                }
                catch (Exception)
                {
                    Console.WriteLine("Bad initial direction: " + direction);
                }
            }
            // Blank off all non-loop cells
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            {
                if (!_loopCells.Contains((x, y)))
                    _chars[x, y] = '.';
            }
            // Substitute the S
            switch (successfulInitialDirections)
            {
                case "NS":
                    _chars[start.Item1, start.Item2] = '|';
                    break;
                case "NE":
                    _chars[start.Item1, start.Item2] = 'L';
                    break;
                case "NW":
                    _chars[start.Item1, start.Item2] = 'J';
                    break;
                case "SE":
                    _chars[start.Item1, start.Item2] = 'F';
                    break;
                case "SW":
                    _chars[start.Item1, start.Item2] = '7';
                    break;
                case "EW":
                    _chars[start.Item1, start.Item2] = '-';
                    break;
            }


            // Now start counting cells!
            Console.WriteLine("Total cells: " + (_width * _height));
            Console.WriteLine("Loop cells: " + _loopCells.Count);
            try
            {
                var insAndOuts = CountInsAndOuts();
                Console.WriteLine("Inside: " + insAndOuts.Item1);
                Console.WriteLine("Outside: " + insAndOuts.Item2);
            }
            catch (Exception)
            {
                WriteCells();
            }
        }

        static void WriteCells()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Console.Write(_chars[x, y]);
                }
                Console.WriteLine();
            }
        }

        static (int, int) GetStart()
        {
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                if (CharAt((x, y)) == 'S')
                    return (x, y);
            throw new InvalidDataException("No S");
        }

        // This would be so much nicer if recursive, but I get a stackoverflow...
        static List<(int, int)> GetLoopCells((int, int) currentPosition, char direction)
        {
            List<(int, int)> loopCells = new List<(int, int)>();
            loopCells.Add(currentPosition);
            var stepsSoFar = 0;
            (int, int) next = (-1, -1);
            char nextDirection = ' ';
            while (true)
            {
                switch (direction)
                {
                    // Characters in board: | - L J 7 F . S
                    case 'N':
                        next = (currentPosition.Item1 - 1, currentPosition.Item2);
                        var nextChar = CharAt(next);
                        if (nextChar == 'S')
                            return loopCells;
                        else if (nextChar == '|')
                        {
                            nextDirection = 'N';
                            break;
                        }
                        else if (nextChar == 'F')
                        {
                            nextDirection = 'E';
                            break;
                        }
                        else if (nextChar == '7')
                        {
                            nextDirection = 'W';
                            break;
                        }
                        else
                            throw new InvalidDataException("Broken pipe");
                    case 'S':
                        next = (currentPosition.Item1 + 1, currentPosition.Item2);
                        nextChar = CharAt(next);
                        if (nextChar == 'S')
                            return loopCells;
                        else if (nextChar == '|')
                        {
                            nextDirection = 'S';
                            break;
                        }
                        else if (nextChar == 'L')
                        {
                            nextDirection = 'E';
                            break;
                        }
                        else if (nextChar == 'J')
                        {
                            nextDirection = 'W';
                            break;
                        }
                        else
                            throw new InvalidDataException("Broken pipe");
                    case 'E':
                        next = (currentPosition.Item1, currentPosition.Item2 + 1);
                        nextChar = CharAt(next);
                        if (nextChar == 'S')
                            return loopCells;
                        else if (nextChar == '-')
                        {
                            nextDirection = 'E';
                            break;
                        }
                        else if (nextChar == 'J')
                        {
                            nextDirection = 'N';
                            break;
                        }
                        else if (nextChar == '7')
                        {
                            nextDirection = 'S';
                            break;
                        }
                        else
                            throw new InvalidDataException("Broken pipe");
                    case 'W':
                        next = (currentPosition.Item1, currentPosition.Item2 - 1);
                        nextChar = CharAt(next);
                        if (nextChar == 'S')
                            return loopCells;
                        else if (nextChar == '-')
                        {
                            nextDirection = 'W';
                            break;
                        }
                        else if (nextChar == 'F')
                        {
                            nextDirection = 'S';
                            break;
                        }
                        else if (nextChar == 'L')
                        {
                            nextDirection = 'N';
                            break;
                        }
                        else
                            throw new InvalidDataException("Broken pipe");
                    default:
                        throw new InvalidDataException("Bad direction");
                }

                stepsSoFar += 1;
                loopCells.Add(next);
                currentPosition = next;
                direction = nextDirection;
            }
        }

        // Would be faster to actively blank off non loop cells rather than checking the list
        // repeatedly

        static (int, int) CountInsAndOuts()
        {
            int ins = 0, outs = 0;
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            {
                if (CharAt((x, y)) != '.')
                    continue;
                // Look north from each point not on the loop. If we get to the
                // boundary with no loop cells, we're OUT. Each - flips our parity.
                // A loop that intrudes into this line and back out again the same
                // side (i.e. a J...7 pair or L...F pair) doesn't count; a J...F or
                // L...7 pair does.
                if (IsInLoop(x, y))
                {
                    ins++;
                    _chars[x, y] = 'I';
                }
                else
                {
                    outs++;
                    _chars[x, y] = 'O';
                }
            }

            return (ins, outs);
        }

        static bool IsInLoop(int x, int y)
        {
            bool isOut = true;
            char lastCorner = ' ';
            for (int xx = x; xx >= 0; xx--)
            {
                // Crossing the loop
                if (CharAt((xx, y)) == '-')
                    isOut = !isOut;
                // I'm not checking invalid situations like the first corner we see is 'F'
                else if (CharAt((xx, y)) == 'J' || CharAt((xx, y)) == 'L')
                {
                    if (lastCorner != ' ') throw new InvalidDataException("bad corner " + x + " " + y);
                    lastCorner = CharAt((xx, y));
                }
                else if (CharAt((xx, y)) == 'F')
                {
                    if (lastCorner == 'J') isOut = !isOut;
                    else if (lastCorner != 'L') throw new InvalidDataException("bad corner" + x + " " + y);
                    lastCorner = ' ';
                }
                else if (CharAt((xx, y)) == '7')
                {
                    if (lastCorner == 'L') isOut = !isOut;
                    else if (lastCorner != 'J') throw new InvalidDataException("bad corner" + x + " " + y);
                    lastCorner = ' ';
                }
            }

            return !isOut;
        }
    }
}
