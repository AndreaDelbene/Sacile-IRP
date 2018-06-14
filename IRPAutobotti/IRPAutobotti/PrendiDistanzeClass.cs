using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{

    public struct PrendiDistanzeStruct
    {
        public DataTable od_pv_pv_completa;
        od_dep_pv,od_pv_dep,od_pv_pv,preferenza_pv_pv
    }
    class PrendiDistanzeClass
    {
        public PrendiDistanzeStruct pdStruct;

        public PrendiDistanzeClass()
        {
            pdStruct = new PrendiDistanzeStruct();
        }

        public PrendiDistanzeStruct PrendiDistanze(int baseCarico, string data, int n_ordini, int[] pv, preferenze, SqlConnection conn)
        {
            string p = "{call TIP.BIS._FRA_GetTabDistance(" + baseCarico + "," + data + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            pdStruct.od_pv_pv_completa = X;

            int[] od_dep_pv = new int[n_ordini];
            int[] od_pv_dep = new int[n_ordini];
            for(int i = 0; i < n_ordini; i++)
            {
                string exp1 = "p1 = " + baseCarico + " and p2 = " + pv[i];
                string exp2 = "p1 = " + pv[i] + " and p2 = " + baseCarico;
                double[] temp1 = X.AsEnumerable().Select(r => r.Field<double>(exp1)).ToArray();
                temp_od_dep_pv[i] = temp1[3];
                double[] temp2 = X.AsEnumerable().Select(r => r.Field<double>(exp2)).ToArray();
                temp_od_pv_dep[i] = temp2[3];
                int[,] od_pv_pv = new int[n_ordini,n_ordini];
                for(int j = 0; j < n_ordini; j++)
                {
                    if (pv[i] == pv[j])
                    {
                        od_pv_pv[i, j] = 0;
                    }
                    else
                    {
                        try
                        {
                            string exp = "p1 = " + pv[i] + " and p2 = " + pv[j];
                            int[] temp = X.AsEnumerable().Select(r => r.Field<int>(exp)).ToArray();
                            od_pv_pv[i, j] = temp[3];
                        }
                        catch (Exception e)
                        {
                            od_pv_pv[i, j] = 1000;
                        }
                    }
                }
                for(int i = 0; i < n_ordini; i++)
                {
                    for(int j = 0; j < n_ordini; j++)
                    {

                    }
                }
            }
            return pdStruct;
        }
    }
}
