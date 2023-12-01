using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace AdventOfCode23
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.UtcNow;

            Day1Puzzle.DoPartTwo(false);
            
            Console.WriteLine("Elapsed time: " + (DateTime.UtcNow - start).TotalSeconds);

            Console.ReadLine();
        }
    }
}
