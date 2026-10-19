using System;
using System.Collections.Concurrent;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Задание 1. Преобразование строк с помощью LINQ");
        ConvertStrings();

        Console.WriteLine();
        Console.WriteLine("Задание 2. Подсчет суммы с помощью PLINQ");
        CalculateSum();
    }

    static void ConvertStrings()
    {
        string[] inputs = { "123", "abc", "456", "def", "789" };

        int[] numbers = inputs
            .Select(text =>
            {
                try
                {
                    return (int?)int.Parse(text);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ошибка: строка \"" + text + "\" не является числом.");
                    return null;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Ошибка: число \"" + text + "\" слишком большое.");
                    return null;
                }
            })
            .Where(number => number.HasValue)
            .Select(number => number.Value)
            .ToArray();

        Console.WriteLine("Преобразованные числа:");
        foreach (int number in numbers)
        {
            Console.WriteLine(number);
        }
    }

    static void CalculateSum()
    {
        int[] numbers = Enumerable.Range(1, 20).ToArray();
        ConcurrentBag<int> errorPositions = new ConcurrentBag<int>();

        int sum = numbers
            .AsParallel()
            .AsOrdered()
            .Select((number, index) =>
            {
                // Порядковый номер начинается с 1, индекс массива - с 0.
                int position = index + 1;
                int x = position % 10;

                try
                {
                    return number / x;
                }
                catch (DivideByZeroException)
                {
                    // Позиции 10, 20 и т. д. не участвуют в сумме.
                    errorPositions.Add(position);
                    return 0;
                }
            })
            .Sum();

        foreach (int position in errorPositions.OrderBy(position => position))
        {
            Console.WriteLine("Ошибка: у элемента №" + position + " делитель равен 0.");
        }

        Console.WriteLine("Сумма S = " + sum);
    }
}
