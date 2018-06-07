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
        public double[] dens_BS;
        public double[] dens_D;
        public double[] dens_B95;
        public double[] dens_GA;
        public double[] dens_BD;
        public int[] CARICA;
        public int[] SCARICA;
        public double[] SCARICALITRO;
        public int[] V_MEDIA;
        public double[] MINxKM;
        public int[] TEMPO_MAX;
        public int[] MAXDROP;
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

        public CaricaSettingsStruct CaricaSettings(int baseCarico,SqlConnection conn)
        {
            //strcat
            string p = "{call TIP.BIS.getSettingByDataBase(" + baseCarico.ToString() + ")}";
            //connection
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            //var curs = comm.ExecuteScalar();
            //SqlDataReader curs = comm.ExecuteReader();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            //prendo la tabella "Data" (colonna della tabella tables) 
            //e la converto in una DataTable
            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            // parametri algortimo
            csStruct.GIACENZA_MIN = 20;
            csStruct.KM_MIN = 90;
            csStruct.DISTANZA_MAX_PVPV = 30;
            csStruct.ELLISSE = 1.05;
            csStruct.beta = 0.1; // 0 solo valore 1 no valore solo preferenza,
            csStruct.esponente = 3;

            if(X!=null)
            {
                //Parsing usando sintassi LINQ, prende la Colonna di X che corrisponde al nome indicato
                // e per ogni elemento r di tipo <type> effettua il casting e mette il tutto in un array
                csStruct.dens_BS = X.AsEnumerable().Select(r => r.Field<double>("densitaBB")).ToArray();
                csStruct.dens_D = X.AsEnumerable().Select(r => r.Field<double>("densitaG")).ToArray();
                csStruct.dens_B95 = X.AsEnumerable().Select(r => r.Field<double>("densitaB")).ToArray();
                csStruct.dens_BD = X.AsEnumerable().Select(r => r.Field<double>("densitaBD")).ToArray();
                csStruct.dens_GA = X.AsEnumerable().Select(r => r.Field<double>("densitaGA")).ToArray();

                csStruct.CARICA = X.AsEnumerable().Select(r => r.Field<int>("tempoCarico")).ToArray();
                csStruct.SCARICA = X.AsEnumerable().Select(r => r.Field<int>("tempoScarico")).ToArray();
                csStruct.SCARICALITRO = X.AsEnumerable().Select(r => r.Field<double>("rateScarico")).ToArray();
                int[] V_MEDIA = X.AsEnumerable().Select(r => r.Field<int>("velMedia")).ToArray();
                double[] MINxKM = new double[V_MEDIA.Length];
                for(int i=0;i<V_MEDIA.Length;i++)
                {
                    MINxKM[i] = 60 / V_MEDIA[i];
                }
                csStruct.V_MEDIA = V_MEDIA;
                csStruct.MINxKM = MINxKM;
                csStruct.TEMPO_MAX = X.AsEnumerable().Select(r => r.Field<int>("tempoMaxLavoro")).ToArray();
                csStruct.MAXDROP = X.AsEnumerable().Select(r => r.Field<int>("Corsia")).ToArray();
                return csStruct;
            }
            else
            {
                Console.WriteLine("Couldn't get this Table / CaricaSettings");
                return null;
            }
            
        }
    }
}
