using System;

// 1. Singleton – Администратор ресторана
class RestaurantAdmin
{
    private static RestaurantAdmin _instance;
    private static readonly object _lock = new object();

    private RestaurantAdmin() { }

    public static RestaurantAdmin GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new RestaurantAdmin();
                }
            }
        }
        return _instance;
    }

    public bool IsAlcoholic { get; private set; }

    public bool IsOpen(int hour)
    {
        return hour >= 9 && hour < 23;
    }

    public bool IsAmericanKitchen(int hour)
    {
        return hour < 14;
    }

    public string GetKitchenType(int hour)
    {
        return IsAmericanKitchen(hour) ? "американская кухня" : "итальянская кухня";
    }

    public void Greet()
    {
        Console.WriteLine("Здравствуйте! Добро пожаловать в наш ресторан.");
    }

    public void AskForAlcoholPreference()
    {
        Console.Write("Вы пьете алкоголь? (да/нет): ");
        string response = Console.ReadLine().ToLower();
        IsAlcoholic = response == "да";
    }
}

// 2. Абстрактная фабрика (Abstract Factory) – Кухни (AmericanKitchen, ItalianKitchen)
interface IKitchen
{
    IBurger CreateBurger(int choice);
    IPasta CreatePasta(int choice);
    string GetMealDrink();
    string GetAlcoholDrink();  // Добавляем метод для обоих типов кухни
}

class AmericanKitchen : IKitchen
{
    public IBurger CreateBurger(int choice)
    {
        return choice switch
        {
            1 => new Cheeseburger(),
            2 => new DoubleBurger(),
            _ => new VeganBurger()
        };
    }

    public IPasta CreatePasta(int choice) => null;

    public string GetMealDrink()
    {
        return "Кола"; // Стандартный напиток для американской кухни
    }

    public string GetAlcoholDrink()
    {
        return "Пиво"; // Алкогольный напиток для американской кухни
    }
}

class ItalianKitchen : IKitchen
{
    public IBurger CreateBurger(int choice) => null;

    public IPasta CreatePasta(int choice)
    {
        return choice switch
        {
            1 => new Carbonara(),
            2 => new Fettuccine(),
            _ => new Lasagna()
        };
    }

    public string GetMealDrink()
    {
        return "Вино"; // Стандартный напиток для итальянской кухни
    }

    public string GetAlcoholDrink()
    {
        return "Вино"; // Алкогольный напиток для итальянской кухни
    }
}

// 3. Фабричный метод (Factory Method) – Вариации бургеров и пасты
interface IBurger
{
    void Cook();
}

class Cheeseburger : IBurger
{
    public void Cook()
    {
        Console.WriteLine("Готовится чизбургер...");
        Console.WriteLine("Подаётся блюдо.");
    }
}

class DoubleBurger : IBurger
{
    public void Cook()
    {
        Console.WriteLine("Готовится дабл бургер...");
        Console.WriteLine("Подаётся блюдо.");
    }
}

class VeganBurger : IBurger
{
    public void Cook()
    {
        Console.WriteLine("Готовится веган бургер...");
        Console.WriteLine("Подаётся блюдо.");
    }
}

interface IPasta
{
    void Cook();
}

class Carbonara : IPasta
{
    public void Cook()
    {
        Console.WriteLine("Готовится паста Карбонара...");
        Console.WriteLine("Подаётся блюдо.");
    }
}

class Fettuccine : IPasta
{
    public void Cook()
    {
        Console.WriteLine("Готовятся феттучини...");
        Console.WriteLine("Подаётся блюдо.");
    }
}

class Lasagna : IPasta
{
    public void Cook()
    {
        Console.WriteLine("Готовится лазанья...");
        Console.WriteLine("Подаётся блюдо.");
    }
}

// 4. Строитель (Builder) – Комплексный обед
class Meal
{
    public string MainCourse { get; set; }
    public string Drink { get; set; }

    public void ShowMeal() => Console.WriteLine($"Ваш комплексный обед: {MainCourse} и {Drink}");
}

class MealBuilder
{
    private readonly Meal _meal;

    public MealBuilder()
    {
        _meal = new Meal();
    }

    public MealBuilder AddMainCourse(string main)
    {
        _meal.MainCourse = main;
        return this;
    }

    public MealBuilder AddDrink(string drink)
    {
        _meal.Drink = drink;
        return this;
    }

    public Meal Build() => _meal;

    public MealBuilder RepeatMeal(Meal meal)
    {
        return new MealBuilder().AddMainCourse(meal.MainCourse).AddDrink(meal.Drink);
    }
}

