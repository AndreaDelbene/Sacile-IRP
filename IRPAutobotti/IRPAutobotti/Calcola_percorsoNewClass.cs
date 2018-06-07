using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    public struct Calcola_percorsoNewStruct
    {
        public int lun;
        public double sequenza;

    }
    class Calcola_percorsoNewClass
    {
        public Calcola_percorsoNewStruct calcola_percorsoNew(int baseCarico, DataTable od_pv_pv_completa,double[] viaggio_temp, sequenza, double lun,int n_viaggio)
        {
            //calcolo indicatori di prestazione
            double[] giro = viaggio_temp;
            int g = giro.Length;
            double[] temp_od_dep_pv = new double[g];
            double[] temp_od_pv_dep = new double[g];
            double[,] temp_od_pv_pv = new double[g, g];
            DataTable X = od_pv_pv_completa;
            //DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            //csStruct.dens_BS = X.AsEnumerable().Select(r => r.Field<double>("densitaBB")).ToArray();

            for (int i=0;i<g;i++)
            {
                temp_od_dep_pv[i]=
            }
        }
    }
}
