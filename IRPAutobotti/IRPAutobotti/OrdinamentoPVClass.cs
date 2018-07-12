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
            int[] indexes = Enumerable.Range(0, pv.Length).ToArray();

            Array.Sort(od_dep_media, indexes);
            Array.Reverse(od_dep_media);
            Array.Reverse(indexes);
            double[] pv_temp = indexes.Select(index => pv[index]).ToArray();
            double[] ordini_temp = indexes.Select(index => ordini[index]).ToArray();
            List<List<double>> valore_temp = new List<List<double>>();
            List<List<double>> od_pv_pv_temp = new List<List<double>>();

            for (int i=0;i<Valore.GetLength(1);i++)
            {
                valore_temp[i].Add(indexes.Select(index => Valore[i, index]).ToArray());
            }
            for (int i = 0; i < od_pv_pv.GetLength(1); i++)
            {
                od_pv_pv_temp[i].Add(indexes.Select(index => od_pv_pv[i, index]).ToArray());
            }

            opvStruct.MioOrdine_ord =;
            opvStruct.ordini_ord = ;
            opvStruct.pv_ord =;
            opvStruct.valore_ord =;
            opvStruct.od_pv_pv_ord =;
            opvStruct.od_dep_pv_ord =;
            opvStruct.od_pv_dep_ord =;
            opvStruct.ordinipeso_ord =;
            opvStruct.ordini_piumeno_ord =;
            opvStruct.max_product_ord =;
            opvStruct.ordiniD_ord =;
            opvStruct.ordiniBD_ord =;
            opvStruct.ordiniB95_ord =;
            opvStruct.ordiniAlpino_ord =;
            opvStruct.ordiniBluAlpino_ord =;
            opvStruct.ordinati_ord =;

            return opvStruct;
        }
    }
}
