using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{

    public struct PrendiPreferenzeStruct
    {
        public DataTable preferenze;
    }
    class PrendiPreferenzeClass
    {
        PrendiPreferenzeStruct ppStruct;
        public PrendiPreferenzeClass()
        {
            ppStruct = new PrendiPreferenzeStruct();
        }
        public PrendiPreferenzeStruct PrendiPreferenze(int baseCarico, SqlConnection conn)
        {

            //connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.getPreferenzePuntiVendita";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@base", baseCarico);
            comm.Connection = conn;

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            conn.Open();
            DataTable table = new DataTable();
            adapter.Fill(table);

            ppStruct.preferenze = table;
            conn.Close();
            return ppStruct;
        }
    }
}
