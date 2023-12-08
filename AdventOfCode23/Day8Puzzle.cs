using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day8Puzzle : PuzzleBase
    {
        internal static void DoPartOne(bool example)
        {
            var lefts = new Dictionary<string, string>();
            var rights = new Dictionary<string, string>();

            var lines = ReadLines(8, example);
            var directions = lines[0];

            string startNode = "AAA", targetNode = "ZZZ";

            foreach (var line in lines.Skip(2))
            {
                var parts = line.Split(new [] { ' ', '=', ')', '(', ',' },
                    StringSplitOptions.RemoveEmptyEntries);

                lefts.Add(parts[0], parts[1]);
                rights.Add(parts[0], parts[2]);
            }

            var currentNode = startNode;
            var steps = 0;
            while (currentNode != targetNode)
            {
                var direction = directions[0];
                currentNode = (direction == 'L')
                    ? lefts[currentNode]
                    : rights[currentNode];
                steps++;
                directions = directions.Substring(1) + direction;
            }

            Console.WriteLine(steps);
        }

        internal static void DoPartTwo(bool example)
        {
            var lefts = new Dictionary<string, string>();
            var rights = new Dictionary<string, string>();

            var lines = ReadLines(8, example);
            var initialDirections = lines[0];

            char startNodeEnding = 'A', targetNodeEnding = 'Z';

            foreach (var line in lines.Skip(2))
            {
                var parts = line.Split(new[] { ' ', '=', ')', '(', ',' },
                    StringSplitOptions.RemoveEmptyEntries);

                lefts.Add(parts[0], parts[1]);
                rights.Add(parts[0], parts[2]);
            }

            Console.WriteLine("Number of directions: " + initialDirections.Length);
            Console.WriteLine("Number of nodes: " + (lines.Count - 2));
            // The path from any starting node must return to the same state in at most
            // (#nodes * #directions) steps. However, that doesn't ensure that the
            // whole ensemble repeats in the same time - as they may enter cycles
            // of different (coprime) lengths. So we have (#directions * (#nodes ^ #startingNodes))
            // states to check ... on my data that's abour 5.1E19
            var currentNodes = lefts.Keys.Where(x => x[2] == startNodeEnding);
            Console.WriteLine("Number of starting nodes: " + currentNodes.Count());

            var simpleEnds = new List<int>();
            var expressions = new List<(int, int)>();
            foreach (var startingNode in currentNodes)
            {
                var currentNode = startingNode;
                var directions = initialDirections;
                var seenStates = new List<(string, string)>();
                while (!seenStates.Contains((currentNode, directions)))
                {
                    seenStates.Add((currentNode, directions));
                    var direction = directions[0];

                    currentNode = (direction == 'L')
                        ? lefts[currentNode]
                        : rights[currentNode];
                    directions = directions.Substring(1) + direction;
                }
                Console.WriteLine($"Starting node {startingNode} repeats after {seenStates.Count} steps");
                var firstRepeatedNodeIndex = seenStates.IndexOf((currentNode, directions));
                Console.WriteLine($"repeated node {firstRepeatedNodeIndex}");
                // So our states go A-Z1-B-C-D-E-Z2-F-G-H-C-D-E-Z2-..., say. This gives us
                //    seen states S = 10
                //    repeated state R = 3
                //    simple end Z1 = 1
                //    repeated end Z2 = 6, 13, 20, ... = 7n+6
                for (int i = 0; i < seenStates.Count; i++)
                {
                    if (seenStates.ElementAt(i).Item1[2] == targetNodeEnding)
                    {
                        if (i < firstRepeatedNodeIndex)
                            simpleEnds.Add(i);
                        else
                            expressions.Add((seenStates.Count - firstRepeatedNodeIndex, i));
                    }
                }
            }
            // I've noted that each start node goes into a cycle that only
            // contains one end node - things would be more complex still if not
            // So we need to find the lowest N such that for each (a, b) in
            // expressions, N = b mod a.
            foreach (var pair in expressions)
                Console.WriteLine(pair);

            // Hey... I haven't solved the general case, which looks pretty
            // nasty, but noticed that *all* six expressions are N = 0 mod a.
            // So, that should mean the answer is the LCM of the a's, right?
            Console.WriteLine(LCM(expressions.Select(x => (long)x.Item1).ToList()));

        }
    }
}
