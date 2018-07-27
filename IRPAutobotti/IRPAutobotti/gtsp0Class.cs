using System;

namespace IRPAutobotti
{
    class gtsp0Class
    {
        public gtsp0Class()
        {
        }

        public int[] gtsp0(double[,] Pk)
        {
            int n = Pk.Length; //n==m
            int[] x = new int[n];
            double[,] Pkp = Pk;
            x[1] = 1;
            Random rnd = new Random();
            for (int k=0;k<n-1;k++)
            {
                for(int j=0;j<n;j++)
                {
                    Pkp[j, (int)x[k]] = 0;
                }

                for(int i=0;i<n;i++)
                {
                    double temp = 0;
                    for(int l=0;l<n;l++)
                    {
                        temp = temp + Pkp[i, l];
                    }
                    if(temp!=0)
                    {
                        for (int l = 0; l < n; l++)
                        {
                            Pkp[i, l] = Pkp[i, l] / temp;
                        }
                    }
                }
                //creo un numero casuale tra 0 e 1
                double r = rnd.Next(1);

                double cc = 0;
                int c = -1;
                while (r > cc)
                {
                    c++;
                    cc = cc + Pk[(int)x[k], c];
                }
                x[k + 1] = c;
                Pk = Pkp;
            }
            return x;
        }
    }
}
