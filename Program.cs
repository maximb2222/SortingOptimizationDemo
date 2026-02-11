using System;
using System.Diagnostics;

namespace SortingOptimizationDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int size = 10000;
            int seed = 42;

            Console.WriteLine("=== Sorting algorithms demo (.NET 8) ===");
            Console.WriteLine($"Array size: {size}\n");

            int[] baseArray = CreateRandomArray(size, seed);

            RunSortTest("Bubble (basic)", BubbleSortBasic, baseArray);     // <-- алгоритм, выбранный для оптимизации
            RunSortTest("Selection sort", SelectionSort, baseArray);
            RunSortTest("Insertion sort", InsertionSort, baseArray);
            RunSortTest("Quick sort", QuickSortWrapper, baseArray);
            RunSortTest("Merge sort", MergeSortWrapper, baseArray);

            Console.WriteLine("\n--- Optimized algorithm ---");
            RunSortTest("Bubble (optimized)", BubbleSortOptimized, baseArray);

            Console.WriteLine("\nDone. Press Enter to exit.");
            Console.ReadLine();
        }

        // ---------- Общий тестовый хелпер ----------

        private static void RunSortTest(string name, Action<int[]> sort, int[] original)
        {
            int[] data = (int[])original.Clone();

            var sw = Stopwatch.StartNew();
            sort(data);
            sw.Stop();

            bool sorted = IsSorted(data);

            Console.WriteLine($"{name,-20} | {sw.ElapsedMilliseconds,6} ms | Sorted: {sorted}");
        }

        private static int[] CreateRandomArray(int size, int seed)
        {
            var rnd = new Random(seed);
            var arr = new int[size];
            for (int i = 0; i < size; i++)
                arr[i] = rnd.Next();
            return arr;
        }

        private static bool IsSorted(int[] data)
        {
            for (int i = 1; i < data.Length; i++)
            {
                if (data[i - 1] > data[i])
                    return false;
            }
            return true;
        }

        // ---------- НЕОПТИМАЛЬНЫЕ / БАЗОВЫЕ СОРТИРОВКИ ----------

        // Пузырьковая сортировка (базовая, неэффективная версия)
        // Выбрана как алгоритм для оптимизации.
        private static void BubbleSortBasic(int[] data)
        {
            int n = data.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    if (data[j] > data[j + 1])
                    {
                        (data[j], data[j + 1]) = (data[j + 1], data[j]);
                    }
                }
            }
        }

        // Сортировка выбором (Selection sort)
        private static void SelectionSort(int[] data)
        {
            int n = data.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (data[j] < data[minIndex])
                        minIndex = j;
                }

                if (minIndex != i)
                {
                    (data[i], data[minIndex]) = (data[minIndex], data[i]);
                }
            }
        }

        // Сортировка вставками (Insertion sort)
        private static void InsertionSort(int[] data)
        {
            int n = data.Length;
            for (int i = 1; i < n; i++)
            {
                int key = data[i];
                int j = i - 1;

                while (j >= 0 && data[j] > key)
                {
                    data[j + 1] = data[j];
                    j--;
                }

                data[j + 1] = key;
            }
        }

        // Быстрая сортировка (простая рекурсивная версия)
        private static void QuickSortWrapper(int[] data)
        {
            QuickSort(data, 0, data.Length - 1);
        }

        private static void QuickSort(int[] data, int left, int right)
        {
            if (left >= right)
                return;

            int pivot = data[(left + right) / 2];

            int i = left;
            int j = right;

            while (i <= j)
            {
                while (data[i] < pivot) i++;
                while (data[j] > pivot) j--;

                if (i <= j)
                {
                    (data[i], data[j]) = (data[j], data[i]);
                    i++;
                    j--;
                }
            }

            if (left < j)
                QuickSort(data, left, j);
            if (i < right)
                QuickSort(data, i, right);
        }

        // Сортировка слиянием (Merge sort, базовая версия)
        private static void MergeSortWrapper(int[] data)
        {
            int[] temp = new int[data.Length];
            MergeSort(data, temp, 0, data.Length - 1);
        }

        private static void MergeSort(int[] data, int[] temp, int left, int right)
        {
            if (left >= right)
                return;

            int mid = (left + right) / 2;

            MergeSort(data, temp, left, mid);
            MergeSort(data, temp, mid + 1, right);
            Merge(data, temp, left, mid, right);
        }

        private static void Merge(int[] data, int[] temp, int left, int mid, int right)
        {
            int i = left;
            int j = mid + 1;
            int k = left;

            while (i <= mid && j <= right)
            {
                if (data[i] <= data[j])
                {
                    temp[k++] = data[i++];
                }
                else
                {
                    temp[k++] = data[j++];
                }
            }

            while (i <= mid)
                temp[k++] = data[i++];

            while (j <= right)
                temp[k++] = data[j++];

            for (int t = left; t <= right; t++)
                data[t] = temp[t];
        }

        // ---------- ОПТИМИЗИРОВАННЫЙ АЛГОРИТМ (ПУЗЫРЬКОВАЯ СОРТИРОВКА) ----------

        // Оптимизированная пузырьковая сортировка:
        // - выходим раньше, если не было обменов
        // - каждый проход сокращает эффективную длину массива
        private static void BubbleSortOptimized(int[] data)
        {
            int n = data.Length;

            bool swapped;
            do
            {
                swapped = false;

                for (int i = 1; i < n; i++)
                {
                    if (data[i - 1] > data[i])
                    {
                        (data[i - 1], data[i]) = (data[i], data[i - 1]);
                        swapped = true;
                    }
                }

                n--;
            }
            while (swapped);
        }
    }
}
