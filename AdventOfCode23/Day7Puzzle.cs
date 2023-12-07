using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode23
{
    internal class Day7Puzzle : PuzzleBase
    {
        internal static void Do(bool example, bool jacksAreWild)
        {
            var lines = ReadLines(7, example);
            var hands = lines.Select(x => new Hand(x, jacksAreWild)).ToList();

            int rank = 1;
            var sortedHands
                = hands.OrderBy(h => h.TypeKey)
                    .ThenBy(h => h.CardsSortKey);

            var total = sortedHands
                .Select(h => (bid: h.Bid, rank: rank++))
                .Sum(x => x.bid * x.rank);

            Console.WriteLine(total);
        }
    }

    internal class Hand
    {
        internal string Cards { get; private set; }
        internal int Bid { get; private set; }
        internal double TypeKey { get; private set; }
        internal long CardsSortKey { get; private set; }
        internal Hand(string input, bool jacksAreWild)
        {
            var parts = input.Split(' ');
            Cards = parts[0];
            Bid = int.Parse(parts[1]);
            TypeKey = FindTypeKey(Cards, jacksAreWild);
            CardsSortKey = FindCardsSortKey(Cards, jacksAreWild);
        }

        private int FindTypeKey(string cards, bool jacksAreWild)
        {
            var counts = cards.GroupBy(c => c)
                .Select(g => new { g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            if (jacksAreWild && counts.Count > 1) // Quins are quins regardless!
            {
                // It's always best to treat a J as if it were whatever else
                // you already have most of
                var jacks = counts.SingleOrDefault(c => c.Key == 'J');
                if (jacks != null)
                {
                    counts.Remove(jacks);
                    var first = counts.First();
                    counts.Remove(first);
                    counts.Insert(0, new { Key = first.Key, Count = first.Count + jacks.Count });
                }
            }

            // This returns 5.0 for 5 of a kind, 4.1 for 4, 3.2 for full house
            // etc. which is fine as a sorting key
            return counts.First().Count * 10
                   + (counts.Count() > 1 ? counts.ElementAt(1).Count : 0);
        }

        private long FindCardsSortKey(string cards, bool jacksAreWild)
        {
            // Again, purely a key - takes cards in their given
            // order rather than proper poker.
            return CardToValue(cards[4], jacksAreWild) + 15 * (
                CardToValue(cards[3], jacksAreWild) + 15 * (
                    CardToValue(cards[2], jacksAreWild) + 15 * (
                        CardToValue(cards[1], jacksAreWild) + 15 *
                        CardToValue(cards[0], jacksAreWild))));
        }

        private int CardToValue(char card, bool jacksAreWild)
        {
            switch (card)
            {
                case 'A': return 14;
                case 'K': return 13;
                case 'Q': return 12;
                case 'J': return (jacksAreWild) ? 1 : 11;
                case 'T': return 10;
                default: return card - '0';
            }
        }
    }
}
