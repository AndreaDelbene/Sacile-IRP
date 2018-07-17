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

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            conn.Open();
            DataTable table = new DataTable();
            adapter.Fill(table);

            pdStruct.od_pv_pv_completa = table;

            double[] od_dep_pv = new double[n_ordini];
            double[] od_pv_dep = new double[n_ordini];

            double[] temp_od_dep_pv = new double[n_ordini];
            double[] temp_od_pv_dep = new double[n_ordini];

            double[,] od_pv_pv = new double[n_ordini, n_ordini];

            for (int i = 0; i < n_ordini; i++)
            {
                string exp1 = "p1 = " + baseCarico + " and p2 = " + pv[i];
                string exp2 = "p1 = " + pv[i] + " and p2 = " + baseCarico;
                DataRow[] datarowTemp1 = table.Select(exp1);
                DataRow[] datarowTemp2 = table.Select(exp2);

                temp_od_dep_pv[i] = (double)(decimal)datarowTemp1[0][2];
                temp_od_pv_dep[i] = (double)(decimal)datarowTemp2[0][2];

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
                            DataRow[] datarowTemp = table.Select(exp);
                            od_pv_pv[i, j] = (double)(decimal)datarowTemp[0][2];
                        }
                        catch (Exception e)
                        {
                            od_pv_pv[i, j] = 1000;
                        }
                    }
                }
            }

            double[,] preferenza_pv_pv = new double[n_ordini,n_ordini];
            for (int i = 0; i < n_ordini; i++)
            {
                for (int j = 0; j < n_ordini; j++)
                {
                    string exp = "p1 = " + pv[i] + " and p2 = " + pv[j];
                    DataRow[] datarowTemp = preferenze.Select(exp);
                    if(datarowTemp.Length == 0)
                    {
                        preferenza_pv_pv[i, j] = 0;
                    }
                    else
                    {
                        preferenza_pv_pv[i, j] = (double)(int)datarowTemp[0][2];
                    }
                }
            }

            conn.Close();

            pdStruct.od_dep_pv = temp_od_dep_pv;
            pdStruct.od_pv_dep = temp_od_pv_dep;
            pdStruct.od_pv_pv = od_pv_pv;
            pdStruct.preferenza_pv_pv = preferenza_pv_pv;
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
