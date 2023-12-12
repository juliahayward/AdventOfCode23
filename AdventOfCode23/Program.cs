using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
namespace AdventOfCode23
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.UtcNow;

            // currently 0.26s for example, 21 cursor-flashes for first of real
            Day12Puzzle.Do(true, true);

            Console.WriteLine("Elapsed time: " + (DateTime.UtcNow - start).TotalSeconds);

            Console.ReadLine();
        }
    }
}
