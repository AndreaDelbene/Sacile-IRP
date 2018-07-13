using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    public struct IdSettingsVariabiliStruct
    {
        public int[] Id;
        public double[] MENOMILLE;
        public double[] RIEMPIMENTOMAX;
    }
    class IdSettingsVariabiliClass
    {
        IdSettingsVariabiliStruct isvStruct;
        public IdSettingsVariabiliClass()
        {
            isvStruct = new IdSettingsVariabiliStruct();
        }

        public IdSettingsVariabiliStruct IdSettingsVariabili(int baseCarico, SqlConnection conn)
        { 
            //connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.getSettingVariabili";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_base", baseCarico);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();
            reader.Read();
            int[] Id = (from IDataRecord r in reader
                                 select (int)r["id"]
                            ).ToArray();
            
            double[] MENOMILLE = new double[Id.Length];
            double[] RIEMPIMENTOMAX = new double[Id.Length];
            reader.Close();
            conn.Close();
            comm.CommandText = "Matlab.BIS.getSettingVariabiliById";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.Add("@indice", SqlDbType.Int);

            for (int i=0;i<Id.Length;i++)
            {

                comm.Parameters["@indice"].Value = Id[i];
                comm.Connection = conn;

                conn.Open();

                reader = comm.ExecuteReader();
                reader.Read();

                string soglie = (string) reader["soglie"];

                string[] S = soglie.Split(Convert.ToChar(soglie), ';');
                MENOMILLE[i] = Convert.ToDouble(Convert.ToChar(S[2])) / 1000;
                RIEMPIMENTOMAX[i] = Convert.ToDouble(Convert.ToChar(S[1])) / 1000;
                reader.Close();
                conn.Close();
            }
            //assegno le variabili nella struct
            isvStruct.Id = Id;
            isvStruct.MENOMILLE = MENOMILLE;
            isvStruct.RIEMPIMENTOMAX = RIEMPIMENTOMAX;
            //e ritorno la struct
            return isvStruct;
        }
    }
}
