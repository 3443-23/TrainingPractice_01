public class Player
{
    public int Health { get; set; }
    public int Mana { get; set; }
    public bool RashamonActive { get; set; }

    public const int MaxHealth = 1000;
    public const int MaxMana = 500;

    public Player(int health, int mana)
    {
        Health = health;
        Mana = mana;
        RashamonActive = false;
    }

    public bool UseRashamon()
    {
        if (Health > 100 && !RashamonActive)
        {
            Health -= 100;
            RashamonActive = true;
            Console.WriteLine("\nВы призываете теневого духа Рашамон!");
            return true;
        }
        else if (RashamonActive)
        {
            Console.WriteLine("\nВы уже призвали духа Рашамон!");
        }
        else if (Health <= 100)
        {
            Health -= 100;
            RashamonActive = true;
            Console.WriteLine("\nВы слишком слабы. Вашу душу забрал Рашамон!");
            return true;
        }
        else
        {
            Console.WriteLine("\nНедостаточно здоровья для призыва Рашамон!");
        }
        return false;
    }

    public bool UseHuganzakura(Boss boss)
    {
        if (RashamonActive && Mana >= 75)
        {
            Mana -= 75;
            boss.TakeDamage(300);
            RashamonActive = false;
            Console.WriteLine("\nВы обрушиваете Хуганзакуру на Босса!");
            return true;
        }
        else if (Mana < 75)
        {
            Console.WriteLine("\nНедостаточно маны для Хуганзакуры!");
        }
        else
        {
            Console.WriteLine("\nСначала нужно призвать Рашамон!");
        }
        return false;
    }

    public bool UseInterdimensionalRift()
    {
        if (Mana >= 100)
        {
            Mana -= 100;
            Health += 400;
            Health = Math.Min(Health, MaxHealth);
            Console.WriteLine("\nВы скрываетесь в межпространственном разломе и восстанавливаете здоровье!");
            return true;
        }
        Console.WriteLine("\nНедостаточно маны для разлома!");
        return false;
    }

    public bool UseDarkStrike(Boss boss)
    {
        if (Mana >= 125)
        {
            Mana -= 125;
            boss.TakeDamage(200);
            Console.WriteLine("\nВы наносите Темный Удар!");
            return true;
        }
        Console.WriteLine("\nНедостаточно маны для Темного Удара!");
        return false;
    }

    public bool UseDarkAbsorption()
    {
        if (Mana < MaxMana)
        {
            Mana += 150;
            Mana = Math.Min(Mana, MaxMana);
            Console.WriteLine("\nВы поглощаете тьму и восстанавливаете ману!");
            return true;
        }
        Console.WriteLine("\nМана уже полна!");
        return false;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}

public class Boss
{
    public int Health { get; private set; }
    public int Damage { get; private set; }

    public Boss(int health, int damage)
    {
        Health = health;
        Damage = damage;
    }

    public void Attack(Player player)
    {
        Console.WriteLine($"\nБосс атакует! (Урон - {Damage})");
        player.TakeDamage(Damage);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}

public class Game
{
    private Player _player;
    private Boss _boss;

    public Game()
    {
        _player = new Player(1000, 500);
        _boss = new Boss(2000, 150);
    }

    public void Start()
    {
        Console.WriteLine("Вы - Теневой Маг, стоящий перед могущественным Боссом!");

        while (_player.Health > 0 && _boss.Health > 0)
        {
            Console.WriteLine("\n----- Новый ход -----");
            Console.WriteLine($"\nВаше здоровье: {_player.Health}, Ваша мана: {_player.Mana}");
            Console.WriteLine($"Здоровье Босса: {_boss.Health}");

            Console.WriteLine("\nВыберите заклинание: ");
            Console.WriteLine("1. Рашамон - Призывает теневого духа (Стоимость: 100 здоровья).");
            Console.WriteLine("2. Хуганзакура - Наносит 300 урона (После Рашамона, Стоимость: 75 маны).");
            Console.WriteLine("3. Межпространственный разлом - Восстанавливает 250 здоровья (Стоимость: 100 маны).");
            Console.WriteLine("4. Темный Удар - 200 урона (Стоимость: 125 маны).");
            Console.WriteLine("5. Поглощение Тьмы - Восстанавливает 150 маны.");

            bool spellUsed = false;

            while (!spellUsed)
            {
                Console.Write("\nВведите номер заклинания: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1: spellUsed = _player.UseRashamon(); break;
                        case 2: spellUsed = _player.UseHuganzakura(_boss); break;
                        case 3: spellUsed = _player.UseInterdimensionalRift(); break;
                        case 4: spellUsed = _player.UseDarkStrike(_boss); break;
                        case 5: spellUsed = _player.UseDarkAbsorption(); break;
                        default: Console.WriteLine("Неверный выбор."); break;
                    }
                }
                else
                {
                    Console.WriteLine("Введите корректный номер.");
                }
            }

            if (_boss.Health > 0 && _player.Health > 0)
            {
                _boss.Attack(_player);
            }

            if (_player.Health <= 0)
            {
                Console.WriteLine("\nВы повержены! Тьма поглотила вас...");
            }
            else if (_boss.Health <= 0)
            {
                Console.WriteLine("\nВы уничтожили босса! Тьма покидает этот мир.");
            }
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {

        Game game = new Game();
        game.Start();
    }
}