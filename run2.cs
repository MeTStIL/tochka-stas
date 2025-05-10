using System;
using System.Collections.Generic;
using System.Linq;


class Program
{
    // Константы для символов ключей и дверей
    static readonly char[] keys_char = Enumerable.Range('a', 26).Select(i => (char)i).ToArray();
    static readonly char[] doors_char = keys_char.Select(char.ToUpper).ToArray();
    static int[] dx = { -1, 1, 0, 0 };
    static int[] dy = { 0, 0, -1, 1 };

    // Метод для чтения входных данных
    static List<List<char>> GetInput()
    {
        using var input = new StreamReader(Console.OpenStandardInput());
        var data = new List<List<char>>();
        string line;
        while ((line = input.ReadLine()) != null && line != "")
        {
            data.Add(line.ToCharArray().ToList());
        }

        return data;
    }

    public static string GetPositions((int, int)[] pos)
    {
        var strPos = pos.Select(e => $"{e.Item1} {e.Item2}");
        return string.Join(";", strPos);
    }

    public static bool CanMove(char state, int keys)
    {
        if (char.IsUpper(state))
            return (keys & (1 << state - 'A')) != 0;
        return true;
    }


    static int Solve(List<List<char>> data)
    {
        var n = data.Count;
        var m = data[0].Count;

        var allRobots = new List<(int, int)>();
        var targetKeys = 0;

        for (var x = 0; x < n; x++)
        {
            for (var y = 0; y < m; y++)
            {
                if (data[x][y] == '@')
                    allRobots.Add((x, y));

                if (char.IsLower(data[x][y]))
                    targetKeys |= 1 << (data[x][y] - 'a');
            }
        }

        var (pos1, pos2, pos3, pos4) = (allRobots[0], allRobots[1], allRobots[2], allRobots[3]);

        var startPos = new[]
        {
            pos1,
            pos2,
            pos3,
            pos4
        };


        var queue = new Queue<(int, (int, int)[], int)>();
        var visited = new HashSet<(long, string)>();
        visited.Add((0, GetPositions(startPos)));
        queue.Enqueue((0, startPos, 0));

        while (queue.Count > 0)
        {
            var (keys, poss, steps) = queue.Dequeue();
            if (keys == targetKeys)
                return steps;

            for (var r = 0; r < 4; r++)
            {
                var (x, y) = poss[r];

                for (var direc = 0; direc < 4; direc++)
                {
                    var (newX, newY) = (x + dx[direc], y + dy[direc]);

                    if (0 <= newX && newX < n && 0 <= newY && newY < m && data[newX][newY] != '#')
                    {
                        if (CanMove(data[newX][newY], keys))
                        {
                            var newKeys = keys;
                            var newPoss = new (int, int)[4];

                            if (char.IsLower(data[newX][newY]))
                                newKeys |= 1 << (data[newX][newY] - 'a');

                            Array.Copy(poss, newPoss, 4);
                            newPoss[r] = (newX, newY);
                            var s = (newKeys, GetPositions(newPoss));

                            if (!visited.Contains(s))
                            {
                                queue.Enqueue((newKeys, newPoss, steps + 1));
                                visited.Add(s);
                            }
                        }
                    }
                }
            }
        }

        return -1;
    }

    static void Main()
    {
        using var output = new StreamWriter(Console.OpenStandardOutput());
        var data = GetInput();
        var result = Solve(data);

        if (result == -1)
            output.WriteLine("No solution found");
        else
            output.WriteLine(result);
    }
}