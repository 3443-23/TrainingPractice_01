class Program
{
    static void Main()
    {
        Game game = new Game();
        game.Run();
    }
}

class Game
{
    private Map map;
    private Player player;
    private List<Enemy> enemies = new();
    private bool running = true;

    public void Run()
    {
        Console.CursorVisible = false;
        map = new Map("map.txt"); // Загружаем карту из файла
        player = new Player(1, 1, map); // Начальная позиция игрока
        SpawnEnemies(5); // Генерация врагов

        while (running)
        {
            map.Draw();
            player.Draw();
            foreach (var enemy in enemies) enemy.Draw();

            DrawHealthBar(0, map.Height + 1, player.Health); // Рисуем здоровье

            HandleInput(); // Обработка ввода
            UpdateEnemies(); // Движение врагов

            if (player.Health <= 0)
            {
                Console.Clear();
                Console.WriteLine("\nВы умерли!");
                break;
            }

            Thread.Sleep(150);
        }
    }

    private void HandleInput()
    {
        if (!Console.KeyAvailable) return;

        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.W: player.Move(0, -1); break;
            case ConsoleKey.S: player.Move(0, 1); break;
            case ConsoleKey.A: player.Move(-1, 0); break;
            case ConsoleKey.D: player.Move(1, 0); break;
            case ConsoleKey.M: map.ShowPath(player.X, player.Y); break; // Подсказка пути
        }
    }

    private void UpdateEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.MoveRandom();
            if (enemy.X == player.X && enemy.Y == player.Y)
            {
                player.TakeDamage(10); // Враг наносит урон при столкновении
            }
        }
    }

    private void DrawHealthBar(int left, int top, int percent)
    {
        int total = 10;
        int filled = percent * total / 100;
        string bar = "[" + new string('#', filled) + new string('_', total - filled) + "]";
        Console.SetCursorPosition(left, top);
        Console.Write($"HP {bar} {percent}%");
    }

    private void SpawnEnemies(int count)
    {
        Random rand = new();
        for (int i = 0; i < count; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(1, map.Width - 1);
                y = rand.Next(1, map.Height - 1);
            } while (!map.IsWalkable(x, y) || (x == player.X && y == player.Y));

            enemies.Add(new Enemy(x, y, map));
        }
    }
}

// --- Player.cs ---
class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Health { get; private set; } = 100;

    private Map map;

    public Player(int x, int y, Map map)
    {
        X = x; Y = y; this.map = map;
    }

    public void Move(int dx, int dy)
    {
        int newX = X + dx;
        int newY = Y + dy;
        if (map.IsWalkable(newX, newY))
        {
            X = newX;
            Y = newY;
            if (map.IsExit(X, Y))
            {
                Console.Clear();
                Console.WriteLine("\nПобеда!");
                Environment.Exit(0);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

    public void Draw()
    {
        Console.SetCursorPosition(X, Y);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write('P');
        Console.ResetColor();
    }
}

// --- Enemy.cs ---
class Enemy
{
    public int X { get; private set; }
    public int Y { get; private set; }
    private Map map;
    private static Random rand = new();

    public Enemy(int x, int y, Map map)
    {
        X = x;
        Y = y;
        this.map = map;
    }

    public void MoveRandom()
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        int dir = rand.Next(4);

        int newX = X + dx[dir];
        int newY = Y + dy[dir];

        if (map.IsWalkable(newX, newY))
        {
            X = newX;
            Y = newY;
        }
    }

    public void Draw()
    {
        Console.SetCursorPosition(X, Y);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write('X');
        Console.ResetColor();
    }
}

// --- Map.cs ---
class Map
{
    private char[,] tiles;
    public int Width => tiles.GetLength(1);
    public int Height => tiles.GetLength(0);

    public Map(string path)
    {
        var lines = File.ReadAllLines(path);
        tiles = new char[lines.Length, lines[0].Length];

        for (int y = 0; y < lines.Length; y++)
            for (int x = 0; x < lines[y].Length; x++)
                tiles[y, x] = lines[y][x];
    }

    public bool IsWalkable(int x, int y)
    {
        return tiles[y, x] != '#';
    }

    public bool IsExit(int x, int y)
    {
        return tiles[y, x] == 'E';
    }

    public void Draw()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(tiles[y, x]);
            }
        }
    }

    public void ShowPath(int startX, int startY)
    {
        var path = FindPath((startX, startY), FindExit());
        foreach (var (x, y) in path)
        {
            if (tiles[y, x] == ' ')
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write('.');
                Console.ResetColor();
            }
        }
    }

    private (int, int) FindExit()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (tiles[y, x] == 'E') return (x, y);
        return (0, 0);
    }

    private List<(int, int)> FindPath((int x, int y) start, (int x, int y) end)
    {
        var queue = new Queue<((int, int), List<(int, int)>)>();
        var visited = new HashSet<(int, int)>();

        queue.Enqueue((start, new List<(int, int)> { start }));
        visited.Add(start);

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();
            if (current == end) return path;

            for (int i = 0; i < 4; i++)
            {
                int nx = current.Item1 + dx[i];
                int ny = current.Item2 + dy[i];
                var next = (nx, ny);

                if (nx >= 0 && ny >= 0 && nx < Width && ny < Height && IsWalkable(nx, ny) && !visited.Contains(next))
                {
                    visited.Add(next);
                    var newPath = new List<(int, int)>(path) { next };
                    queue.Enqueue((next, newPath));
                }
            }
        }

        return new List<(int, int)>();
    }
}