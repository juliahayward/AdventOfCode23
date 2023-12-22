using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace AdventOfCode23
{
    internal class Day22Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var lines = ReadLines(22, example);
            var blocks = new List<Block>();
            char name = 'A';
            foreach (var line in lines)
            {
                var pieces = line.Split(',', '~');
                blocks.Add(new Block() { xmin = int.Parse(pieces[0]),
                    ymin = int.Parse(pieces[1]),
                    zmin = int.Parse(pieces[2]),
                    xmax = int.Parse(pieces[3]),
                    ymax = int.Parse(pieces[4]),
                    zmax = int.Parse(pieces[5]),
                    name = name.ToString()
                });
                name++;
            }

            foreach (var block in blocks)
            {
                block.IsSupportedBy = blocks.Where(b => b.GetSupports(block)).ToList();
                block.Supports = blocks.Where(b => block.GetSupports(b)).ToList();
            }

            // Now let them fall to their resting place
            while (true)
            {
                var faller = blocks.FirstOrDefault(x => !x.IsOnGround && !x.IsSupportedBy.Any());
                if (faller == null)
                    break;

                faller.zmin -= 1;
                faller.zmax -= 1;
                // No longer supporting those above...
                foreach (var higher in faller.Supports)
                {
                    higher.IsSupportedBy.Remove(faller);
                }
                faller.Supports.Clear();
                // Might be supported by new ones below
                foreach (var lower in blocks.Where(x => x.zmax == faller.zmin - 1))
                {
                    if (lower.GetSupports(faller))
                    {
                        lower.Supports.Add(faller);
                        faller.IsSupportedBy.Add(lower);
                    }
                }
            }

            var blocksThatAreAUniqueSupport = new List<Block>();
            foreach (var block in blocks)
            {
                if (block.IsSupportedBy.Count == 1)
                    blocksThatAreAUniqueSupport.Add(block.IsSupportedBy.First());
            }
            Console.WriteLine("Total blocks: " + blocks.Count());
            Console.WriteLine("Can't be disintegrated:" + blocksThatAreAUniqueSupport.Distinct().Count());
            Console.WriteLine("Can be disintegrated:" + (blocks.Count() - blocksThatAreAUniqueSupport.Distinct().Count()));

            var count = 0;
            foreach (var startingBlock in blocks)
            {
                blocks.ForEach(x => x.NoLongerASupport = false);
                CheckStillSupported(startingBlock);
                var countAffected = blocks.Count(x => x.NoLongerASupport) - 1;
                count += countAffected;
            }

            Console.WriteLine(count);

            void CheckStillSupported(Block b)
            {
                b.NoLongerASupport = true;
                foreach (var higher in b.Supports)
                {
                    if (higher.IsSupportedBy.All(x => x.NoLongerASupport))
                        CheckStillSupported(higher);
                }
            }
        }
    }

    internal class Block
    {
        public int xmin, xmax, ymin, ymax, zmin, zmax;
        public string name;

        public bool IsOnGround => zmin == 1;

        public List<Block> IsSupportedBy = new List<Block>();

        public List<Block> Supports = new List<Block>();

        public bool GetSupports(Block other)
        {
            return zmax == other.zmin - 1 
                   && (xmax >= other.xmin && other.xmax >= xmin)
                   && (ymax >= other.ymin && other.ymax >= ymin);
        }

        public bool NoLongerASupport = false;


    }
}
