using System;
using System.Collections.Generic;

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

// 2. Абстрактная фабрика – Кухни
interface IKitchen
{
    IBurger CreateBurger(int choice);
    IPasta CreatePasta(int choice);
    string GetMealDrink();
    string GetAlcoholDrink();
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
    public string GetMealDrink() => "Кола";
    public string GetAlcoholDrink() => "Пиво";
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

    public string GetMealDrink() => "Сок";
    public string GetAlcoholDrink() => "Вино";
}

// 3. Фабричный метод – Бургеры и паста
interface IBurger
{
    string GetName();
}

class Cheeseburger : IBurger { public string GetName() => "Чизбургер"; }
class DoubleBurger : IBurger { public string GetName() => "Дабл бургер"; }
class VeganBurger : IBurger { public string GetName() => "Веган бургер"; }

interface IPasta
{
    string GetName();
}

class Carbonara : IPasta { public string GetName() => "Паста Карбонара"; }
class Fettuccine : IPasta { public string GetName() => "Феттучини"; }
class Lasagna : IPasta { public string GetName() => "Лазанья"; }

// 4. Строитель – Комплексный обед
class Meal
{
    public string MainCourse { get; set; }
    public string Drink { get; set; }

    public void ShowMeal() => Console.Write($"{MainCourse} ({Drink})");
}

class MealBuilder
{
    private readonly Meal _meal = new Meal();

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

        Console.WriteLine($"Сейчас у нас {admin.GetKitchenType(hour)}.");
        bool isAmericanKitchen = admin.IsAmericanKitchen(hour);
        IKitchen kitchen = isAmericanKitchen ? new AmericanKitchen() : new ItalianKitchen();

        Console.Write("Сколько человек будет за столом? ");
        if (!int.TryParse(Console.ReadLine(), out int peopleCount) || peopleCount < 1)
        {
            Console.WriteLine("Неверное количество людей.");
            return;
        }

        List<Meal> orders = new List<Meal>();
        Meal? previousOrder = null;

        for (int i = 0; i < peopleCount; i++)
        {
            Console.WriteLine($"\nЗаказ для {i + 1}-го человека:");

            if (i > 0)
            {
                Console.Write("Вы хотите выбрать то же самое, что и предыдущий человек? (да/нет): ");
                string sameAsPrevious = Console.ReadLine().ToLower();

                if (sameAsPrevious == "да" && previousOrder != null)
                {
                    Console.WriteLine("Вы выбрали тот же заказ.");
                    orders.Add(previousOrder);
                    continue;
                }
            }

            // Спрашиваем про алкоголь
            admin.AskForAlcoholPreference();

            MealBuilder mealBuilder = new MealBuilder();

            if (isAmericanKitchen)
            {
                Console.WriteLine("1 - Бургер");
                Console.WriteLine("2 - Комплексный обед (бургер + напиток)");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("Выберите бургер:");
                    Console.WriteLine("1 - Чизбургер");
                    Console.WriteLine("2 - Дабл бургер");
                    Console.WriteLine("3 - Веган бургер");
                    IBurger burger = kitchen.CreateBurger(int.Parse(Console.ReadLine() ?? "1"));
                    mealBuilder.AddMainCourse(burger.GetName());
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Выберите бургер для обеда:");
                    Console.WriteLine("1 - Чизбургер");
                    Console.WriteLine("2 - Дабл бургер");
                    Console.WriteLine("3 - Веган бургер");
                    IBurger burger = kitchen.CreateBurger(int.Parse(Console.ReadLine() ?? "1"));
                    mealBuilder.AddMainCourse(burger.GetName());
                    mealBuilder.AddDrink(admin.IsAlcoholic ? kitchen.GetAlcoholDrink() : kitchen.GetMealDrink());
                }
            }
            else
            {
                Console.WriteLine("1 - Пасту");
                Console.WriteLine("2 - Комплексный обед (паста + напиток)");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("Выберите пасту:");
                    Console.WriteLine("1 - Карбонара");
                    Console.WriteLine("2 - Феттучини");
                    Console.WriteLine("3 - Лазанья");
                    IPasta pasta = kitchen.CreatePasta(int.Parse(Console.ReadLine() ?? "1"));
                    mealBuilder.AddMainCourse(pasta.GetName());
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Выберите пасту для обеда:");
                    Console.WriteLine("1 - Карбонара");
                    Console.WriteLine("2 - Феттучини");
                    Console.WriteLine("3 - Лазанья");
                    IPasta pasta = kitchen.CreatePasta(int.Parse(Console.ReadLine() ?? "1"));
                    mealBuilder.AddMainCourse(pasta.GetName());
                    mealBuilder.AddDrink(admin.IsAlcoholic ? kitchen.GetAlcoholDrink() : kitchen.GetMealDrink());
                }
            }

            Meal meal = mealBuilder.Build();
            orders.Add(meal);
            previousOrder = meal;
        }

        Console.WriteLine("\nГотовятся блюда: ");
        foreach (var order in orders)
        {
            order.ShowMeal();
            Console.Write(", ");
        }
        Console.WriteLine("\nВсе блюда поданы! Спасибо за визит.");
    }
}
