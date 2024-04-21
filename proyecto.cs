using System;
using System.IO;
using System.Linq;

namespace MyNamespace
{
    class Test_1
    {
        static void Main()
        {
            int[] numbers = new int[10];
            Console.WriteLine("Please enter 10 different numbers:");

            for (int i = 0; i < 10; i++)
            {
                Console.Write($"Enter number {i + 1}: ");
                numbers[i] = Convert.ToInt32(Console.ReadLine());
            }

            int[] sortedByBubbleSort = BubbleSort(numbers);
            int[] sortedByInsertionSort = InsertionSort(numbers);

            File.WriteAllText("sortedNumbers.txt", $"Bubble Sort: {string.Join(", ", sortedByBubbleSort)}\nInsertion Sort: {string.Join(", ", sortedByInsertionSort)}");
            Console.WriteLine("The sorted numbers have been written to sortedNumbers.txt.");
        }
        static int[] BubbleSort(int[] array)
        {
            int[] arr = (int[])array.Clone();
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
            return arr;
        }
        static int[] InsertionSort(int[] array)
        {
            int[] arr = (int[])array.Clone();
            for (int i = 1; i < arr.Length; i++)
            {
                int key = arr[i];
                int j = i - 1;
                while (j >= 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j = j - 1;
                }
                arr[j + 1] = key;
            }
            return arr;
        }
    }
}