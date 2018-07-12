
namespace IRPAutobotti
{
    class stspClass
    {
        public stspClass()
        {
            // Do nothing
        }
        public double stsp(int[] x, double[,] A)
        {
            double s = 0;
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
