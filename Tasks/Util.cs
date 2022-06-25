using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksFEE
{
    internal static class Util
    {
        public static string sp = "----------------";
        public static string nwl = Environment.NewLine;
        public static long LCM(int[] element_array)
        {
            long lcm_of_array_elements = 1;
            int divisor = 2;
            while (true)
            {
                int counter = 0;
                bool divisible = false;
                for (int i = 0; i < element_array.Length; i++)
                {
                    if (element_array[i] == 0)
                    {
                        return 0;
                    }
                    else if (element_array[i] < 0)
                    {
                        element_array[i] = element_array[i] * (-1);
                    }
                    if (element_array[i] == 1)
                    {
                        counter++;
                    }
                    if (element_array[i] % divisor == 0)
                    {
                        divisible = true;
                        element_array[i] = element_array[i] / divisor;
                    }
                }
                if (divisible)
                {
                    lcm_of_array_elements *= divisor;
                }
                else
                {
                    divisor++;
                }
                if (counter == element_array.Length)
                {
                    return lcm_of_array_elements;
                }
            }
        }

        public static string Multiply(this string source, int multiplier)
        {
            StringBuilder sb = new StringBuilder(multiplier * source.Length);
            for (int i = 0; i < multiplier; i++)
            {
                sb.Append(source);
            }

            return sb.ToString();
        }
    }

}
