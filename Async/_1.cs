using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Async
{
    internal static class _1
    {
        public static void First()
        {
            int[] array = GenerateLargeArray(1000000);

            int mid = array.Length / 2;
            int[] firstHalf = array.Take(mid).ToArray();
            int[] secondHalf = array.Skip(mid).ToArray();

            Task<int> task1 = Task.Run(() => SumArray(firstHalf));
            Task<int> task2 = Task.Run(() => SumArray(secondHalf));

            int sum1 = task1.Result;
            int sum2 = task2.Result;

            int totalSum = sum1 + sum2;

            Console.WriteLine($"Сумма первой половины: {sum1}");
            Console.WriteLine($"Сумма второй половины: {sum2}");
            Console.WriteLine($"Общая сумма: {totalSum}");
        }

        static int[] GenerateLargeArray(int size)
        {
            Random random = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = random.Next(-10000, 10000);
            }
            return array;
        }

        static int SumArray(int[] array)
        {
            return array.Sum();
        }


        public static void Second()
        {
            List<string> strings = new List<string>
            {
                "Hello, world!",
                "My name us Anton",
                "Nice to meet you",
            };

            Dictionary<string, int> characterCounts = new Dictionary<string, int>();

            Parallel.ForEach(strings, str =>
            {
                int count = str.Length;

                lock (characterCounts)
                {
                    characterCounts[str] = count;
                }
            });

            Console.WriteLine("Результаты подсчёта символов в строках:");
            foreach (var kvp in characterCounts)
            {
                Console.WriteLine($"Строка: \"{kvp.Key}\", Количество символов: {kvp.Value}");
            }
        }


        public static void Third()
        {
            int[] numbers = GenerateLargeArray(100000000);

            var stopwatch = Stopwatch.StartNew();
            var (minParallel, maxParallel) = FindMinMaxParallel(numbers);
            stopwatch.Stop();
            Console.WriteLine($"Параллельный поиск: Min = {minParallel}, Max = {maxParallel}, Время = {stopwatch.ElapsedMilliseconds} мс");


            stopwatch.Restart();
            var (minSequential, maxSequential) = FindMinMaxSequential(numbers);
            stopwatch.Stop();
            Console.WriteLine($"Последовательный поиск: Min = {minSequential}, Max = {maxSequential}, Время = {stopwatch.ElapsedMilliseconds} мс");
        }


        static (int min, int max) FindMinMaxParallel(int[] array)
        {
            int numThreads = Environment.ProcessorCount; 
            int chunkSize = array.Length / numThreads; 

            int[] mins = new int[numThreads];
            int[] maxs = new int[numThreads];

            Parallel.For(0, numThreads, threadIndex =>
            {
                int start = threadIndex * chunkSize;
                int end = (threadIndex == numThreads - 1) ? array.Length : start + chunkSize;

                int localMin = int.MaxValue;
                int localMax = int.MinValue;

                for (int i = start; i < end; i++)
                {
                    if (array[i] < localMin) localMin = array[i];
                    if (array[i] > localMax) localMax = array[i];
                }

                mins[threadIndex] = localMin;
                maxs[threadIndex] = localMax;
            });

            int min = mins.Min();
            int max = maxs.Max();

            return (min, max);
        }

        static (int min, int max) FindMinMaxSequential(int[] array)
        {
            int min = array[0];
            int max = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] < min) min = array[i];
                if (array[i] > max) max = array[i];
            }

            return (min, max);
        }
    }
}
