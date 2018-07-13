using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    public struct DisponibilitaMezziStruct
    {
        public int[] IdM;
        public double[,] scomparti;
        public double[] captontemp;
        public string[] targatemp1;
        public string[] targatemp2;
    }
    class DisponibilitaMezziClass
    {
        DisponibilitaMezziStruct dmStruct;
        public DisponibilitaMezziClass()
        {
            dmStruct = new DisponibilitaMezziStruct();
        }

        public DisponibilitaMezziStruct DisponibilitaMezzi(int attivo, string data, int baseCarico, SqlConnection conn)
        {

            //connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.DisponibilitaMezzi";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@attivo", attivo);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@idBase", baseCarico);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();

            List<int[]> scompartiAnt = new List<int[]>();
            List<int[]> scompartiPost = new List<int[]>();
            //capacità tonnellate
            int[] turno = (from IDataRecord r in reader select (int)r["Turno"]).ToArray();
            double[] captonAnt = new double[turno.Length];
            double[] captonPost = new double[turno.Length];
            //variabile temporanea per la somma
            double[] captonTemp = new double[turno.Length];
            int[] IdM = (from IDataRecord r in reader select (int)r["Id"]).ToArray();
            // 17 è la colonna finale degli scomparti anteriori
            //prendo le colonne da 8 a 18 per gli Anteriori, da 20 a 30 per i Posteriori
            for (int i=0;i<10;i++)
            {
                int[] scompartiAntTemp = (from IDataRecord r in reader select (int)r.GetValue(8 + i)).ToArray();
                int[] scompartiPostTemp = (from IDataRecord r in reader select (int)r.GetValue(20 + i)).ToArray();
                scompartiAnt.Add(scompartiAntTemp);
                scompartiPost.Add(scompartiPostTemp);
            }
            double[,] scomparti = new double[scompartiPost.Count, scompartiPost[0].Length];
            // sommo quindi membro a membro ogni elemento e lo metto dentro ad una matrice di interi
            for(int i=0;i<scompartiAnt.Count;i++)
            {
                for(int j=0;j<scompartiAnt[0].Length;j++)
                {
                    scomparti[i, j] = scompartiAnt[i][j] + scompartiPost[i][j];
                }
            }
            // prendo i valori dalle colonne
            captonAnt = (from IDataRecord r in reader select (double)r["PortUtilAnt"]).ToArray();
            captonPost = (from IDataRecord r in reader select (double)r["PortUtilPost"]).ToArray();
            for(int i=0;i<captonAnt.Length;i++)
            {
                //somma membro a membro
                captonTemp[i] = captonAnt[i] + captonPost[i];
            }
            //stesso ragionamento per le targhe
            string[] targaAnt = (from IDataRecord r in reader select (string)r["targaAnt"]).ToArray();
            string[] targaPost = (from IDataRecord r in reader select (string)r["targaPost"]).ToArray();
            //creo le variabili temporanee
            string[] targaTemp1 = new string[targaAnt.Length];
            string[] targaTemp2 = new string[targaAnt.Length];
            for (int i=0;i<targaAnt.Length;i++)
            {
                targaTemp1[i] = targaAnt[i] + "/" + targaPost[i];
                targaTemp2[i] = "''" + targaAnt[i] + "/" + targaPost[i] + "''";
            }
            //e le copio nella struct da ritornare
            dmStruct.IdM = IdM;
            dmStruct.captontemp = captonTemp;
            dmStruct.targatemp1 = targaTemp1;
            dmStruct.targatemp2 = targaTemp2;
            dmStruct.scomparti = scomparti;
            reader.Close();
            conn.Close();
            return dmStruct;
        }
    }
}
