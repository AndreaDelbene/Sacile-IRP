using System;

namespace IRPAutobotti
{
    public struct AssegnazionePrioritaStruct
    {
        public double[,] p;
        public double[,] Valore;
        public double[] od_dep_media;
    }

    class AssegnazionePrioritaClass
    {
        AssegnazionePrioritaStruct apStruct;
        public AssegnazionePrioritaClass()
        {
            apStruct = new AssegnazionePrioritaStruct();
        }

        public AssegnazionePrioritaStruct AssegnazionePriorita(double[] od_dep_pv, double[] od_pv_dep, double[,] od_pv_pv, int n_ordini, double[] peso, 
            double maxcap, double[] ordini, int esponente, double ELLISSE, double beta, double[,] preferenza_pv_pv, int DISTANZA_MAX_PVPV)
        {
            double[,] p = new double[n_ordini,n_ordini];
            double[,] Valore = new double[n_ordini,n_ordini];
            double[] od_dep_media = new double[od_dep_pv.Length];
            for (int i=0;i<od_dep_pv.Length;i++)
            {
                od_dep_media[i]= (od_dep_pv[i] + od_pv_dep[i]);
            }
           
            object[] returnObj = new object[3];
            for (int i=0;i<n_ordini;i++)
            {
                for(int j=0;j<n_ordini;j++)
                {
                    //normalizzo la coppia per la quantità max di un mezzo tipico
                    p[i, j] = (peso[i] + peso[j]) / (maxcap);
                    //p[i,j] = (ordini[i] + ordini[j]) / (capmax);
                    if(p[i, j]>1 || (ordini[i] + ordini[j])>40)
                    {
                        //i due pv nn sono accoppiati bene quindi "li disaccoppio"
                        p[i, j] = 0;
                    }
                    //Valore è la matrice di quanto sta bene la coppiata del pv i e pv j
                    Valore[i, j] = -1;
                    if(od_pv_pv[i, j]>0)
                    {
                        Valore[i, j] = p[i, j] / (Math.Pow(od_pv_pv[i, j],esponente));
                        if(((od_dep_pv[j]+od_pv_pv[i,j])/od_pv_dep[i])<=ELLISSE)
                        {
                            Valore[i, j] = Valore[i, j] * 1.1;
                        }
                        if(od_pv_pv[i,j]<=DISTANZA_MAX_PVPV)
                        {
                            Valore[i, j] = Valore[i, j] * 2;
                        }
                        if(od_pv_dep[i]<=od_pv_pv[i,j])
                        {
                            Valore[i, j] = Valore[i, j] * 0.1;
                        }
                        if(((od_dep_pv[i]+od_pv_pv[i,j])/od_pv_dep[i])>2)
                        {
                            Valore[i, j] = Valore[i, j] * 0.01;
                        }
                        if(Valore[i,j]>0)
                        {
                            Valore[i,j]=Valore[i,j]*(0.1+(beta*preferenza_pv_pv[i,j]));
                        }
                    }
                }
            }
            apStruct.p = p;
            apStruct.Valore = Valore;
            apStruct.od_dep_media = od_dep_media;
            return apStruct;
        }
    }
}
