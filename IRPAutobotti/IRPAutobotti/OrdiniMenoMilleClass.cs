
namespace IRPAutobotti
{
    public struct OrdiniMenoMilleStruct
    {
        public double[] ordini;
        public double[] ordiniD;
        public double[] ordiniBD;
        public double[] ordiniB95;
        public double[] ordiniBS;
        public double[] ordiniAlpino;
        public double[] ordiniBluAlpino;
        public double[] peso;
    }
    class OrdiniMenoMilleClass
    {
        OrdiniMenoMilleStruct ommStruct;
        public OrdiniMenoMilleClass()
        {
            ommStruct = new OrdiniMenoMilleStruct();
        }

        public OrdiniMenoMilleStruct OrdiniMenoMille(int n_ordini, double MENOMILLE, double[] ordini, double[] ordiniD, double[] ordiniBD, double[] ordiniB95, double[] ordiniBS, double[] ordiniAlpino,
            double[] ordiniBluAlpino, double dens_D, double dens_BD, double dens_BS, double dens_B95)
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
