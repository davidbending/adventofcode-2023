namespace AdventOfCode;

using System.Security.Principal;
using System.Text.RegularExpressions;
using Spectre.Console;

public sealed class Day05 : BaseDay
{
    private readonly string[] _input;
    private readonly IList<long> _seeds;
    private readonly Map _seedSoilMap;
    private readonly Map _soilFertilizerMap;
    private readonly Map _fertilizerWaterMap;
    private readonly Map _waterLightMap;
    private readonly Map _lightTemperatureMap;
    private readonly Map _temperatureHumidityMap;
    private readonly Map _humidityLocationMap;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);

        _seeds = ReadSeeds(_input[0]);
        var maps = ReadMaps(_input[1..]);

        _seedSoilMap = maps["seed-soil"];
        _soilFertilizerMap = maps["soil-fertilizer"];
        _fertilizerWaterMap = maps["fertilizer-water"];
        _waterLightMap = maps["water-light"];
        _lightTemperatureMap = maps["light-temperature"];
        _temperatureHumidityMap = maps["temperature-humidity"];
        _humidityLocationMap = maps["humidity-location"];
    }

    private IList<long> ReadSeeds(string line)
    {
        var seedsRegex = new Regex(@"^seeds: ((\d+)\s*)+");
        var match = seedsRegex.Match(line);
        if (match.Success)
        {
            return match.Groups[2].Captures.Select(cap => long.Parse(cap.Value)).ToList();
        }

        return new List<long>();
    }

    private Dictionary<string, Map> ReadMaps(string[] lines)
    {
        var mapDictionary = new Dictionary<string, Map>();
        var map = Map.Empty;
        var mapRegex = new Regex(@"^([^\-]+)\-to\-([^\-]+) map:");
        var mapEntry = new Regex(@"^(\d+)\s+(\d+)\s+(\d+)");

        foreach (var line in lines)
        {
            var match = mapRegex.Match(line);
            if (match.Success)
            {
                if (!map.IsEmpty)
                {
                    mapDictionary.Add($"{map.From}-{map.To}", map);
                }

                map = new Map(match.Groups[1].Value, match.Groups[2].Value);
            }
            else
            {
                match = mapEntry.Match(line);

                if (match.Success)
                {
                    map.AddMapping(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
                }
            }
        }

        if (!map.IsEmpty)
        {
            mapDictionary.Add($"{map.From}-{map.To}", map);
        }

        return mapDictionary;
    }


    public override ValueTask<string> Solve_1()
    {
        var lowestLocation = long.MaxValue;

        foreach (var seed in _seeds)
        {
            var location = GetLocation(seed);

            if (location < lowestLocation)
            {
                lowestLocation = location;
            }
        }

        return new ValueTask<string>(lowestLocation.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var lowestLocation = long.MaxValue;
        var seedRanges = new SortedList<long, SeedRange>();

        for (var i = 0; i < _seeds.Count; i += 2)
        {
            seedRanges.Add(_seeds[i], new SeedRange(_seeds[i], _seeds[i + 1]));
        }

        var block = 1;
        var lastRange = new SeedRange(0, 0);

        foreach (var (_, range) in seedRanges)
        {
            var done = 0;
            Console.WriteLine($"Working on block {block++} of {seedRanges.Count} (lastRange: {lastRange}, this: {range})");

            if (range.Start < lastRange.End)
            {
                range.Start = lastRange.End;
                Console.WriteLine($"\tAdjusted to {range}");
            }

            lastRange = range;

            for (var seed = range.Start; seed < range.End; seed++)
            {
                var location = GetLocation(seed);

                if (location < lowestLocation)
                {
                    lowestLocation = location;
                }

                done++;
                if (done % 10_000_000 == 0)
                {
                    Console.WriteLine($"\tCompleted {done:N0} of {range.Length:N0} ({1.0 * done / range.Length:P0})");
                }
            }
        }

        return new ValueTask<string>(lowestLocation.ToString());
    }

    private long GetLocation(long seed)
    {
        var soil = _seedSoilMap.Lookup(seed);
        var fertilizer = _soilFertilizerMap.Lookup(soil);
        var water = _fertilizerWaterMap.Lookup(fertilizer);
        var light = _waterLightMap.Lookup(water);
        var temperature = _lightTemperatureMap.Lookup(light);
        var humidity = _temperatureHumidityMap.Lookup(temperature);
        var location = _humidityLocationMap.Lookup(humidity);
        return location;
    }

    private class SeedRange(long start, long length)
    {
        public long Start { get; set; } = start;

        public long Length { get; set; } = length;

        public long End { get; set; } = start + length;

        public override string ToString()
        {
            return $"{Start:N0} --> {End:N0} ({Length:N0} entries)";
        }
    }

    private class Map(string from, string to)
    {
        public string From { get; } = from;

        public string To { get; } = to;

        public static Map Empty => new(string.Empty, string.Empty);

        public bool IsEmpty => string.IsNullOrEmpty(From);

        private readonly SortedList<long, Mapping> _mappings = new();

        public void AddMapping(string destStart, string sourceStart, string length)
        {
            var mapping = new Mapping(destStart, sourceStart, length);
            _mappings.Add(mapping.SourceStart, mapping);
        }

        public long Lookup(long value)
        {
            foreach (var (_, mapping) in _mappings)
            {
                if (value >= mapping.SourceStart && value < mapping.SourceEnd)
                {
                    return (value - mapping.SourceStart) + mapping.DestinationStart;
                }

                if (value < mapping.SourceStart) break;
            }

            return value;
        }
    }

    private class Mapping(string destStart, string sourceStart, string length)
    {
        public long DestinationStart { get; set; } = long.Parse(destStart);

        public long SourceStart { get; set; } = long.Parse(sourceStart);

        public long SourceEnd { get; set; } = long.Parse(sourceStart) + long.Parse(length);

        public long Length { get; set; } = long.Parse(length);
    }
}