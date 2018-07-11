using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    public struct PrendereValoriSogliaVariabiliStruct
    {
        public double MENOMILLE;
        public double RIEMPIMENTOMAX;
    }
    class PrendereValoriSogliaVariabiliClass
    {
        public PrendereValoriSogliaVariabiliStruct vsvStruct;
        public PrendereValoriSogliaVariabiliClass()
        {
            vsvStruct = new PrendereValoriSogliaVariabiliStruct();
        }

        public PrendereValoriSogliaVariabiliStruct PrendereValoriSogliaVariabili(int Id, SqlConnection conn)
        {
            string p = "{call Matlab.[BIS].[getSettingVariabiliById](" + Id + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }

            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            string temp = "";
            foreach (DataRow row in X.Rows)
            {
                temp = row["Soglie"].ToString();
            }
            string[] s = temp.Split(';');
            vsvStruct.MENOMILLE = Convert.ToDouble(s[1]);
            vsvStruct.RIEMPIMENTOMAX = Convert.ToDouble(s[0]);
            return vsvStruct;
        }
    }
}