// Главная программа
class Program
{
    static void Main()
    {
        RestaurantAdmin admin = RestaurantAdmin.GetInstance();
        admin.Greet();

        

        Console.Write("Сколько сейчас времени (0-23)? ");
        if (!int.TryParse(Console.ReadLine(), out int hour) || hour < 0 || hour > 23)
        {
            Console.WriteLine("Неверный формат времени.");
            return;
        }

        if (!admin.IsOpen(hour))
        {
            Console.WriteLine("Извините, ресторан закрыт.");
            return;
        }

        // Запрашиваем, пьет ли посетитель алкоголь
        admin.AskForAlcoholPreference();

        bool isAmericanKitchen = admin.IsAmericanKitchen(hour);
        Console.WriteLine($"Сейчас у нас {admin.GetKitchenType(hour)}.");
        IKitchen kitchen = isAmericanKitchen ? new AmericanKitchen() : new ItalianKitchen();

        // Спрашиваем количество людей
        Console.Write("Сколько человек будет за столом? ");
        if (!int.TryParse(Console.ReadLine(), out int peopleCount) || peopleCount < 1)
        {
            Console.WriteLine("Неверное количество людей.");
            return;
        }

        // Массив для хранения заказов
        Meal[] orders = new Meal[peopleCount];
        Meal? previousOrder = null;

        for (int i = 0; i < peopleCount; i++)
        {
            Console.WriteLine($"\nЗаказ для {i + 1}-го человека:");

            // Спрашиваем, хотят ли выбрать "мне тоже самое"
            if (i > 0)
            {
                Console.Write("Вы хотите выбрать то же самое, что и предыдущий человек? (да/нет): ");
                string sameAsPrevious = Console.ReadLine().ToLower();

                if (sameAsPrevious == "да" && previousOrder != null)
                {
                    Console.WriteLine("Вы выбрали тот же заказ.");
                    orders[i] = previousOrder;
                    continue;
                }
            }

            // Показываем меню
            if (isAmericanKitchen)
            {
                Console.WriteLine("1 - Бургер");
                Console.WriteLine("2 - Комплексный обед (бургер + кола)");
            }
            else
            {
                Console.WriteLine("1 - Пасту");
                Console.WriteLine("2 - Комплексный обед (паста + напиток)");
            }

            string choice = Console.ReadLine();
            Meal selectedMeal = null;

            if (isAmericanKitchen)
            {
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Выберите бургер:");
                        Console.WriteLine("1 - Чизбургер");
                        Console.WriteLine("2 - Дабл бургер");
                        Console.WriteLine("3 - Веган бургер");
                        IBurger burger = kitchen.CreateBurger(int.Parse(Console.ReadLine() ?? "1"));
                        burger?.Cook();
                        break;

                    case "2":
                        Console.WriteLine("Выберите бургер для обеда:");
                        Console.WriteLine("1 - Чизбургер");
                        Console.WriteLine("2 - Дабл бургер");
                        Console.WriteLine("3 - Веган бургер");
                        IBurger selectedBurger = kitchen.CreateBurger(int.Parse(Console.ReadLine() ?? "1"));
                        selectedMeal = new MealBuilder()
                            .AddMainCourse($"Бургер ({selectedBurger.GetType().Name})")
                            .AddDrink(admin.IsAlcoholic ? ((AmericanKitchen)kitchen).GetAlcoholDrink() : kitchen.GetMealDrink())
                            .Build();
                        selectedMeal.ShowMeal();
                        break;

                    default:
                        Console.WriteLine("Такого варианта нет в меню.");
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Выберите пасту:");
                        Console.WriteLine("1 - Карбонара");
                        Console.WriteLine("2 - Феттучини");
                        Console.WriteLine("3 - Лазанья");
                        IPasta pasta = kitchen.CreatePasta(int.Parse(Console.ReadLine() ?? "1"));
                        pasta?.Cook();
                        break;

                    case "2":
                        Console.WriteLine("Выберите пасту для обеда:");
                        Console.WriteLine("1 - Карбонара");
                        Console.WriteLine("2 - Феттучини");
                        Console.WriteLine("3 - Лазанья");
                        IPasta selectedPasta = kitchen.CreatePasta(int.Parse(Console.ReadLine() ?? "1"));
                        selectedMeal = new MealBuilder()
                            .AddMainCourse($"Паста ({selectedPasta.GetType().Name})")
                            .AddDrink(admin.IsAlcoholic ? "Вино" : "Сок")
                            .Build();
                        selectedMeal.ShowMeal();
                        break;

                    default:
                        Console.WriteLine("Такого варианта нет в меню.");
                        break;
                }
            }

            // Сохраняем заказ для текущего человека
            orders[i] = selectedMeal;
            previousOrder = selectedMeal;
        }

        // Подаем блюда для всех сразу
        Console.WriteLine("\nВсем заказаны блюда!");
        foreach (var order in orders)
        {
            order?.ShowMeal();
        }

        Console.WriteLine("Спасибо за визит! Ждём вас снова.");
    }
}
