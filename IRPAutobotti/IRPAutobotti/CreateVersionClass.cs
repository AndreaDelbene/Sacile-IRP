using System;
using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class CreateVersionClass
    {
        public CreateVersionClass()
        {
        }

        public int CreateVersion(int baseCarico, string login, string data, int IdSetting, int IdRunner, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();

            comm.CommandText = "Matlab.BIS.createVersionSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_base", baseCarico);
            comm.Parameters.AddWithValue("@login", login);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@idSettingVariabili", IdSetting);
            comm.Parameters.AddWithValue("@id_runner", IdRunner);
            
            comm.Connection = conn;

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            conn.Open();
            DataTable table = new DataTable();
            adapter.Fill(table);
            int IdVersion = (int)(Int64)table.Rows[0]["id"];
            conn.Close();
            return IdVersion;
        }
    }
}
