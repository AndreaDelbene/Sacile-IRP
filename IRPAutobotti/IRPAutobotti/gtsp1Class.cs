using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    class gtsp1Class
    {
        public int[] gtsp1(double[,] Pkp)
        {
            int n = Pkp.Length;//n==m
            int[] x = new int[n];
            int k = 1;
            Random rnd = new Random();
            while (k<n)
            {
                double r = rnd.Next(1);
                double cc = 0;
                int c = 0;
                while(r>cc)
                {
                    c++;
                    if(c<n)
                        cc = cc + Pkp[k, c];
                    else
                    {
                        cc = cc + Pkp[k, n];
                        break;
                    }
                }
                if (c <= n)
                    x[k] = c;
                else
                    x[k] = n;
                k++;

                double ss = 0;
                for(int j=0;j<n;j++)
                {
                    int count = x.Where(p => p == j + 1).Count();
                    if(count==1)
                    {
                        ss = ss + Pkp[k, j];
                        Pkp[k, j] = 0;
                    }
                }

                for(int j=0;j<n;j++)
                {
                    int count = x.Where(p => p == j + 1).Count();
                    if(count==0)
                    {
                        if (ss < 1)
                            Pkp[k, j] = Pkp[k, j] / (1 - ss);
                        else
                            Pkp[k, j] = 0;
                    }
                }
            }
            for(int j=0;j<n;j++)
            {
                int count = x.Where(p => p == j + 1).Count();
                if(count==0)
                {
                    x[n] = j;
                    break;
                }
            }
            return x;
        }
    }
}
