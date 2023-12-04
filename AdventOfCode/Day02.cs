namespace AdventOfCode;

using System.Text.RegularExpressions;

public sealed class Day02 : BaseDay
{
    private readonly string _input;
    private readonly Regex _gameRegex = new Regex(@"^Game (\d+): (((\d)+ (red|green|blue)(, )?)+( ;)?)");

    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var games = ParseGames();

        // only 12 red cubes, 13 green cubes, and 14 blue cubes?
        var matchingGames = games.Where(g => g.Red <= 12 && g.Green <= 13 && g.Blue <= 14);

        return new ValueTask<string>(matchingGames.Sum(g => g.Index).ToString());
    }

    private List<Game> ParseGames()
    {
        var lines = _input.Split("\r\n");
        var games = new List<Game>();

        foreach (var line in lines)
        {
            games.Add(ParseLineMax(line));
        }

        return games;
    }

    public override ValueTask<string> Solve_2()
    {
        var lines = _input.Split("\r\n");
        var total = 0;

        foreach (var line in lines)
        {
            total+=ParseLineMin(line);
        }

        return new ValueTask<string>(total.ToString());
    }


    private Game ParseLineMax(string line)
    {
        var game = new Game();
        var hands = new List<Hand>();

        var match = Regex.Match(line, @"^Game (\d+):\s*(.*)");
        if (match.Success)
        {
            game.Index = int.Parse(match.Groups[1].Value);
            var chunks = match.Groups[2].Value.Split(";");
            foreach (var chunk in chunks)
            {
                var hand = new Hand(chunk.Trim());
                hands.Add(hand);
            }

            game.Red = hands.Max(h => h.Red);
            game.Green = hands.Max(h => h.Green);
            game.Blue = hands.Max(h => h.Blue);
        }

        return game;
    }

    private int ParseLineMin(string line)
    {
        var game = new Game();
        var hands = new List<Hand>();

        var match = Regex.Match(line, @"^Game (\d+):\s*(.*)");
        if (match.Success)
        {
            game.Index = int.Parse(match.Groups[1].Value);
            var chunks = match.Groups[2].Value.Split(";");
            foreach (var chunk in chunks)
            {
                var hand = new Hand(chunk.Trim());
                hands.Add(hand);
            }

            game.Red = hands.Max(h => h.Red);
            game.Green = hands.Max(h => h.Green);
            game.Blue = hands.Max(h => h.Blue);
        }

        return game.Red * game.Green * game.Blue;
    }

}

//Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
//    Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
//    Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
//    Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
//    Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green