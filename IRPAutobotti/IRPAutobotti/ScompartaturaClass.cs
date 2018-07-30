using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class ScompartaturaClass
    {
        public int Scompartatura(int IdM, string Quantita, double MENOMILLE, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();
            comm.CommandText = "Matlab.BIS.checkScompartiMezzo";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@idMezzo", IdM);
            comm.Parameters.AddWithValue("@stringaProdotti", Quantita);
            comm.Parameters.AddWithValue("@consentiVuoti", 0);
            comm.Parameters.AddWithValue("@sogliaMenoMille", MENOMILLE);
            comm.Parameters.AddWithValue("@completo", 0);
            comm.Connection = conn;

            conn.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            //conn.Open();
            DataTable table = new DataTable();
            try
            {
                adapter.Fill(table);
            }
            catch(SqlException e)
            {
                conn.Close();
                return -1;

            }

            List<double> data = new List<double>();
            /*foreach(DataRow r in table.Rows)
            {
                foreach(var item in r.ItemArray)
                {
                    System.Console.Write(item+"\t");
                }
                System.Console.WriteLine();
            }*/
            foreach(DataRow r in table.Rows)
            {
                data.Add((int)r[1]);
            }
            
            int x;
            try
            {
                x = data.Count;
            }
            catch
            {
                x = -1;
            }

            conn.Close();
            return x;
        }
    }
}
