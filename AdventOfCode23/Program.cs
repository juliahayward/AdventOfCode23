﻿using System;
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

            
            Day6Puzzle.DoPart2(false);
            
            Console.WriteLine("Elapsed time: " + (DateTime.UtcNow - start).TotalSeconds);

            Console.ReadLine();
        }
    }
}
