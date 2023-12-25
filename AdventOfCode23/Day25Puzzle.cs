using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day25Puzzle : PuzzleBase
    {
        static Dictionary<string, Component> _components = new Dictionary<string, Component>();
        static List<Edge> _edges = new List<Edge>();

        // Part one is amenable to brute force. Part 2 won't be...
        internal static void DoPart1(bool example)
        {
            var lines = ReadLines(25, example);

            foreach (var line in lines)
            {
                var pieces = line.Split(new [] {' ', ':'}, StringSplitOptions.RemoveEmptyEntries);
                if (!_components.ContainsKey(pieces[0]))
                    _components.Add(pieces[0], new Component { name = pieces[0] });
                for (int i = 1; i < pieces.Length; i++)
                {
                    if (!_components.ContainsKey(pieces[i]))
                        _components.Add(pieces[i], new Component { name = pieces[i] });

                    var edge = new Edge { start = pieces[0], end = pieces[i] };
                    _edges.Add(edge);
                    _components[pieces[0]].edges.Add(pieces[i], edge);
                    _components[pieces[i]].edges.Add(pieces[0], edge);
                }
            }
            // Try deactivating three edges. Then pick a node, and see if you can get to
            // all others. If not, then all the ones you can get to make up a connected set
            // and the rest make up the other.
            var numberOfComponents = _components.Count;

            for (int i = 0; i < _edges.Count; i++)
            for (int j = i + 1; j < _edges.Count; j++)
            for (int k = j + 1; k < _edges.Count; k++)
            {
                _edges[i].isActive = _edges[j].isActive = _edges[k].isActive = false;
                var start = _components.First().Key;
                int count = CountComponentsConnectedTo(start);
                if (count < numberOfComponents)
                {
                    Console.WriteLine(count * (numberOfComponents - count));
                }
                _edges[i].isActive = _edges[j].isActive = _edges[k].isActive = true;
            }
        }

        // Use Girvan-Newman to find the edge with highest path-betweenness
        internal static void DoPart2(bool example)
        {
            var lines = ReadLines(25, example);

            foreach (var line in lines)
            {
                var pieces = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (!_components.ContainsKey(pieces[0]))
                    _components.Add(pieces[0], new Component { name = pieces[0] });
                for (int i = 1; i < pieces.Length; i++)
                {
                    if (!_components.ContainsKey(pieces[i]))
                        _components.Add(pieces[i], new Component { name = pieces[i] });

                    var edge = new Edge { start = pieces[0], end = pieces[i] };
                    _edges.Add(edge);
                    _components[pieces[0]].edges.Add(pieces[i], edge);
                    _components[pieces[i]].edges.Add(pieces[0], edge);
                }
            }

            // We will iterate cutting the "hottest" (most between) edge until we find we
            // don't come across at least one node in our shortest-distance calculation.
            while (true)
            {
                foreach (var start in _components.Keys)
                {
                    // Find all the shortest-paths to each other node.
                    var nodesToVisit = new List<(string, string)>() { (start, start) };
                    var shortestDistances = new Dictionary<string, int>();
                    var shortestRoutes = new Dictionary<string, List<string>>();

                    shortestDistances[start] = 0;
                    shortestRoutes[start] = new List<string>() { start };
                    while (nodesToVisit.Any())
                    {
                        var currentNode = nodesToVisit.First();
                        foreach (var edge in _components[currentNode.Item1]
                                     .edges.Where(x => x.Value.isActive))
                        {
                            var node = (edge.Value.start == currentNode.Item1)
                                ? edge.Value.end
                                : edge.Value.start;
                            if (!shortestDistances.ContainsKey(node))
                            {
                                shortestDistances.Add(node, shortestDistances[currentNode.Item1] + 1);
                                shortestRoutes.Add(node,
                                    new List<string> { currentNode.Item2 + "-" + node });
                                nodesToVisit.Add((node, currentNode.Item2 + "-" + node));
                            }
                            else if (shortestDistances[node] == shortestDistances[currentNode.Item1] + 1)
                            {
                                shortestRoutes[node].Add(currentNode.Item2 + "-" + node);
                            }
                        }

                        nodesToVisit.Remove(currentNode);
                    }

                    if (shortestDistances.Count() < _components.Count)
                    {
                        Console.WriteLine("Two groups, one of size " + shortestDistances.Count());
                        Console.WriteLine(shortestRoutes.Count() * (_components.Count - shortestDistances.Count()));
                        return;
                    }

                    // Add in all the contributions to edge-betweenness
                    foreach (var routesToOtherNode in shortestRoutes)
                    {
                        // If there are multiple shortest routes to the target, each one
                        // counts as 1/n to the betweenness
                        var weight = 1.0 / routesToOtherNode.Value.Count();
                        foreach (var route in routesToOtherNode.Value)
                        {
                            var unprocessedRoute = route;
                            while (unprocessedRoute.Length >= 7)
                            {
                                var edgeStart = unprocessedRoute.Substring(0, 3);
                                var edgeEnd = unprocessedRoute.Substring(4, 3);
                                _components[edgeStart].edges[edgeEnd].edgeBetweenness += weight;
                                unprocessedRoute = unprocessedRoute.Substring(4);
                            }
                        }
                    }
                }

                var hottestEdge = _edges.OrderByDescending(x => x.edgeBetweenness).First();
                Console.WriteLine(hottestEdge.start + "-" + hottestEdge.end + " betweenness " +
                                  hottestEdge.edgeBetweenness);

                // Cut this edge, reset everything and try again. Note that the second most
                // between edge is not necessarily the second one to cut - lots of paths will
                // reconfigure themselves
                _edges.ForEach(x => x.edgeBetweenness = 0);
                hottestEdge.isActive = false;
            }
        }

        private static int CountComponentsConnectedTo(string start)
        {
            foreach (var c in _components.Values)
            {
                c.visited = false;
            }
            VisitAllConnected(start);
            return _components.Values.Count(x => x.visited);
        }

        private static void VisitAllConnected(string start)
        {
            _components[start].visited = true;
            foreach (var edge in _components[start].edges.Where(x => x.Value.isActive))
            {
                var otherNode = edge.Key;
                if (!_components[otherNode].visited)
                    VisitAllConnected(otherNode);
            }
        }
    }

    internal class Component
    {
        public string name;
        // Destination component, edge
        public Dictionary<string, Edge> edges = new Dictionary<string, Edge>();
        public bool visited = false;
    }
    internal class Edge
    {
        public string start, end;
        public bool isActive = true;
        public double edgeBetweenness = 0;
    }
}
