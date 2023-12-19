using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day19Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var lines = ReadLines(19, example);
            var workflows = new Dictionary<string, List<Rule>>();
            var parts = new List<Part>();
            bool finishedWorkflows = false;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    finishedWorkflows = true;
                }
                else if (!finishedWorkflows)
                {
                    var workflow = ParseWorkflow(line);
                    workflows.Add(workflow.Item1, workflow.Item2);
                }
                else
                {
                    parts.Add(ParsePart(line));
                }
            }

            while (parts.Any(x => x.InProgress))
            {
                var firstPart = parts.First(x => x.InProgress);
                
                firstPart.currentWorkflow = ProcessPart(firstPart,
                    workflows[firstPart.currentWorkflow]);
            }

            // This sums up the pre-given list...
            Console.WriteLine(parts.Where(x => x.Accepted)
                .Sum(x => x.TotalRating));

            // For part 2, the number of combinations is going to be ludicrously high.
            // However, we could search through all the rules, find the values at which
            // the part behaviour changes, and then process blocks of parts by picking
            // a representative one.
            var criticalValues = new Dictionary<char, List<int>>();
            criticalValues['x'] = new List<int>() { 1 };
            criticalValues['m'] = new List<int>() { 1 };
            criticalValues['a'] = new List<int>() { 1 };
            criticalValues['s'] = new List<int>() { 1 };

            foreach (var workflow in workflows)
            {
                foreach (var rule in workflow.Value)
                {
                    switch (rule.comparisonType)
                    {
                        case (char)0: continue;
                        case '<':
                            criticalValues[rule.partProperty].Add(rule.comparisonValue + 1);
                            break;
                        case '>':
                            criticalValues[rule.partProperty].Add(rule.comparisonValue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("bad rule");
                    }
                }
            }
            // So, we have 293 * 273 * 288 * 308 representative cases - a few
            // orders of magnitude less than (4000)^4 but still at 10^10 too many to
            // iterate through. 

            // Next idea - start with a PartBlock of size (4000, 4000, 4000, 4000).
            // Pass it through the workflow graph, and at each step instead of
            // accepting or rejecting it, cut that block into pieces based on the
            // conditions it hits. Then we should end up with a set of pieces each
            // of which behaves the same way for all the parts within.

            var partBlocks = new List<PartBlock>()
            {
                new PartBlock()
                    { xmin = 1, xmax = 4000, mmin = 1, mmax = 4000, amin = 1, amax = 4000, smin = 1, smax = 4000 }
            };
            while (partBlocks.Any(x => x.InProgress))
            {
                var firstBlock = partBlocks.First(x => x.InProgress);
                // This block will be altered in place, any new
                // children can be added
                partBlocks.AddRange(ProcessPartBlock(firstBlock,
                    workflows[firstBlock.currentWorkflow]));

                // checksum - should be 4000^4
                Console.WriteLine(partBlocks.Sum(x => x.Size));
            }

            // Example - expect 167409079868000
            // Real - expect 138616621185978 if you ever have time to refactor!
            Console.WriteLine(partBlocks.Where(x => x.Accepted)
                .Sum(x => x.Size));
        }

        internal static (string, List<Rule>) ParseWorkflow(string line)
        {
            // eg: px{ a < 2006:qkq,m > 2090:A,rfg}
            var pieces = line.Split(new [] {'{', '}', ',', ':'},
                StringSplitOptions.RemoveEmptyEntries);
            var rules = new List<Rule>();
            var name = pieces[0];
            for (int i = 1; i < pieces.Length - 2; i+= 2)
            {
                 rules.Add(new Rule()
                 {
                     partProperty = pieces[i][0],
                     comparisonType = pieces[i][1],
                     comparisonValue = int.Parse(pieces[i].Substring(2)),
                     destination = pieces[i+1]
                 });
            }
            rules.Add(new Rule()
            {
                destination = pieces[pieces.Length - 1]
            });
            return (name, rules);
        }

        internal static Part ParsePart(string line)
        {
            var pieces = line.Split(new [] {'{', ',', '}'},
                StringSplitOptions.RemoveEmptyEntries);
            // Assume they are all x=, m=, ...
            return new Part()
            {
                x = int.Parse(pieces[0].Replace("x=", "")),
                m = int.Parse(pieces[1].Replace("m=", "")),
                a = int.Parse(pieces[2].Replace("a=", "")),
                s = int.Parse(pieces[3].Replace("s=", ""))
            };
        }

        internal static string ProcessPart(Part part, List<Rule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.SatisfiedBy(part)) return rule.destination;
            }

            throw new InvalidDataException("No rule satisfied");
        }

        internal static IEnumerable<PartBlock> ProcessPartBlock(PartBlock block,
            List<Rule> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.comparisonType == 0)
                {
                    block.currentWorkflow = rule.destination;
                }
                if (rule.partProperty == 'x' && rule.comparisonType == '<')
                {
                    if (block.xmin < rule.comparisonValue && block.xmax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.xmax = rule.comparisonValue - 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.xmin = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.xmax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 'x' && rule.comparisonType == '>')
                {
                    if (block.xmin < rule.comparisonValue && block.xmax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.xmin = rule.comparisonValue + 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.xmax = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.xmax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 'm' && rule.comparisonType == '<')
                {
                    if (block.mmin < rule.comparisonValue && block.mmax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.mmax = rule.comparisonValue - 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.mmin = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.mmax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 'm' && rule.comparisonType == '>')
                {
                    if (block.mmin < rule.comparisonValue && block.mmax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.mmin = rule.comparisonValue + 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.mmax = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.mmax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 'a' && rule.comparisonType == '<')
                {
                    if (block.amin < rule.comparisonValue && block.amax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.amax = rule.comparisonValue - 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.amin = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.amax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 'a' && rule.comparisonType == '>')
                {
                    if (block.amin < rule.comparisonValue && block.amax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.amin = rule.comparisonValue + 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.amax = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.amax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 's' && rule.comparisonType == '<')
                {
                    if (block.smin < rule.comparisonValue && block.smax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.smax = rule.comparisonValue - 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.smin = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.smax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
                if (rule.partProperty == 's' && rule.comparisonType == '>')
                {
                    if (block.smin < rule.comparisonValue && block.smax > rule.comparisonValue)
                    {
                        // This rule splits the block
                        var carvedOffPart = block.Clone();
                        carvedOffPart.smin = rule.comparisonValue + 1;
                        carvedOffPart.currentWorkflow = rule.destination;
                        block.smax = rule.comparisonValue;
                        yield return carvedOffPart;
                    }
                    else if (block.smax <= rule.comparisonValue)
                    {
                        // Here it applies to the whole block
                        block.currentWorkflow = rule.destination;
                    }
                }
            }
        }
    }

    internal class Rule
    {
        public char partProperty;
        public int comparisonValue;
        public char comparisonType;
        public string destination;

        public bool SatisfiedBy(Part part)
        {
            // default case
            if (comparisonType == 0)
                return true;

            var relevantPartValue = part.GetProperty(partProperty);
            switch (comparisonType)
            {
                case '<': return relevantPartValue < comparisonValue;
                case '>': return relevantPartValue > comparisonValue;
                case '=': return relevantPartValue == comparisonValue;
                default: throw new ArgumentOutOfRangeException("Invalid operator " + comparisonType);
            }
        }
    }

    internal class Part
    {
        public int x, m, a, s;
        public string currentWorkflow = "in";

        public bool Accepted => currentWorkflow == "A";
        public bool Rejected => currentWorkflow == "R";
        public bool InProgress => !Accepted && !Rejected;

        public int TotalRating => x + m + a + s;

        public int GetProperty(char property)
        {
            switch (property)
            {
                case 'x': return x;
                case 'm': return m;
                case 'a': return a;
                case 's': return s;
                default: throw new ArgumentOutOfRangeException("unknown property " + property);
            }
        }
    }

    internal class PartBlock
    {
        internal int xmin, xmax, mmin, mmax, amin, amax, smin, smax;
        internal string currentWorkflow = "in";

        public bool Accepted => currentWorkflow == "A";
        public bool Rejected => currentWorkflow == "R";
        public bool InProgress => !Accepted && !Rejected;

        public long Size => (long)(xmax - xmin + 1) * (long)(mmax - mmin + 1) * (long)(amax - amin + 1) * (long)(smax - smin + 1);

        public PartBlock Clone()
        {
            return new PartBlock()
            {
                xmin = xmin, xmax = xmax, mmin = mmin, mmax = mmax,
                amin = amin, amax = amax, smin = smin, smax = smax,
                currentWorkflow = currentWorkflow
            };
        }
    }
}
