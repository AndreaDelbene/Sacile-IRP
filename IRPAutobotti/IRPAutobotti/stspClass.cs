
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
                s += (int) A[x[i], x[i + 1]];
            }
            s += (int) A[x[size], x[1]];
            return s;
        }
    }
}
