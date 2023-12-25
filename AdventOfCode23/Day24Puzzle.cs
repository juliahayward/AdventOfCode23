using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day24Puzzle : PuzzleBase
    {
        internal static void Do(bool example)
        {
            var lines = ReadLines(24, example);
            var hailstones = lines.Select(x => Hailstone.Parse(x)).ToList();

            // Area of interest. I'm going to work with BigInteger because I'm
            // firstly going to avoid loss of precision with division, and secondly 
            // avoid overflow with longs, because I'm sure the input data will have some!
            BigInteger aoi_Low = (example) ? 7 : 200000000000000;
            BigInteger aoi_High = (example) ? 27 : 400000000000000;

            int collisions = 0;

            for (int i = 0; i < hailstones.Count - 1; i++)
            for (int j = i + 1; j < hailstones.Count; j++)
            {
                var h1 = hailstones[i];
                var h2 = hailstones[j];
                // Point of intersection when (X1 + V1t1) = (X2 + V2t2) for some t1, t2
                // In x-y plane: 
                // t1 = (vy2 (px1 - px2) - vx2 (py1 - py2)) / (vy1 vx2 -  vx1 vy2)
                // Substitute this back to get the point of intersection...
                // Div. by 0 if paths are parallel
                var top = h2.vy * (h1.px - h2.px) - h2.vx * (h1.py - h2.py);
                var bottom = h1.vy * h2.vx - h1.vx * h2.vy;
                if (bottom == 0) continue;
                if (bottom < 0)
                {
                    // Doesn't change the value of t = top/bottom, but makes the
                    // subsequent logic easier. Trying to avoid division!
                    bottom = -bottom;
                    top = -top;
                }

                if (top < 0) // collision point is in the past for 1
                    continue;

                var top2 = h1.vy * (h2.px - h1.px) - h1.vx * (h2.py - h1.py);
                var bottom2 = h2.vy * h1.vx - h2.vx * h1.vy;
                if (top2 * bottom2 < 0) // collision point is in the past for 2
                    continue;

                // point of intersection is iX = h1.px + (top * h1.vx / bottom)
                var iXTimesBottom = h1.px * bottom + top * h1.vx; 
                var iYTimesBottom = h1.py * bottom + top * h1.vy;
                if (iXTimesBottom >= aoi_Low * bottom
                    && iXTimesBottom <= aoi_High * bottom
                    && iYTimesBottom >= aoi_Low * bottom
                    && iYTimesBottom <= aoi_High * bottom)
                {
                    collisions++;
                }
            }

            Console.WriteLine(collisions);
        }

        internal static void DoPart2(bool example)
        {
            var lines = ReadLines(24, example);
            var hailstones = lines.Select(x => Hailstone.Parse(x)).ToList();
            // Points of intersection are defined by X + V*t = Xh + Vh*t. Considering each hailstone
            // gives us three equations and one extra unknown, so we could take three hailstones
            // and start doing nasty linear algebra (by taking vector cross products to get (X - Xh) x (V - Vh) = 0
            // and cancelling the XxV term between various instances).

            // Hang on! We have two stones that have the same pz and vz. So the rock must have the same
            // values if it is to intercept both. Sadly same is not true for x, y directions.
            var zzz = hailstones.GroupBy(x => x.pz)
                .Where(g => g.Count() > 1);
            var rockZ = zzz.First().First().pz;
            var rockVz = zzz.First().First().vz;

            // Now we've got this, we can find the time of interception of other stones. Any two will do.
            var h1 = hailstones.First(x => x.pz != rockZ);
            var h2 = hailstones.Last(x => x.pz != rockZ);
            // rockZ + t1 * rockVz = stoneZ + t1 * stoneVz
            // keep potential rational numbers as integer pairs
            var t1_top = rockZ - h1.pz;
            var t1_bottom = h1.vz - rockVz;
            var t2_top = rockZ - h2.pz;
            var t2_bottom = h2.vz - rockVz;
            // Hey presto - the t's are integers!
            var t1 = t1_top / t1_bottom;
            var t2 = t2_top / t2_bottom;
            /*
             * Now we have
             * rockX + t1 * rockVx = stone1X + t1 * stone1Vx
             * rockX + t2 * rockVx = stone2X + t2 * stone2Vx
             *
             * whence
             * (t2-t1) * rockX + t2 * stone1X - t1 * stone2X
             *
             * and similar for y. 
             */
            var rockX = (t2 * h1.px - t1 * h2.px + t1 * t2 * (h1.vx - h2.vx)) / (t2 - t1);

            var rockY = (t2 * h1.py - t1 * h2.py + t1 * t2 * (h1.vy - h2.vy)) / (t2 - t1);

            Console.WriteLine(rockX + rockY + rockZ);
        }
    }

    internal class Hailstone
    {
        public BigInteger px, py, pz; // initial position
        public BigInteger vx, vy, vz;  // velocity

        public static Hailstone Parse(string line)
        {
            var pieces = line.Split(',', '@');
            return new Hailstone()
            {
                px = long.Parse(pieces[0].Trim()),
                py = long.Parse(pieces[1].Trim()),
                pz = long.Parse(pieces[2].Trim()),
                vx = int.Parse(pieces[3].Trim()),
                vy = int.Parse(pieces[4].Trim()),
                vz = int.Parse(pieces[5].Trim())
            };
        }
    }
}
