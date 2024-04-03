using System.Collections.Generic;


namespace YokaiNoMori.General
{
    public static class Tools
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random randomGenerator = new System.Random();

            int elementsMaxIndex = list.Count;
            while (elementsMaxIndex > 1)
            {
                elementsMaxIndex--;
                int changePosition = randomGenerator.Next(elementsMaxIndex + 1);

                T swapValue = list[changePosition];
                list[changePosition] = list[elementsMaxIndex];
                list[elementsMaxIndex] = swapValue;
            }
        }

        public static bool IsBetween(this int number, int min, int max, bool isExclusive = true)
        {
            return isExclusive ? (number >= min && number < max) : (number >= min && number <= max);
        }
    }
}
