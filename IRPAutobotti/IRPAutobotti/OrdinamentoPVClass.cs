using System;
using System.Collections.Generic;
using System.Linq;

namespace IRPAutobotti
{

    public struct OrdinamentoPVStruct
    {
        public double[] MioOrdine_ord;
        public double[] ordini_ord;
        public double[] pv_ord;
        public double[,] valore_ord;
        public double[,] od_pv_pv_ord;
        public double[] od_dep_pv_ord;
        public double[] od_pv_dep_ord;
        public double[] ordinipeso_ord;
        public double[] ordini_piumeno_ord;
        public double[] max_product_ord;
        public double[] ordiniD_ord;
        public double[] ordiniBD_ord;
        public double[] ordiniB95_ord;
        public double[] ordiniBS_ord;
        public double[] ordiniAlpino_ord;
        public double[] ordiniBluAlpino_ord;
        public double[] ordinati_ord;
    }

    class OrdinamentoPVClass
    {
        public OrdinamentoPVStruct opvStruct;

        public OrdinamentoPVClass()
        {
            opvStruct = new OrdinamentoPVStruct();
        }

        public OrdinamentoPVStruct OrdinamentoPV(double[] od_dep_media, double[] pv, double[] ordini, double[,] Valore, double[,] od_pv_pv, int n_ordini,
            double[] od_dep_pv, double[] od_pv_dep, double[] ordinipiumeno, double[] prodottomax, double[] peso, double[] ordiniD, double[] ordiniBD, 
            double[] ordiniB95, double[] ordiniBS, double[] ordiniAlpino, double[] ordiniBluAlpino, double[] MioOrdine , double[] ordinati)
        {

            // "Primo sortrows"
            int[] indexes = Enumerable.Range(0, pv.Length).ToArray();

            Array.Sort(od_dep_media, indexes);
            Array.Reverse(od_dep_media);
            Array.Reverse(indexes);

            double[] pv_ord = indexes.Select(index => pv[index]).ToArray();
            double[] ordini_ord = indexes.Select(index => ordini[index]).ToArray();
            List<double[]> valore_ord = new List<double[]>();
            List<double[]> od_pv_pv_temp = new List<double[]>();

            const int doubleSize = 8;
            double[] temp = new double[Valore.GetLength(0)];
            double[] temp2 = new double[od_pv_pv.GetLength(0)];
            for (int i=0;i<Valore.GetLength(1);i++)
            {
                for(int ii = 0; ii < Valore.GetLength(0); ii++)
                {
                    temp[ii] = Valore[i, ii];
                }
                valore_ord.Add(indexes.Select(index => temp[index]).ToArray().ToArray());
            }
            for (int i = 0; i < od_pv_pv.GetLength(1); i++)
            {
                for (int ii = 0; ii < od_pv_pv.GetLength(0); ii++)
                {
                    temp2[ii] = od_pv_pv[i, ii];
                }
                od_pv_pv_temp.Add(indexes.Select(index => temp2[index]).ToArray().ToArray());
            }

            // "Secondo" sortrows
            // Traspongo valore_ord
            double[,] valore_ord_T = Transpose(valore_ord);
            // Riutilizzo gli indici generati prima e ordino valore_ord_T
            temp = new double[valore_ord_T.GetLength(0)];
            valore_ord.Clear();
            for (int i = 0; i < valore_ord_T.GetLength(1); i++)
            {
                for (int ii = 0; ii < valore_ord_T.GetLength(0); ii++)
                {
                    temp[ii] = Valore[i, ii];
                }
                valore_ord.Add(indexes.Select(index => temp[index]).ToArray().ToArray());
            }
            double[,] valore_ord_Matrix = Transpose(valore_ord);

            // Rioridino anche la trasposta di od_pv_pv_temp
            double[,] od_pv_pv_T = Transpose(od_pv_pv_temp);
            temp2 = new double[od_pv_pv_T.GetLength(0)];
            od_pv_pv_temp.Clear();
            for (int i = 0; i < od_pv_pv_T.GetLength(1); i++)
            {
                for (int ii = 0; ii < od_pv_pv_T.GetLength(0); ii++)
                {
                    temp2[ii] = Valore[i, ii];
                }
                od_pv_pv_temp.Add(indexes.Select(index => temp[index]).ToArray().ToArray());
            }
            double[,] od_pv_pv_ord_Matrix = Transpose(od_pv_pv_temp);

            // Ordino i restanti array sempre secondo l'indice iniziale
            double[] od_dep_pv_ord = indexes.Select(index => od_dep_pv[index]).ToArray();
            double[] od_pv_dep_ord = indexes.Select(index => od_pv_dep[index]).ToArray();
            double[] ordinipeso_ord = indexes.Select(index => peso[index]).ToArray();
            double[] ordini_piumeno_ord = indexes.Select(index => ordinipiumeno[index]).ToArray();
            double[] max_product_ord = indexes.Select(index => prodottomax[index]).ToArray();
            double[] ordiniD_ord = indexes.Select(index => ordiniD[index]).ToArray();
            double[] ordiniBD_ord = indexes.Select(index => ordiniBD[index]).ToArray();
            double[] ordiniB95_ord = indexes.Select(index => ordiniB95[index]).ToArray();
            double[] ordiniBS_ord = indexes.Select(index => ordiniBS[index]).ToArray();
            double[] ordiniAlpino_ord = indexes.Select(index => ordiniAlpino[index]).ToArray();
            double[] ordiniBluAlpino_ord = indexes.Select(index => ordiniBluAlpino[index]).ToArray();
            double[] MioOrdine_ord = indexes.Select(index => MioOrdine[index]).ToArray();
            double[] ordinati_ord = indexes.Select(index => ordinati[index]).ToArray();

            opvStruct.MioOrdine_ord = MioOrdine_ord;
            opvStruct.ordini_ord = ordini_ord;
            opvStruct.pv_ord = pv_ord;
            opvStruct.valore_ord = valore_ord_Matrix;
            opvStruct.od_pv_pv_ord = od_pv_pv_ord_Matrix;
            opvStruct.od_dep_pv_ord = od_dep_pv_ord;
            opvStruct.od_pv_dep_ord = od_pv_dep_ord;
            opvStruct.ordinipeso_ord = ordinipeso_ord;
            opvStruct.ordini_piumeno_ord = ordini_piumeno_ord;
            opvStruct.max_product_ord = max_product_ord;
            opvStruct.ordiniD_ord = ordiniD_ord;
            opvStruct.ordiniBD_ord = ordiniBD_ord;
            opvStruct.ordiniB95_ord = ordiniB95_ord;
            opvStruct.ordiniAlpino_ord = ordiniAlpino_ord;
            opvStruct.ordiniBluAlpino_ord = ordiniBluAlpino_ord;
            opvStruct.ordinati_ord = ordinati_ord;

            return opvStruct;
        }

        public double[,] Transpose(List<double[]> matrix)
        {
            int w = matrix[0].Length;
            int h = matrix.Count;

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i][j];
                }
            }

            return result;
        }

    }
}
