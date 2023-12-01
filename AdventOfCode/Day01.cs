namespace AdventOfCode;

using System.Text;
using System.Text.RegularExpressions;

public sealed class Day01 : BaseDay
{
    private readonly string[] _input;
    private readonly Regex _first = new("^[^\\d]*([\\d])");
    private readonly Regex _last = new("([\\d])[^\\d]*$");
    private readonly Regex _firstWords  ;
    private readonly Regex _lastWords;
    private readonly IDictionary<string, int> _numerals;

    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
        _numerals = new Dictionary<string, int>
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
        };

        for (var i = 0; i <= 9; i++)
        {
            _numerals.Add(i.ToString(), i);
        }

        var match = new StringBuilder();

        foreach (var (key, _) in _numerals)
        {
            if (match.Length > 0)
            {
                match.Append('|');
            }

            match.Append($"({key})");
        }

        _firstWords = new Regex($"^.*?({match})");
        _lastWords = new Regex($"({match})", RegexOptions.RightToLeft);
    }

    public override ValueTask<string> Solve_1()
    {
        var result = _input.Sum(line => int.Parse($"{FirstDigit(line)}" + $"{LastDigit(line)}"));

        return new ValueTask<string>($"Solution for {ClassPrefix} {CalculateIndex()} is {result}");
    }

    private string LastDigit(string line)
    {
        return _last.Match(line).Groups[1].Value;
    }

    private string FirstDigit(string line)
    {
        return _first.Match(line).Groups[1].Value;
    }

    public override ValueTask<string> Solve_2()
    {
        var result = (_input.Select(line => new { line, val1 = _numerals[_firstWords.Match(line).Groups[1].Value] })
            .Select(t => new { t, val2 = _numerals[_lastWords.Match(t.line).Groups[1].Value] })
            .Select(t => int.Parse($"{t.t.val1}{t.val2}"))).Sum();

        return new ValueTask<string>($"Solution for {ClassPrefix} {CalculateIndex()} is {result}");
    }
}