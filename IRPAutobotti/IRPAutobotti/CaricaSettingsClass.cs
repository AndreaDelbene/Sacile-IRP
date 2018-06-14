using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    //struct contenente i valori di ritorno
    public struct CaricaSettingsStruct
    {
        public int GIACENZA_MIN;
        public double dens_BS;
        public double dens_D;
        public double dens_B95;
        public double dens_GA;
        public double dens_BD;
        public int CARICA;
        public int SCARICA;
        public double SCARICALITRO;
        public int V_MEDIA;
        public double MINxKM;
        public int TEMPO_MAX;
        public int MAXDROP;
        public int KM_MIN;
        public int DISTANZA_MAX_PVPV;
        public double ELLISSE;
        public double beta;
        public int esponente;

    }

    class CaricaSettingsClass
    {
        CaricaSettingsStruct csStruct;
        //Costruttore
        public CaricaSettingsClass()
        {
            csStruct = new CaricaSettingsStruct();
        }

        public CaricaSettingsStruct CaricaSettings(int baseCarico, SqlConnection conn)
        {

            //connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "BIS.getSettingByDataBase";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_base", baseCarico);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();


            // Lettura riga per riga del risultato
            /*while (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader.GetValue(i).ToString() + " - ");
                    }
                    Console.Write("\n");
                }
                reader.NextResult();
            }*/

            csStruct.GIACENZA_MIN = 20;
            csStruct.KM_MIN = 90;
            csStruct.DISTANZA_MAX_PVPV = 30;
            csStruct.ELLISSE = 1.05;
            csStruct.beta = 0.1; // 0 solo valore 1 no valore solo preferenza,
            csStruct.esponente = 3;

            reader.Read();
            csStruct.dens_BS = (double)(decimal)reader["densitaBB"];
            csStruct.dens_D = (double)(decimal)reader["densitaG"];
            csStruct.dens_B95 = (double)(decimal)reader["densitaB"];
            csStruct.dens_BD = (double)(decimal)reader["densitaBD"];
            csStruct.dens_GA = (double)(decimal)reader["densitaGA"];

            csStruct.CARICA = (int)reader["tempoCarico"];
            csStruct.SCARICA = (int)reader["tempoScarico"];
            csStruct.SCARICALITRO = (double)(decimal)reader["rateScarico"];
            int V_MEDIA = (int)reader["velMedia"];
            double MINxKM = 60 / V_MEDIA;
            csStruct.V_MEDIA = V_MEDIA;
            csStruct.MINxKM = MINxKM;
            csStruct.TEMPO_MAX = (int)reader["tempoMaxLavoro"];
            csStruct.MAXDROP = (int)reader["Corsia"];

            reader.Close();
            return csStruct;
        }
    }
}
