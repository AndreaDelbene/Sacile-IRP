using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    class stspClass
    {
        public stspClass()
        {
            // Do nothing
        }
        public int stsp(int[] x, double[,] A)
        {
            int s = 0;
            int size = A.Length;
            for(int i = 0; i < size - 1; i++)
            {
                s += A[x[i], x[i + 1]];
            }
            s += A[x[size], x[1]];
            return s;
        }
    }
}
