
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
            int size = A.GetLength(0);
            for(int i = 0; i < size - 1; i++)
            {
                s += (int) A[x[i], x[i + 1]];
            }
            s += (int) A[x[size - 1], x[0]];
            return s;
        }
    }
}
