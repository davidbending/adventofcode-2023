namespace AdventOfCode;

using System.Text;

public sealed class Day03 : BaseDay
{
    private readonly string[] _input;
    private char[][] _matrix;
    private int _height;
    private int _width;

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }


    public override ValueTask<string> Solve_1()
    {
        GetMatrix();

        var partNumbers = new List<int>();
        var total = 0;

        for (var row = 0; row < _matrix.Length; row++)
        {
            var partNumber = new StringBuilder();
            var adjacentToSymbol = false;

            for (var col = 0; col < _matrix[row].Length; col++)
            {
                if (char.IsDigit(_matrix[row][col]))
                {
                    adjacentToSymbol = adjacentToSymbol || IsAdjacentToSymbol(row, col);
                    partNumber.Append(_matrix[row][col]);
                }
                else
                {
                    if (adjacentToSymbol && partNumber.Length > 0)
                    {
                        partNumbers.Add(int.Parse(partNumber.ToString()));
                        total += int.Parse(partNumber.ToString());
                    }

                    adjacentToSymbol = false;
                    partNumber = new StringBuilder();
                }
            }

            if (adjacentToSymbol && partNumber.Length > 0)
            {
                partNumbers.Add(int.Parse(partNumber.ToString()));
                total += int.Parse(partNumber.ToString());
            }
        }

        return new ValueTask<string>($"{partNumbers.Sum(s => s).ToString()} which should be {total}");
    }

    private void GetMatrix()
    {
        _matrix = _input.Select(l => l.ToCharArray()).ToArray();
        _height = _input.Length;
        _width = _matrix[0].Length;
    }

    private bool IsAdjacentToSymbol(int row, int col)
    {
        if (row > 0)
        {
            if (col > 0)
            {
                if (IsSymbol(row - 1, col - 1)) return true;
            }

            if (IsSymbol(row - 1, col)) return true;

            if (col < _height - 1)
            {
                if (IsSymbol(row - 1, col + 1)) return true;
            }
        }

        if (col > 0)
        {
            if (IsSymbol(row, col - 1)) return true;
        }

        if (col < _width - 1)
        {
            if (IsSymbol(row, col + 1)) return true;

        }

        if (row < _height - 1)
        {
            if (col > 0)
            {
                if (IsSymbol(row + 1, col - 1)) return true;
            }

            if (IsSymbol(row + 1, col)) return true;

            if (col < _height - 1)
            {
                if (IsSymbol(row + 1, col + 1)) return true;
            }
        }

        return false;
    }

    private bool IsSymbol(int row, int col)
    {
        var c = _matrix[row][col];
        if (!char.IsDigit(c) && c != '.')
        {
            return true;
        }

        return false;
    }

    public override ValueTask<string> Solve_2()
    {
        GetMatrix();

        var total = 0;

        for (var row = 0; row < _height; row++)
        {
            for (var col = 0; col < _width; col++)
            {
                if (_matrix[row][col] == '*')
                {
                    var (match, num1, num2) = FindAdjacentNumbers(row, col);
                    if (match)
                    {
                        total += (num1 * num2);
                    }
                }
            }
        }

        return new ValueTask<string>(total.ToString());
    }

    private (bool match, int num1, int num2) FindAdjacentNumbers(int row, int col)
    {
        var adjacentNumbers = new List<int>();

        if (row > 0)
        {
            SearchRow(adjacentNumbers, row - 1, col);
        }

        SearchRow(adjacentNumbers, row, col);

        if (row < _height - 1)
        {
            SearchRow(adjacentNumbers, row + 1, col);
        }

        if (adjacentNumbers.Count == 2)
        {
            return (true, adjacentNumbers[0], adjacentNumbers[1]);
        }

        return (false, 0, 0);
    }

    private void SearchRow(List<int> adjacentNumbers, int row, int col)
    {
        var foundDigit = false;
        var x = Math.Max(0, col - 1);
        while (x > 0 && IsDigit(row, x))
        {
            foundDigit = true;
            x--;
        }

        var number = new StringBuilder();
        if (foundDigit && !IsDigit(row, x)) x++;

        while (x < _width && IsDigit(row, x))
        {
            if (IsDigit(row, x))
            {
                number.Append(_matrix[row][x]);
            }
            x++;
        }

        if (number.Length > 0)
        {
            adjacentNumbers.Add(int.Parse(number.ToString()));
        }

        if (x > col)
        {
            return;
        }

        number.Clear();
        x = Math.Min(_width - 1, col);

        while (x < _width - 1 && (IsDigit(row, x) || (x - col) <= 1))
        {
            if (IsDigit(row, x))
            {
                number.Append(_matrix[row][x]);
            }

            x++;
        }

        if (number.Length > 0)
        {
            adjacentNumbers.Add(int.Parse(number.ToString()));
        }
    }

    private bool IsDigit(int row, int col)
    {
        return char.IsDigit(_matrix[row][col]);
    }
}