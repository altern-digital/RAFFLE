using System.Collections.Generic;

public static class ShuffleLib
{
    public static void ShuffleArray<T>(List<T> array)
    {
        System.Random rand = new System.Random();
        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    public static void ShuffleList<T>(List<T> list)
    {
        System.Random rand = new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}