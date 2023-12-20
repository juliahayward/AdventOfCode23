using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static AdventOfCode23.PuzzleBase;

namespace AdventOfCode23
{
    internal class Day17Puzzle : PuzzleBase
    {
        static Dictionary<string, int> _cache = new Dictionary<string, int>();
        internal static void Do(bool example, bool part2)
        {
            int minStepsBeforeTurn = (part2) ? 4 : 1;
            int maxStepsBeforeTurn = (part2) ? 10 : 3;

            var grid = ReadLinesAsGrid(17, example);
            // Note that the optimal path may loop back on itself given
            // length constraints. The state we're in includes the (x,y) we're
            // at, the direction we're facing, and the number of steps we have
            // already taken in that direction.
            // We'll build up paths from near the goal so that we can do plenty of cacheing
            var states = new Dictionary<string, CrucibleState>();
            
            for (int i = 0; i < grid.Width; i++)
                for (int j=0; j < grid.Height; j++)
                    for (int s = 1; s <= maxStepsBeforeTurn; s++)
                        foreach (char f in "NSEW")
                        {
                            var state = new CrucibleState() { x = i, y = j, facing = f, stepsAlreadyTaken = s };
                            states.Add(state.key, state);
                            // Terminal states - to end, in SE corner AND able to stop there!
                            if (i == grid.Width - 1 && j == grid.Height - 1 && s >= minStepsBeforeTurn)
                                state.heatLossToEnd = 0;
                            if (i == 0 && j == 0 && "NW".Contains(f)) // We're not starting part-way through a S or E movement
                                state.isOrigin = true;
                        }



            // Don't forget you can't double-back to get around the 3-step limit!
            foreach (var state in states.Values)
            {
                // Check that we're not heading in a different direction and unable to turn yet
                if (!(state.facing != 'E' && state.stepsAlreadyTaken < minStepsBeforeTurn))
                {
                    var eastKey =
                        $"{state.x + 1},{state.y},{'E'},{(state.facing == 'E' ? state.stepsAlreadyTaken + 1 : 1)}";

                    if (states.ContainsKey(eastKey) && state.facing != 'W')
                    {
                     //   state.successors.Add(states[eastKey]);
                        states[eastKey].predecessors.Add(state);
                    }
                }

                if (!(state.facing != 'S' && state.stepsAlreadyTaken < minStepsBeforeTurn))
                {
                    var southKey =
                        $"{state.x},{state.y + 1},{'S'},{(state.facing == 'S' ? state.stepsAlreadyTaken + 1 : 1)}";
                    if (states.ContainsKey(southKey) && state.facing != 'N')
                    {
                      //  state.successors.Add(states[southKey]);
                        states[southKey].predecessors.Add(state);
                    }
                }

                if (!(state.facing != 'W' && state.stepsAlreadyTaken < minStepsBeforeTurn))
                {
                    var westKey =
                        $"{state.x - 1},{state.y},{'W'},{(state.facing == 'W' ? state.stepsAlreadyTaken + 1 : 1)}";
                    if (states.ContainsKey(westKey) && state.facing != 'E')
                    {
                       // state.successors.Add(states[westKey]);
                        states[westKey].predecessors.Add(state);
                    }
                }

                if (!(state.facing != 'N' && state.stepsAlreadyTaken < minStepsBeforeTurn))
                {
                    var northKey =
                        $"{state.x},{state.y - 1},{'N'},{(state.facing == 'N' ? state.stepsAlreadyTaken + 1 : 1)}";
                    if (states.ContainsKey(northKey) && state.facing != 'S')
                    {
                      //  state.successors.Add(states[northKey]);
                        states[northKey].predecessors.Add(state);
                    }
                }
            }

            // TODO: We could recursively purge out all states that aren't origins and have no
            // predecessors, so that the next part doesn't put dead-ends in liveStates and 
            // bulk out the sorting so much

            // These are all our destinations
            var liveStates = states.Values
                .Where(s => s.heatLossToEnd.HasValue)
                .ToList();

            int statesVisited = 0;
            while (true)
            {
                var current = liveStates
                    .OrderBy(s => s.heatLossToEnd)
                    .FirstOrDefault();
                if (current == null) // All visited
                    break;
                if (current.isOrigin) // We're at (0,0), no better route possible
                    break;

                var valueThisPath = current.heatLossToEnd.Value + (grid.Cells[current.x, current.y] - '0');

                foreach (var predecessor in current.predecessors)
                {
                    if (predecessor.heatLossToEnd.HasValue)
                    {
                        //if (predecessor.heatLossToEnd > valueThisPath)
                        //    predecessor.bestRoute = current.facing + current.bestRoute;
                        predecessor.heatLossToEnd = Math.Min(predecessor.heatLossToEnd.Value,
                            valueThisPath);
                    }
                    else
                    {
                        predecessor.heatLossToEnd = valueThisPath;
                        //predecessor.bestRoute = current.facing + current.bestRoute;
                        liveStates.Add(predecessor);
                    }
                }

                current.visited = true;
                statesVisited++;
                if (statesVisited % 1000 == 0) Console.WriteLine(statesVisited);
                liveStates.Remove(current);
            }

            // Several options, but this one allows S and W movements by up to 3
            var bestFromStart = states.Values.Where(x => x.isOrigin && x.heatLossToEnd.HasValue)
                .OrderBy(x => x.heatLossToEnd)
                .First();
                

            Console.WriteLine(bestFromStart.heatLossToEnd.Value);
            Console.WriteLine(bestFromStart.bestRoute);
        }


        internal class CrucibleState
        {
            internal string key => $"{x},{y},{facing},{stepsAlreadyTaken}";
            internal int x, y;
            internal char facing;
            internal int stepsAlreadyTaken;
            internal int? heatLossToEnd;
            internal bool visited = false;
            internal bool isOrigin = false;
            internal string bestRoute = "";
            internal List<CrucibleState> successors = new List<CrucibleState>();
            internal List<CrucibleState> predecessors = new List<CrucibleState>();
        }

        
    }
}
