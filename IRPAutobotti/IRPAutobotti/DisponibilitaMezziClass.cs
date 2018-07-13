using System;
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

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            conn.Open();
            DataTable table = new DataTable();
            adapter.Fill(table);

            List<int[]> scompartiAnt = new List<int[]>();
            List<int[]> scompartiPost = new List<int[]>();


            long[] IdMLong = (from DataRow r in table.Rows select (long)r["id"]).ToArray();
            int[] IdM = IdMLong.Select(item => unchecked((int)item)).ToArray();

            //capacità tonnellate
            int[] turno = (from DataRow r in table.Rows select (int)r["Turno"]).ToArray();
            double[] captonAnt = new double[turno.Length];
            double[] captonPost = new double[turno.Length];
            //variabile temporanea per la somma
            double[] captonTemp = new double[turno.Length];



            // 17 è la colonna finale degli scomparti anteriori
            //prendo le colonne da 8 a 18 per gli Anteriori, da 21 a 30 per i Posteriori
            for (int i = 0; i < 10; i++)
            {
                //var temp = ;
                scompartiAnt.Add((from DataRow r in table.Rows select (int)((decimal)r[7 + i])).ToArray());
                scompartiPost.Add((from DataRow r in table.Rows select (int)((decimal)r[19 + i])).ToArray());
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
            captonAnt = (from DataRow r in table.Rows select (double)(float)r["PortUtilAnt"]).ToArray();
            captonPost = (from DataRow r in table.Rows select (double)(float)r["PortUtilPost"]).ToArray();
            for(int i=0;i<captonAnt.Length;i++)
            {
                //somma membro a membro
                captonTemp[i] = captonAnt[i] + captonPost[i];
            }
            //stesso ragionamento per le targhe
            string[] targaAnt = (from DataRow r in table.Rows select (string)r["targaAnt"]).ToArray();
            string[] targaPost = (from DataRow r in table.Rows select (string)r["targaPost"]).ToArray();
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

            conn.Close();
            return dmStruct;
        }
    }
}
