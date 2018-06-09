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
            /*setdbprefs('DataReturnFormat', 'table');
            setdbprefs('NullNumberRead', 'NaN');
            setdbprefs('NullStringRead', 'null');*/

            string p1 = "{call TIP.BIS.getSettingVariabili(" + baseCarico.ToString() + ")}";
            SqlCommand comm1 = new SqlCommand(p1, conn);
            comm1.ExecuteNonQuery();
            var tables1 = new DataTable();
            using (var curs = new SqlDataAdapter(comm1))
            {
                curs.Fill(tables1);
            }
            DataTable X1 = tables1.DefaultView.ToTable(false, tables1.Columns["Data"].ColumnName);
            //Data{2}
            DataTable X = X1.DefaultView.ToTable(false, tables1.Columns[2].ColumnName);
            int[] Id = X.AsEnumerable().Select(r => r.Field<int>("id")).ToArray();
            double[] MENOMILLE = new double[Id.Length];
            double[] RIEMPIMENTOMAX = new double[Id.Length];

            for (int i=0;i<Id.Length;i++)
            {
                string p2 = "{call TIP.[BIS].[getSettingVariabiliById](" + Id[i].ToString() + ")}";
                SqlCommand comm2 = new SqlCommand(p2, conn);
                comm2.ExecuteNonQuery();
                var tables2 = new DataTable();
                using (var curs = new SqlDataAdapter(comm2))
                {
                    curs.Fill(tables2);
                }
                DataTable X2 = tables2.DefaultView.ToTable(false, tables2.Columns["Data"].ColumnName);
                string soglie = X1.AsEnumerable().Select(r => r.Field<string>("soglie")).ToString();

                string[] S = soglie.Split(Convert.ToChar(soglie), ';');
                MENOMILLE[i] = Convert.ToDouble(Convert.ToChar(S[2])) / 1000;
                RIEMPIMENTOMAX[i] = Convert.ToDouble(Convert.ToChar(S[1])) / 1000;
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
