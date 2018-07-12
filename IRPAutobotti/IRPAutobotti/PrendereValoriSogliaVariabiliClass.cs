using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    public struct PrendereValoriSogliaVariabiliStruct
    {
        public double MENOMILLE;
        public double RIEMPIMENTOMAX;
    }
    class PrendereValoriSogliaVariabiliClass
    {
        public PrendereValoriSogliaVariabiliStruct vsvStruct;
        public PrendereValoriSogliaVariabiliClass()
        {
            vsvStruct = new PrendereValoriSogliaVariabiliStruct();
        }

        public PrendereValoriSogliaVariabiliStruct PrendereValoriSogliaVariabili(int Id, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.[BIS].[getSettingVariabiliById]";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@indice", Id);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();
            
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }

            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            string temp = "";
            foreach (DataRow row in X.Rows)
            {
                temp = row["Soglie"].ToString();
            }
            string[] s = temp.Split(';');
            vsvStruct.MENOMILLE = Convert.ToDouble(s[1]);
            vsvStruct.RIEMPIMENTOMAX = Convert.ToDouble(s[0]);
            return vsvStruct;
        }
    }
}
