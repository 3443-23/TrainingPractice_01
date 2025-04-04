abstract class ChessPiece
{
    public (int Row, int Col) Pos;

    // Конструктор: преобразует строковую позицию в координаты
    protected ChessPiece(string pos)
    {
        Pos = (pos[1] - '1', pos[0] - 'a');
    }

    public abstract bool CanMoveTo((int Row, int Col) target);
}

// Ладья: по вертикали или горизонтали
class Rook : ChessPiece
{
    public Rook(string p) : base(p) { }

    public override bool CanMoveTo((int Row, int Col) target)
    {
        return Pos.Row == target.Row || Pos.Col == target.Col;
    }
}

// Слон: по диагонали
class Bishop : ChessPiece
{
    public Bishop(string p) : base(p) { }

    public override bool CanMoveTo((int Row, int Col) target)
    {
        return Math.Abs(Pos.Row - target.Row) == Math.Abs(Pos.Col - target.Col);
    }
}

// Ферзь: сочетает движения ладьи и слона
class Queen : ChessPiece
{
    public Queen(string p) : base(p) { }

    public override bool CanMoveTo((int Row, int Col) target)
    {
        return new Rook(PosToString()).CanMoveTo(target) || new Bishop(PosToString()).CanMoveTo(target);
    }

    // Преобразование координат обратно в шахматную позицию
    private string PosToString() => $"{(char)(Pos.Col + 'a')}{Pos.Row + 1}";
}

// Конь: буквой "Г" (2 на 1)
class Knight : ChessPiece
{
    public Knight(string p) : base(p) { }

    public override bool CanMoveTo((int Row, int Col) target)
    {
        int dx = Math.Abs(Pos.Row - target.Row);
        int dy = Math.Abs(Pos.Col - target.Col);
        return dx * dy == 2;
    }
}

// Король: на 1 клетку в любом направлении
class King : ChessPiece
{
    public King(string p) : base(p) { }

    public override bool CanMoveTo((int Row, int Col) target)
    {
        return Math.Max(Math.Abs(Pos.Row - target.Row), Math.Abs(Pos.Col - target.Col)) == 1;
    }
}

class Program
{
    // Создание фигуры по названию
    static ChessPiece CreatePiece(string name, string pos) => name switch
    {
        "ладья" => new Rook(pos),
        "слон" => new Bishop(pos),
        "ферзь" => new Queen(pos),
        "конь" => new Knight(pos),
        "король" => new King(pos),
        _ => throw new ArgumentException("Неизвестная фигура")
    };

    static void Main()
    {
        Console.Write("Введите исходные данные: ");
        var input = Console.ReadLine().ToLower().Split();

        var white = CreatePiece(input[0], input[1]);  // Белая фигура
        var black = CreatePiece(input[2], input[3]);  // Черная фигура
        var target = (Row: input[4][1] - '1', Col: input[4][0] - 'a'); // Целевая клетка

        if (white.CanMoveTo(target))
        {
            if (black.CanMoveTo(target))
                Console.WriteLine($"{Cap(input[0])} не дойдет до {input[4]} — под ударом");
            else
                Console.WriteLine($"{Cap(input[0])} дойдет до {input[4]}");
        }
        else
        {
            Console.WriteLine($"{Cap(input[0])} не может дойти до {input[4]}");
        }
    }

    // Преобразует первую букву в верхний регистр
    static string Cap(string s) => char.ToUpper(s[0]) + s[1..];
}
