using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day6Puzzle : PuzzleBase
    {
        internal static void DoPart1(bool example)
        {
            var lines = ReadLines(6, example);
            var times = lines[0].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Skip(1);
            var distances = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Skip(1);

            // Given t, and the time we hold the button down x, the distance we
            // travel is x(t-x), and this needs to be greater than d. Once we've
            // done that, the number of ways is x_max - x_min + 1 where we solve
            // x(t-x)=d, x_min is the lower root rounded up and x_max is the higher
            // rounded down.
            // Caveat - if the roots are exact, then we need to round 1 more.
            long product = 1;
            for (int i = 0; i < times.Count(); i++)
            {
                var t = int.Parse(times.ElementAt(i));
                var d = int.Parse(distances.ElementAt(i));
                // x^2 - tx + d = 0
                var discriminant = Math.Sqrt(t * t - 4 * d);
                bool isSquare = ((int)(discriminant) * (int)discriminant == t * t - 4 * d);
                var x_min = (int)Math.Ceiling((t - discriminant) /2);
                var x_max = (int)Math.Floor((t + discriminant) /2);
                if (isSquare)
                {
                    Console.WriteLine((t * t - 4 * d));
                    x_min++;
                    x_max--;
                }
                product *= (x_max - x_min + 1);
            }

            Console.WriteLine(product);
        }

        internal static void DoPart2(bool example)
        {
            var lines = ReadLines(6, example);
            var t = long.Parse(lines[0].Replace(" ", "").Split(':').Last());
            var d = long.Parse(lines[1].Replace(" ", "").Split(':').Last());

            // Should be the same process as part 1, except watch for overflows!

            var discriminant = Math.Sqrt(t * t - 4 * d);
            bool isSquare = ((int)(discriminant) * (int)discriminant == t * t - 4 * d);
            var x_min = (int)Math.Ceiling((t - discriminant) / 2);
            var x_max = (int)Math.Floor((t + discriminant) / 2);
            if (isSquare)
            {
                Console.WriteLine((t * t - 4 * d));
                x_min++;
                x_max--;
            }

            Console.WriteLine(x_max - x_min + 1);
        }
    }
}
