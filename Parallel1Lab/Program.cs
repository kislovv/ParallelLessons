using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Parallel1Lab
{
    internal class Program
    {
        private static double[] _array;
        private static int _threadCount;
        private static readonly Dictionary<int, int> ElementToThreadsNumber = new Dictionary<int, int>
        {
            {10, 2},
            {102, 3},
            {1000, 4},
            {10000, 5},
            {100000, 6}
        };

        //Коэфициент сложности (для пункта 4,6)
        private const int Coeficient = 10000;

        private static void RunAroundParallel(object objNumberOfThreads)
        {
            var threadNumber = (int) objNumberOfThreads;

            for (var index = threadNumber; index < _array.Length; index += _threadCount)
            {
                _array[index] = index;
                for (var indexCoef = 0; indexCoef < Coeficient; indexCoef++)
                {
                    _array[index] = ArrayDifficultOperation(_array[index]);
                }
            }
        }
        public static void Main()
        {
            Console.WriteLine("Подсчет в лоб. с автоматическим разбиением потоков.");
            foreach (var (key, value) in ElementToThreadsNumber)
            {
                _array = new double[key];
                _threadCount = value;

                var sw = new Stopwatch();
                sw.Start();

                for (var i = 0; i < key; i++)
                {
                    _array[i] = ArrayDifficultOperation(_array[i]);
                }

                Console.Write($"Consistently - {sw.Elapsed.ToString()} \t");
                sw.Stop();
                sw.Reset();
                _array = new double[key];
                sw.Start();
                Parallel.For(0, _array.Length, new ParallelOptions {MaxDegreeOfParallelism = _threadCount},
                    i =>
                    {
                        _array[i] = i;
                        _array[i] = ArrayDifficultOperation(_array[i]);
                    });
                Console.WriteLine(value: $"Parallel - {sw.Elapsed.ToString()}");
                sw.Stop();
            }

            Console.WriteLine("Нажмите чтобы продолжить подсчет с круговым подсчетом");
            Console.ReadKey();

            foreach (var (key, value) in ElementToThreadsNumber)
            {
                _array = new double[key];
                _threadCount = value;

                var sw = new Stopwatch();
                sw.Start();



                for (var j = 0; j < _array.Length; j++)
                {
                    _array[j] = j;
                    for (var index = 0; index < Coeficient; index++)
                    {
                        _array[j] = ArrayDifficultOperation(_array[j]);
                    }
                }


                Console.Write($"Consistently - {sw.Elapsed.ToString()} \t");
                sw.Stop();
                sw.Reset();
                _array = FillArray(new double[key]);
                sw.Start();
                var threads = new Thread[_threadCount];

                for (var i = 0; i < _threadCount; i++)
                {
                    var workIndex = i;
                    threads[i] = new Thread(RunAroundParallel) {Name = $"{i} Thread"};
                    threads[i].Start(workIndex);
                }

                for (var i = 0; i < _threadCount; i++)
                {
                    threads[i].Join();
                    //Console.WriteLine($"Основной поток: Завершение рабочего потока.{threads[i].Name}");
                }

                sw.Stop();
                Console.WriteLine(value: $"Parallel - {sw.Elapsed.ToString()}");

            }
        }

        private static double[] FillArray(double[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }

            return array;
        }
        private static double ArrayDifficultOperation(double inputElement)
        {
            inputElement *= Math.E;
            inputElement += inputElement % 3;
            inputElement = Math.Log(inputElement);
            return inputElement;
        }

    }
}