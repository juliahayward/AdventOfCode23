using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day23Puzzle : PuzzleBase
    {
        // First stab at part 2 was simply to remove all the constraints, but that
        // blows up timewise.
        internal static void Do(bool example)
        {
            var grid = ReadLinesAsGrid(23, example);

            var start = new PathStep { x = 1, y = 0, distance = 0, previous = null };
            // If we skip to the second step, as we know it, the # will do all our bounds checks
            var second = new PathStep { x = 1, y = 1, distance = 1, previous = start };
            var unprocessedSteps = new List<PathStep>();
            var maxPathLength = 0;
            unprocessedSteps.Add(second);

            // I don;t think, if you arrive at a cell that's already been visited,
            // that you can discard the shorter option - it may turn out to be longer
            // ultimately using pieces of the longer option.
            while (unprocessedSteps.Any())
            {
                var thisStep = unprocessedSteps.First();
                // Try a step E. Must not be forest, must be downhill if we're on a slope,
                // and must not have been visited before on this path
                if (grid.Cells[thisStep.x, thisStep.y + 1] != '#'
                    && !"^><".Contains(grid.Cells[thisStep.x, thisStep.y + 1])
                    && !thisStep.HasPassedThrough(thisStep.x, thisStep.y + 1))
                {
                    var next = new PathStep()
                    {
                        x = thisStep.x,
                        y = thisStep.y + 1,
                        distance = thisStep.distance + 1,
                        previous = thisStep
                    };
                    if (next.y == grid.Height - 1)
                    {
                        if (next.distance > maxPathLength)
                            Console.WriteLine(next.distance);
                        maxPathLength = Math.Max(maxPathLength, next.distance);
                    }
                    else
                        unprocessedSteps.Add(next);
                }
                // Now try others - simpler in that we know we're not ending on this one
                if (grid.Cells[thisStep.x + 1, thisStep.y] != '#'
                    && !"v<^".Contains(grid.Cells[thisStep.x + 1, thisStep.y])
                    && !thisStep.HasPassedThrough(thisStep.x + 1, thisStep.y))
                {
                    var next = new PathStep()
                    {
                        x = thisStep.x + 1, y = thisStep.y,
                        distance = thisStep.distance + 1,
                        previous = thisStep
                    };
                    unprocessedSteps.Add(next);
                }
                if (grid.Cells[thisStep.x - 1, thisStep.y] != '#'
                    && !"v>^".Contains(grid.Cells[thisStep.x - 1, thisStep.y])
                    && !thisStep.HasPassedThrough(thisStep.x - 1, thisStep.y))
                {
                    var next = new PathStep()
                    {
                        x = thisStep.x - 1,
                        y = thisStep.y,
                        distance = thisStep.distance + 1,
                        previous = thisStep
                    };
                    unprocessedSteps.Add(next);
                }
                if (grid.Cells[thisStep.x, thisStep.y - 1] != '#'
                    && !"v><".Contains(grid.Cells[thisStep.x, thisStep.y - 1])
                    && !thisStep.HasPassedThrough(thisStep.x, thisStep.y - 1))
                {
                    var next = new PathStep()
                    {
                        x = thisStep.x,
                        y = thisStep.y - 1,
                        distance = thisStep.distance + 1,
                        previous = thisStep
                    };
                    unprocessedSteps.Add(next);
                }

                unprocessedSteps.RemoveAt(0);
            }

            Console.WriteLine(maxPathLength);
        }

        internal static void DoPart2(bool example)
        {
            var grid = ReadLinesAsGrid(23, example);
            var nodes = new Node[grid.Width, grid.Height];

            for (int i = 1; i < grid.Width - 1; i++)
            for (int j = 1; j < grid.Height - 1; j++)
            {
                if (grid.Cells[i, j] == '#') continue;

                nodes[i, j] = new Node() { x = i, y = j };
                if (nodes[i - 1, j] != null)
                {
                    nodes[i, j].neighbours.Add(nodes[i - 1, j], 1);
                    nodes[i - 1, j].neighbours.Add(nodes[i, j], 1);
                }

                if (nodes[i, j - 1] != null)
                {
                    nodes[i, j].neighbours.Add(nodes[i, j - 1], 1);
                    nodes[i, j - 1].neighbours.Add(nodes[i, j], 1);
                }
            }

            nodes[1, 0] = new Node() { x = 1, y = 0 };
            nodes[1, 0].neighbours.Add(nodes[1, 1], 1);
            nodes[1, 1].neighbours.Add(nodes[1, 0], 1);
            nodes[grid.Width - 2, grid.Height - 1] = new Node() { x = grid.Width - 2, y = grid.Height - 1 };
            nodes[grid.Width - 2, grid.Height - 1].neighbours
                .Add(nodes[grid.Width - 2, grid.Height - 2], 1);
            nodes[grid.Width - 2, grid.Height - 2].neighbours
                .Add(nodes[grid.Width - 2, grid.Height - 1], 1);

            // Now reduce to a network
            bool progress = true;
            while (progress)
            {
                progress = false;
                for (int i = 1; i < grid.Width - 1; i++)
                for (int j = 1; j < grid.Height - 1; j++)
                {
                    if (nodes[i, j] != null && nodes[i, j].neighbours.Count == 2)
                    {
                        var nweDistance = nodes[i, j].neighbours.Sum(x => x.Value);
                        var n1 = nodes[i, j].neighbours.First().Key;
                        var n2 = nodes[i, j].neighbours.Last().Key;
                        n1.neighbours.Remove(nodes[i, j]);
                        n2.neighbours.Remove(nodes[i, j]);
                        n1.neighbours.Add(n2, nweDistance);
                        n2.neighbours.Add(n1, nweDistance);
                            nodes[i,j].neighbours.Clear();
                        progress = true;
                    }
                }
            }

            var start = new PathStep { x = 1, y = 0, distance = 0, previous = null };
            var unprocessedSteps = new List<PathStep>() { start };
            var maxPathLength = 0;

            // I don;t think, if you arrive at a cell that's already been visited,
            // that you can discard the shorter option - it may turn out to be longer
            // ultimately using pieces of the longer option.
            while (unprocessedSteps.Any())
            {
                unprocessedSteps.Sort();
                var thisStep = unprocessedSteps.First();
                var thisNode = nodes[thisStep.x, thisStep.y];
                foreach (var nextNode in thisNode.neighbours.Keys)
                {
                    var nextStep = new PathStep()
                    {
                        x = nextNode.x,
                        y = nextNode.y,
                        distance = thisStep.distance + thisNode.neighbours[nextNode],
                        previous = thisStep
                    };

                    // Already visited that node
                    if (thisStep.HasPassedThrough(nextStep.x, nextStep.y))
                        continue;

                    if (nextStep.y == grid.Height - 1) // end
                    {
                        if (nextStep.distance > maxPathLength)
                            Console.WriteLine(nextStep.distance);
                        maxPathLength = Math.Max(maxPathLength, nextStep.distance);
                    }
                    else
                        unprocessedSteps.Add(nextStep);
                }

                unprocessedSteps.RemoveAt(0);
            }
        }
    }

    internal class Node
    {
        public int x, y;
        public Dictionary<Node, int> neighbours = new Dictionary<Node, int>();
        public override string ToString() => $"({x},{y})";
    }

    internal class PathStep : IComparable
    {
        public int x, y, distance;
        public PathStep previous;

        // Do the longest path first as this will find our max quicker
        public int CompareTo(object obj)
        {
            return - distance.CompareTo((obj as PathStep).distance);
        }

        public bool HasPassedThrough(int x, int y)
        {
            var step = this;
            while (step != null)
            {
                if (step.x == x && step.y == y)
                    return true;
                step = step.previous;
            }

            return false;
        }
    }
}
