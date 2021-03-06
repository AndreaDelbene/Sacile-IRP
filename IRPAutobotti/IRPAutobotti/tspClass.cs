﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace IRPAutobotti
{
    struct TspStruct
    {
        public int[] pi;
        public int lunghezza;
    }
    class tspClass
    {
        TspStruct tspStruct;
        public tspClass()
        {
            tspStruct = new TspStruct();
        }

        public TspStruct tsp(int N, double rho, double alpha, double[,] A, int traj)
        {
            int[] I = new int[N];
            //GetLength(0) ritorna il numero di colonne mentre con 1, il numero di righe
            int n = A.GetLength(1);
            double tol = 0.005;
            int z = 7;
            int[] kk = new int[n];
            for (int i = 0; i < n; i++)
                kk[i] = i;
            int[] st = new int[z];  //inizializza un vettore lungo 7 di zeri
            double[,] Pold = new double[n, n];

            double[,] P = new double[n, n];
            double[,] Ptemp = new double[n, n];
            if (traj == 0)
            {
                //viene generata una matrice che ha la diagonale nulla
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        P[i, j] = 1.0 / (n - 1);
                        if (i == j)
                            Ptemp[i, j] = P[i, j];
                        else
                            Ptemp[i, j] = 0;
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Pold[i, j] = 0;
                        P[i, j] = P[i, j] - Ptemp[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        P[i, j] = 1.0 / (n);
            }

            int count = 1;
            List<int[]> X = new List<int[]>();
            List<int[]> Y = new List<int[]>();
            int[] S = new int[N];
            int g = 0;
            gtsp0Class gtsp0 = new gtsp0Class();
            gtsp1Class gtsp1 = new gtsp1Class();
            stspClass stsp = new stspClass();
            while (Max(AbsOfDifference(P,Pold)) > tol)
            {
                for (int i = 0; i < N; i++)
                {
                    if (traj == 0)
                    {
                        X.Insert(i, (int[])gtsp0.gtsp0(P).Clone());
                    }
                    else
                    {
                        X.Insert(i, (int[])gtsp1.gtsp1(P).Clone());
                    }


                    int[] x = new int[n];
                    if(traj == 0)
                    {
                        S[i] = stsp.stsp(X[i], A);
                    }
                    else
                    {
                        int[] y =(int[]) X[i].Clone();
                        int ki = 0;
                        for(int zi = 0; zi < n; zi++)
                        {
                            ki = y[zi];
                            x[ki] = zi;
                        }
                        Y.Insert(i, (int[])x.Clone());
                        S[i] = stsp.stsp(x,A);
                    }
                }

                I = Enumerable.Range(0, S.Length).ToArray();  // Indexes of S
                Array.Sort(S, I);
                g = (int)Math.Floor(rho * N);
                double[,] w = new double[n, n];
                if (traj == 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            double ss = 0.0;
                            for (int k = 0; k < g; k++)
                            {
                                int pos = search(X, I[k], i);
                                int ii = kk[pos];
                                if (ii < n - 1)
                                {
                                    
                                    if (X[I[k]][ii + 1] == j)
                                        ss++;
                                }
                                else
                                {
                                    if (X[I[k]][0] == j)
                                        ss++;
                                }
                            }
                            w[i, j] = (double)ss / (double)g;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            int ss = 0;
                            for (int k = 0; k < g; k++)
                            {
                                if(Y[I[k]][i] == j)
                                    ss++;
                            }
                            w[i, j] = (double)ss / (double)g;
                        }
                    }
                }
                Pold = (double[,])P.Clone();
                P = (double[,])calcP(alpha,w,(1 - alpha),P).Clone();
                // slittamento del vettore di una posizione in alto rimuovendo la prima posizione ed aggiungendo nuovamente l'ultima
                int[] temp = (int[])st.Clone();
                for (int i = 0; i < st.Length-1; i++)
                {
                    st[i] = temp[i + 1];
                }
                st[st.Length - 1] = S[g];

                int c = st.Where(p => p == 7).Count();
                if (c == 7)
                    break;
                count++;
            }
            int[] pi = new int[X.Capacity];
            if(traj == 0)
            {
                pi = (int[])X[I[0]].Clone();
            }
            else
            {
                pi = (int[])Y[I[0]].Clone();
            }
            tspStruct.lunghezza = S[g];
            tspStruct.pi = (int[])pi.Clone();
            return tspStruct;
        }

        private double[,] calcP(double alpha, double[,] w, double v, double[,] p)
        {
            int r = p.GetLength(1);
            int c = p.GetLength(0);
            double[,] result = new double[r,c];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    result[i, j] = (alpha * w[i, j]) + (v * p[i, j]);
            return result;
        }

        private int search(List<int[]> X, int I, int i)
        {
            for(int j = 0; j < X[I].Length; j++)
            {
                if (X[I][j] == i)
                    return j;
            }
            return -1;
        }

        private double Max(double[,] matrix)
        {
            double max = 0;
            for (int i = 0; i < matrix.GetLength(1); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (max < matrix[i, j])
                        max = matrix[i, j];
            return max;
        }

        private double[,] AbsOfDifference(double[,] P, double[,] Pold)
        {
            double[,] result = new double[P.GetLength(1), P.GetLength(0)];
            for (int i = 0; i < P.GetLength(1); i++)
            {
                for (int j = 0; j < P.GetLength(0); j++)
                {
                    result[i, j] = P[i, j] - Pold[i, j];
                    if (result[i, j] < 0)
                        result[i, j] *= -1;
                }
            }
            return result;
        }
    }
}
