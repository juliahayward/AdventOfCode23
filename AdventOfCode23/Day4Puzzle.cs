using System;
using System.Linq;

namespace AdventOfCode23
{
    internal class Day4Puzzle : PuzzleBase
    {
        internal static void DoPart1(bool example)
        {
            var lines = ReadLines(4, example);
            int totalScore = 0;

            foreach (var line in lines)
            {
                var parts = line.Split(':', '|');
                var winners = parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var mine = parts[2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var mywinners = winners.Intersect(mine);
                var score = (mywinners.Any()) ? Math.Pow(2, mywinners.Count() - 1) : 0;
                totalScore += (int)score;
            }

            Console.WriteLine(totalScore);
        }

        internal static void DoPart2(bool example)
        {
            var lines = ReadLines(4, example);
            var numberOfCards = new int[lines.Count];
            int currentCard = 0;    // Note - this is one less than the name in the data!

            foreach (var line in lines)
            {
                numberOfCards[currentCard] += 1;    // We're given one
                var parts = line.Split(':', '|');
                var winners = parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var mine = parts[2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var mywinners = winners.Intersect(mine);
                if (mywinners.Any())
                {
                    // We get one copy of the next few for *each* one we have at this index
                    for (int i = currentCard + 1; i <= currentCard + mywinners.Count(); i++)
                        numberOfCards[i] += numberOfCards[currentCard];
                }

                currentCard++;
            }

            Console.WriteLine(numberOfCards.Sum());
        }
    }
}
