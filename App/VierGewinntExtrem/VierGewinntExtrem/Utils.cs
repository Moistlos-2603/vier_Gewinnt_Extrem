namespace Utils
{
    using System.Runtime.CompilerServices;
    public static class Util
    {
        /// <summary>
        /// Sets all indecies of an array to the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArraySetValue<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static string MulStrInt(string s, int i)
        {
            string temp = string.Empty;

            for (int j = 0; j < i; j++)
            {
                temp += s;
            }
            return temp;
        }

    }
}
