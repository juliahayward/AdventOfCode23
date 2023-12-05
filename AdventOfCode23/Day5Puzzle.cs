using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day5Puzzle : PuzzleBase
    {
        internal static void DoPart1(bool example)
        {
            var lines = ReadLines(5, example);
            var seeds = ReadSeeds(lines);
            var maps = new[]
            {
                ReadMap(lines, "seed-to-soil map"),
                ReadMap(lines, "soil-to-fertilizer map"),
                ReadMap(lines, "fertilizer-to-water map"),
                ReadMap(lines, "water-to-light map"),
                ReadMap(lines, "light-to-temperature map"),
                ReadMap(lines, "temperature-to-humidity map"),
                ReadMap(lines, "humidity-to-location map")
            };

            var locations = seeds.Select(s => DoAllMappings(s, maps));

            Console.WriteLine(locations.Min());
        }

        internal static void DoPart2(bool example)
        {
            var lines = ReadLines(5, example);
            var seedRanges = ReadSeedRanges(lines).ToList();
            var maps = new[]
            {
                ReadMap(lines, "seed-to-soil map"),
                ReadMap(lines, "soil-to-fertilizer map"),
                ReadMap(lines, "fertilizer-to-water map"),
                ReadMap(lines, "water-to-light map"),
                ReadMap(lines, "light-to-temperature map"),
                ReadMap(lines, "temperature-to-humidity map"),
                ReadMap(lines, "humidity-to-location map")
            };

            foreach (var map in maps)
                seedRanges = MapRanges(seedRanges, map).ToList();

            Console.WriteLine(seedRanges.Min(x => x.Item1));
        }

        // Part 1 is pretty easy to brute force!
        static IEnumerable<long> ReadSeeds(List<string> lines)
        {
            return lines[0].Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1) // "seeds"
                .Select(x => long.Parse(x))
                .ToList();
        }

        // For part 2 - the first trap is trying to materialise a list of 10^8 elements :)
        // Even if you avoid that, though, you still have a humungous search ahead of you.
        //
        // So, to sidestep this, note that each map is piecewise linear. So what I'm going
        // to try to do is start with a list of ranges of seed numbers; without evaluating
        // each seed explicitly, I'll map across ranges 

        static IEnumerable<(long, long)> ReadSeedRanges(List<string> lines)
        {
            var values = lines[0].Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1) // "seeds"
                .Select(x => long.Parse(x))
                .ToList();
            for (int i = 0; i < values.Count; i += 2)
            {
                Console.WriteLine("Seed range " + values[i] + " " + (values[i] + values[i + 1] - 1));
                yield return (values[i], values[i] + values[i + 1] - 1);
            }
        }

        static IEnumerable<(long, long, long)> ReadMap(List<string> lines, string mapTitle)
        {
            bool inCorrectMap = false;
            foreach (var line in lines)
            {
                if (!inCorrectMap)
                {
                    if (line.Contains(mapTitle))
                        inCorrectMap = true;
                    continue;
                }
                if (string.IsNullOrWhiteSpace(line))
                    break;
                var parts = line.Split(' ')
                    .Select(x => long.Parse(x))
                    .ToArray();
                yield return (parts[0], parts[1], parts[2]);
            }
        }

        static long DoAllMappings(long s, IEnumerable<(long, long, long)>[] maps)
        {
            foreach (var map in maps)
            {
                s = DoMapping(s, map);
            }
            return s;
        }

        static long DoMapping(long s, IEnumerable<(long, long, long)> map)
        {
            foreach (var element in map)
            {
                if (s >= element.Item2 && s < element.Item2 + element.Item3)
                    return (s - element.Item2) + element.Item1;
            }

            return s;
        }

        static IEnumerable<(long, long)> MapRanges(List<(long, long)> ranges, IEnumerable<(long, long, long)> map)
        {
            // First of all, split ranges so that each is handled by exactly
            // zero or one lines of the map

            // Each map element splits each range at most once - toss the pieces on to a list
            // and recycle it as multiple elements may split a range
            foreach (var element in map)
            {
                var splitRanges = new List<(long, long)>();
                foreach (var range in ranges)
                {
                    var elementBottom = element.Item2;
                    var elementTop = element.Item2 + element.Item3 - 1; // inclusive
                    var overlap = (elementBottom <= range.Item2 && elementTop >= range.Item1);
                    if (!overlap)
                    {
                        splitRanges.Add((range.Item1, range.Item2));
                        continue;
                    }
                    // bottom unaffected region
                    if (elementBottom > range.Item1)
                        splitRanges.Add((range.Item1, elementBottom - 1));
                    // top unaffected region
                    if (elementTop < range.Item2)
                        splitRanges.Add((elementTop + 1, range.Item2));
                    // middle affected region
                    splitRanges.Add((Math.Max(elementBottom, range.Item1),
                            Math.Min(elementTop, range.Item2)));
                }

                ranges = splitRanges;
            }
            // Now map the ranges. Each range should be affected by at most one map element.
            foreach (var range in ranges)
            {
                bool wasMapped = false;
                foreach (var element in map)
                {
                    var elementBottom = element.Item2;
                    var elementTop = element.Item2 + element.Item3 - 1; // inclusive
                    if (elementBottom <= range.Item1 && elementTop >= range.Item2)
                    {
                        wasMapped = true;
                        yield return (range.Item1 + element.Item1 - element.Item2,
                            range.Item2 + element.Item1 - element.Item2);
                    }
                }
                if (!wasMapped)
                    yield return range;
            }
        }


        // yes, maybe add nunit when I'm not trying to code on a train that's
        // full and I'm standing :)
        internal static void TestRanges()
        {
            var ranges = new List<(long, long)>() { (100, 200) };
            var map = new List<(long, long, long)>() { (0, 50, 20) };
            var splitRanges = MapRanges(ranges, map);
            if (splitRanges.Count() != 1 || splitRanges.First() != (100, 200))
                throw new Exception("fail");
            map = new List<(long, long, long)>() { (0, 50, 100) };
            splitRanges = MapRanges(ranges, map);
            if (splitRanges.Count() != 2 || !splitRanges.Contains((100, 149)) || !splitRanges.Contains((150, 200)))
                throw new Exception("fail");
            map = new List<(long, long, long)>() { (0, 150, 100) };
            splitRanges = MapRanges(ranges, map);
            if (splitRanges.Count() != 2 || !splitRanges.Contains((100, 149)) || !splitRanges.Contains((150, 200)))
                throw new Exception("fail");
            map = new List<(long, long, long)>() { (0, 150, 20) };
            splitRanges = MapRanges(ranges, map);
            if (splitRanges.Count() != 3 || !splitRanges.Contains((100, 149)) || !splitRanges.Contains((150, 169)) || !splitRanges.Contains((170, 200)))
                throw new Exception("fail");
        }
    }
}
