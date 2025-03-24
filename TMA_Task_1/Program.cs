public class Player
{
    public int Gold { get; private set; }
    public int Crystals { get; private set; }

    public Player(int gold)
    {
        Gold = gold;
        Crystals = 0;
    }

    public void AttemptToBuyCrystals(int crystalsToBuy, int crystalPrice)
    {
        int cost = crystalsToBuy * crystalPrice;
        int canBuy = Convert.ToInt32(Gold >= cost);

        Gold -= cost * canBuy;
        Crystals += crystalsToBuy * canBuy;

        Console.WriteLine("\nРезультат сделки:");
        Console.WriteLine($"Остаток золота: {Gold}");
        Console.WriteLine($"Количество кристаллов: {Crystals}");
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        const int crystalPrice = 7;

        Console.Write("Введите ваше начальное количество золота: ");
        if (!int.TryParse(Console.ReadLine(), out int gold))
        {
            Console.WriteLine("Ошибка: введено не число. Завершение программы.");
            return;
        }

        Player player = new Player(gold);

        Console.Write("Введите количество кристаллов, которое хотите приобрести (цена одного кристалла - 7 золота): ");
        if (!int.TryParse(Console.ReadLine(), out int crystalsToBuy))
        {
            Console.WriteLine("Ошибка: введено не число. Завершение программы.");
            return;
        }

        player.AttemptToBuyCrystals(crystalsToBuy, crystalPrice);
    }
}