using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace IRPAutobotti
{
    public struct Calcola_percorsoNewStruct
    {
        public List<double> lun;
        public List<List<double>> sequenza;
        public int n_viaggio;
    }
    class Calcola_percorsoNewClass
    {
        Calcola_percorsoNewStruct cpnStruct;
        public Calcola_percorsoNewClass()
        {
            cpnStruct = new Calcola_percorsoNewStruct();
        }

        public Calcola_percorsoNewStruct calcola_percorsoNew(int baseCarico, DataTable od_pv_pv_completa, List<double> viaggio_temp,
            List<List<double>> sequenza, List<double> lun,int n_viaggio)
        {
            //assegno le variabili in Input a quelle nella struct di ritorno
            cpnStruct.lun = lun;
            cpnStruct.sequenza = sequenza;
            cpnStruct.n_viaggio = n_viaggio;
            //calcolo indicatori di prestazione
            List<double> giro = viaggio_temp;
            int g = giro.Count;
            double[] temp_od_dep_pv = new double[g];
            double[] temp_od_pv_dep = new double[g];
            double[,] temp_od_pv_pv = new double[g, g];
            //prendo la terza colonna della table od_pv_pv_completa
            DataTable X = od_pv_pv_completa;
            //DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            //csStruct.dens_BS = X.AsEnumerable().Select(r => r.Field<double>("densitaBB")).ToArray();
            
            for (int i=0;i<g;i++)
            {
                string exp1 = "p1 = " + baseCarico + " and p2 = " + giro[i];
                string exp2 = "p1 = " + giro[i] + " and p2 = " + baseCarico;
                DataRow[] datarow_temp = X.Select(exp1).ToArray();
                foreach (DataRow r in datarow_temp)
                {
                    temp_od_dep_pv[i] = (double)(decimal)r[2];
                }
                datarow_temp = X.Select(exp2).ToArray();
                foreach (DataRow r in datarow_temp)
                {
                    temp_od_pv_dep[i] = (double)(decimal)r[2];
                }
                for(int j=0;j<g;j++)
                {
                    if(giro[i]==giro[j])
                        temp_od_pv_pv[i, j] = 0;
                    else
                    {
                        string exp3 = "p1 = " + giro[i] + " and p2 = " + giro[j];
                        datarow_temp = X.Select(exp3).ToArray();
                        foreach (DataRow r in datarow_temp)
                        {
                            temp_od_pv_pv[i, j] = (double)(decimal)r[2];
                        }
                    }
                }

            }
            /*od=[0 temp_od_dep_pv;
            temp_od_pv_dep' temp_od_pv_pv];

            dichiaro od come una (g+1)*(g+1)*/

            double[,] od = new double[g + 1, g + 1];
            for(int i=0;i<g+1;i++)
            {
                for(int j=0;j<g+1;j++)
                {
                    if (i == 0 && j == 0)
                        //se sono nella posizione [0,0] metto lo zero
                        od[i, j] = 0;
                    else if (i == 0)
                        //se invece mi muovo lungo la riga 0, prendo l'elemento j-1 esimo dal vettore 
                        od[i, j] = temp_od_dep_pv[j-1];
                    else if (j == 0)
                        //se invece mi muovo lungo la colonna 0, prendo l'elemento j-1 esimo dal vettore 
                        od[i, j] = temp_od_pv_dep[i-1];
                    else
                        //altrimenti prendo gli elementi della matrice
                        od[i, j] = temp_od_pv_pv[i-1, j-1];
                }
            }
            System.Console.WriteLine("Enter tsp in percorsoNew");
            TspStruct returnStruct = new TspStruct();
            tspClass t = new tspClass();
            returnStruct = t.tsp(1000, 0.05, 0.8, od, 0);
            System.Console.WriteLine("Exit tsp in percorsoNew");

            int[] tour = returnStruct.pi;
            cpnStruct.lun[n_viaggio] = returnStruct.lunghezza;
            for(int i=0;i<tour.Length;i++)
            {
                if(tour[i]==1)
                    cpnStruct.sequenza[n_viaggio][i] = baseCarico;
                else
                    cpnStruct.sequenza[n_viaggio][i] = giro[tour[i]-1];
            }
            return cpnStruct;
        }
    }
}
