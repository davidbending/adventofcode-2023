namespace AdventOfCode;

using System.Text.RegularExpressions;

public sealed class Day04 : BaseDay
{
    private readonly string[] _input;
    private readonly Regex _cardRegex = new(@"^Card\s+\d+:\s+((\d+)\s+)+\|(\s+(\d+))+");

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }


    public override ValueTask<string> Solve_1()
    {
        var score = 0;

        // Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        foreach (var line in _input)
        {
            var match = _cardRegex.Match(line);

            if (!match.Success) continue;

            var winningNumbers = new HashSet<int>();
            foreach (Capture capture in match.Groups[1].Captures)
            {
                winningNumbers.Add(int.Parse(capture.Value.Trim()));
            }

            var card = 0;

            foreach (Capture capture in match.Groups[3].Captures)
            {
                if (winningNumbers.Contains(int.Parse(capture.Value.Trim())))
                {
                    if (card == 0)
                    {
                        card = 1;
                    }
                    else
                    {
                        card *= 2;
                    }
                }
            }

            score += card;
        }

        return new ValueTask<string>(score.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var cards = _input.Select(line => new Card(line)).ToList();
        var finishedCards = new List<Card>();

        foreach (var card in cards)
        {
            finishedCards.Add(card);
            var matches = card.MatchedNumbers.Intersect(card.WinningNumbers).Count();
            var cardCopies = finishedCards.Where(c => c.Index == card.Index).ToArray();

            foreach (var copy in cardCopies)
            {
                for (var i = 0; i < matches; i++)
                {
                    finishedCards.Add(cards[card.Index + i]);
                }
            }
        }

        return new ValueTask<string>(finishedCards.Count.ToString());
    }

    public struct Card
    {
        private static readonly Regex CardRegex = new(@"^Card\s+(\d+):\s+((\d+)\s+)+\|(\s+(\d+))+");

        public Card(string line)
        {
            WinningNumbers = new HashSet<int>();

            var match = CardRegex.Match(line);

            Index = int.Parse(match.Groups[1].Value);

            foreach (Capture capture in match.Groups[2].Captures)
            {
                WinningNumbers.Add(int.Parse(capture.Value.Trim()));
            }

            MatchedNumbers = new HashSet<int>();

            foreach (Capture capture in match.Groups[4].Captures)
            {
                MatchedNumbers.Add(int.Parse(capture.Value.Trim()));
            }
        }

        public int Index { get; set; }

        public ISet<int> WinningNumbers { get; }

        public ISet<int> MatchedNumbers { get; }
    }
}