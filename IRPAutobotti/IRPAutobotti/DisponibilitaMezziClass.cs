using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    public struct DisponibilitaMezziStruct
    {
        public int[] IdM;
        public int[,] scomparti;
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
            string p = "{call TIP.BIS.DisponibilitaMezzi(" + attivo.ToString() + ",'" + data.ToString() + "'," + baseCarico.ToString() + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();

            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            int[] scompartiAnt = new int[10];
            int[] scompartiPost = new int[10];
            //capacità tonnellate
            int[] turno = X.AsEnumerable().Select(r => r.Field<int>("Turno")).ToArray();
            double[] captonAnt = new double[turno.Length];
            double[] captonPost = new double[turno.Length];
            //variabile temporanea per la somma
            double[] captonTemp = new double[turno.Length];
            int[] IdM = X.AsEnumerable().Select(r => r.Field<int>("Id")).ToArray();
            // 17 è la colonna finale degli scomparti anteriori
            for (int i=0;i<10;i++)
            {
                scompartiAnt[i] = X.AsEnumerable().Select(r => r.Field<int>(8+i)).ToArray();
            }
            // prendo i valori dalle colonne
            captonAnt = X.AsEnumerable().Select(r => r.Field<double>("PortUtilAnt")).ToArray();
            captonPost = X.AsEnumerable().Select(r => r.Field<double>("PortUtiPost")).ToArray();
            for(int i=0;i<captonAnt.Length;i++)
            {
                //somma membro a membro
                captonTemp[i] = captonAnt[i] + captonPost[i];
            }
            //copio nella struct
            dmStruct.captontemp = captonTemp;
            //stesso ragionamento per le targhe
            string[] targaAnt = X.AsEnumerable().Select(r => r.Field<string>("targaAnt")).ToArray();
            string[] targaPost = X.AsEnumerable().Select(r => r.Field<string>("targaPost")).ToArray();
            //creo le variabili temporanee
            string[] targaTemp1 = new string[targaAnt.Length];
            string[] targaTemp2 = new string[targaAnt.Length];
            for (int i=0;i<targaAnt.Length;i++)
            {
                targaTemp1[i] = targaAnt[i] + "/" + targaPost[i];
                targaTemp2[i] = "''" + targaAnt[i] + "/" + targaPost[i] + "''";
            }
            //e le copio nella struct da ritornare
            dmStruct.targatemp1 = targaTemp1;
            dmStruct.targatemp2 = targaTemp2;
            return dmStruct;
        }
    }
}
