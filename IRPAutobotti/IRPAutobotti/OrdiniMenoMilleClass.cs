using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    public struct OrdiniMenoMilleStruct
    {
        public int[] ordini;
        public int[] ordiniD;
        public int[] ordiniBD;
        public int[] ordiniB95;
        public int[] ordiniBS;
        public int[] ordiniAlpino;
        public int[] ordiniBluAlpino;
        public double[] peso;
    }
    class OrdiniMenoMilleClass
    {
        OrdiniMenoMilleStruct ommStruct;
        public OrdiniMenoMilleClass()
        {
            ommStruct = new OrdiniMenoMilleStruct();
        }

        public OrdiniMenoMilleStruct OrdiniMenoMille(int n_ordini, int MENOMILLE, int[] ordini, int[] ordiniD, int[] ordiniBD, int[] ordiniB95, int[] ordiniBS, int[] ordiniAlpino, int[] ordiniBluAlpino, double dens_D, double dens_BD, double dens_BS, double dens_B95)
        {
            double[] peso = new double[n_ordini];
            for(int i = 0; i < n_ordini; i++)
            {
                if (ordiniD[i] < MENOMILLE)
                    ordiniD[i]--;
                if (ordiniBD[i] < MENOMILLE)
                    ordiniBD[i]--;
                if (ordiniB95[i] < MENOMILLE)
                    ordiniB95[i]--;
                if (ordiniBS[i] < MENOMILLE)
                    ordiniBS[i]--;
                if (ordiniAlpino[i] < MENOMILLE)
                    ordiniAlpino[i]--;
                if (ordiniBluAlpino[i] < MENOMILLE)
                    ordiniBluAlpino[i]--;

                // calcolo il peso di ciascun ordine, usando le densità
                peso[i] = (ordiniD[i] * dens_D) + (ordiniBD[i] * dens_BD) + (ordiniB95[i] * dens_B95) + (ordiniBS[i] * dens_BS) + (ordiniAlpino[i] * dens_D) + (ordiniBluAlpino[i] * dens_BD);

                // ricalcolo la quantita degli ordini tenendo conto dei meno mille che ho fatto
                ordini[i] = ordiniD[i] + ordiniBD[i] + ordiniBS[i] + ordiniB95[i] + ordiniAlpino[i] + ordiniBluAlpino[i];
            }

            ommStruct.ordini = ordini;
            ommStruct.ordiniD = ordiniD;
            ommStruct.ordiniBD = ordiniBD;
            ommStruct.ordiniB95 = ordiniB95;
            ommStruct.ordiniBS = ordiniBS;
            ommStruct.ordiniAlpino = ordiniAlpino;
            ommStruct.ordiniBluAlpino = ordiniBluAlpino;
            ommStruct.peso = peso;

            return ommStruct;
        }
    }
}
