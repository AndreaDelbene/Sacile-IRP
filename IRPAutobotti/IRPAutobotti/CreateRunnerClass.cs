using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{

    class CreateRunnerClass
    {
        public CreateRunnerClass()
        {
        }

        public int CreateRunner(string login,int BaseDiCarico,string data,SqlConnection conn)
        {
            /*setdbprefs('DataReturnFormat', 'numeric');
            setdbprefs('NullNumberRead', 'NaN');
            setdbprefs('NullStringRead', 'null');*/

            string p = "{call TIP.BIS.createRunnerBySolution('"+ login + "'," + BaseDiCarico.ToString() + ",'" + data + "',sacile)}";
            //connection
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            int[] temp = tables.AsEnumerable().Select(r => r.Field<int>("Data")).ToArray();
            int IdRunner = temp[2];
            return IdRunner;
        }
    }
}
