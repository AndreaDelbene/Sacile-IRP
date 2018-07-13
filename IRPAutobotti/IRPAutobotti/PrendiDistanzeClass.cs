using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{

    public struct PrendiDistanzeStruct
    {
        public DataTable od_pv_pv_completa;
        public double[] od_dep_pv;
        public double[] od_pv_dep;
        public double[,] od_pv_pv;
        public double[,] preferenza_pv_pv;
    }
    class PrendiDistanzeClass
    {
        public PrendiDistanzeStruct pdStruct;

        public PrendiDistanzeClass()
        {
            pdStruct = new PrendiDistanzeStruct();
        }

        public PrendiDistanzeStruct PrendiDistanze(int baseCarico, string data, int n_ordini, double[] pv, DataTable preferenze, SqlConnection conn)
        {

            // connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS._FRA_GetTabDistance";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_base", baseCarico);
            comm.Parameters.AddWithValue("@data", data);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();

            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            pdStruct.od_pv_pv_completa = X;

            double[] od_dep_pv = new double[n_ordini];
            double[] od_pv_dep = new double[n_ordini];

            double[] temp_od_dep_pv = new double[n_ordini];
            double[] temp_od_pv_dep = new double[n_ordini];

            for (int i = 0; i < n_ordini; i++)
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
            }
            for (int i = 0; i < n_ordini; i++)
            {
                for (int j = 0; j < n_ordini; j++)
                {

                }
            }

            reader.Close();
            conn.Close();
            return pdStruct;
        }

        private int[] find(int[,] matrix, int condition1, int condition2, int n_max_element)
        {
            int counter = 0;
            int position = 0;
            List<int> result = new List<int>();
            for(int i = 0; i < matrix.GetLength(1); i++)
            {
                for(int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[i, j] == condition1 && matrix[i, j] == condition2)
                    {
                        if (counter < n_max_element)
                        {
                            result.Add(position);
                        }
                        position++;
                    }
                }
            }
            return result.ToArray();
        }
    }
}
