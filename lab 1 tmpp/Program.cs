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
}

// 2. Abstract Factory – Кухни
interface IKitchen
{
    IBurger CreateBurger(int choice);
    IPasta CreatePasta(int choice);
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
}

// 3. Factory Method – Вариации блюд
interface IBurger
{
    void Cook();
}

class Cheeseburger : IBurger
{
    public void Cook() => Console.WriteLine("Готовится чизбургер...");
}

class DoubleBurger : IBurger
{
    public void Cook() => Console.WriteLine("Готовится дабл бургер...");
}

class VeganBurger : IBurger
{
    public void Cook() => Console.WriteLine("Готовится веган бургер...");
}

interface IPasta
{
    void Cook();
}

class Carbonara : IPasta
{
    public void Cook() => Console.WriteLine("Готовится паста Карбонара...");
}

class Fettuccine : IPasta
{
    public void Cook() => Console.WriteLine("Готовятся феттучини...");
}

class Lasagna : IPasta
{
    public void Cook() => Console.WriteLine("Готовится лазанья...");
}

// 4. Builder – Комплексный обед
class Meal
{
    public string MainCourse { get; private set; }
    public string Drink { get; private set; }

    public void ShowMeal() => Console.WriteLine($"Ваш комплексный обед: {MainCourse} и {Drink}");
}

class MealBuilder
{
    private readonly Meal _meal = new Meal();

    public MealBuilder AddMainCourse(string main)
    {
        _meal.GetType().GetProperty("MainCourse").SetValue(_meal, main);
        return this;
    }

    public MealBuilder AddDrink(string drink)
    {
        _meal.GetType().GetProperty("Drink").SetValue(_meal, drink);
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

        bool isAmericanKitchen = admin.IsAmericanKitchen(hour);
        Console.WriteLine("Проходите, пожалуйста. Сегодня у нас " + admin.GetKitchenType(hour) + ".");
        IKitchen kitchen = isAmericanKitchen ? new AmericanKitchen() : new ItalianKitchen();

        Console.WriteLine("Что закажете?");
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
                    Meal americanMeal = new MealBuilder()
                        .AddMainCourse("Бургер")
                        .AddDrink("Кола")
                        .Build();
                    americanMeal.ShowMeal();
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
                    Console.WriteLine("Выберите напиток:");
                    Console.WriteLine("1 - Вино");
                    Console.WriteLine("2 - Сок");
                    string drink = Console.ReadLine() == "1" ? "Вино" : "Сок";

                    Meal italianMeal = new MealBuilder()
                        .AddMainCourse("Паста")
                        .AddDrink(drink)
                        .Build();
                    italianMeal.ShowMeal();
                    break;

                default:
                    Console.WriteLine("Такого варианта нет в меню.");
                    break;
            }
        }

        Console.WriteLine("Спасибо за визит! Ждём вас снова.");
    }
}
