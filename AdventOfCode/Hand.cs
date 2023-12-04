namespace AdventOfCode;

using System.Text.RegularExpressions;

public class Hand
{
    public Hand(string text)
    {
        var matches = Regex.Matches(text, @"(\d+) (red|green|blue)");
        foreach (Match match in matches)
        {
            if (match.Groups[2].Value == "red")
            {
                Red += int.Parse(match.Groups[1].Value);
            }

            if (match.Groups[2].Value == "green")
            {
                Green += int.Parse(match.Groups[1].Value);
            }

            if (match.Groups[2].Value == "blue")
            {
                Blue += int.Parse(match.Groups[1].Value);
            }
        }
    }

    public int Red { get; }
    public int Green { get;  }
    public int Blue { get; }
}