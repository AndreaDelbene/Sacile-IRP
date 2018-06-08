using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{

    public struct OrdinamentoPVStruct
    {
        public double[,] MioOrdine_ord;
        public double[,] ordini_ord;
        public double[,] pv_ord;
        public double[,] valore_ord;
        public double[,] od_pv_pv_ord;
        public double[,] od_dep_pv_ord;
        public double[,] od_pv_dep_ord;
        public double[,] ordinipeso_ord;
        public double[,] ordini_piumeno_ord;
        public double[,] max_product_ord;
        public double[,] ordiniD_ord;
        public double[,] ordiniBD_ord;
        public double[,] ordiniB95_ord;
        public double[,] rdiniBS_ord;
        public double[,] ordiniAlpino_ord;
        public double[,] ordiniBluAlpino_ord;
        public double[,] ordinati_ord;
    }

    class OrdinamentoPVClass
    {
        public OrdinamentoPVStruct opvStruct;
        public OrdinamentoPVClass()
        {
            opvStruct = new OrdinamentoPVStruct();
        }

        public OrdinamentoPVStruct OrdinamentoPV(double[] od_dep_media, pv, int[] ordini, double[,] Valore, double[][] od_pv_pv, int n_ordini, double[] od_dep_pv, double[] od_pv_dep, ordinipiumeno, prodottomax, double[] peso, double[] ordiniD, double[] ordiniBD, double[] ordiniB95, double[] ordiniBS, double[] ordiniAlpino, double[] ordiniBluAlpino, MioOrdine , ordinati)
        {


            return opvStruct;
        }
    }
}
