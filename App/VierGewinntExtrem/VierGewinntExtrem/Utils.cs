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

        /// <summary>
        /// The string gets concated to it self i times.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <returns>string</returns>
        public static string MulStrInt(string s, int i)
        {
            string temp = string.Empty;

            for (int j = 0; j < i; j++)
            {
                temp += s;
            }
            return temp;
        }

        /// <summary>
        /// Returns if in a string all chars are equal.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool AllEqual(string str)
        {
            for(int i = 1; i < str.Length; i++)
            {
                if(str[i] != str[i-1])
                    return false;
            }
            return true;
        }
    }
}
