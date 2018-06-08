using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    class CreateVersionClass
    {
        public CreateVersionClass()
        {
        }

        public int CreateVersion(int baseCarico, string login, string data, int IdSetting, int IdRunner, SqlConnection conn)
        {
            /*setdbprefs('DataReturnFormat', 'numeric');
            setdbprefs('NullNumberRead', 'NaN');
            setdbprefs('NullStringRead', 'null');*/

            string p = "{call TIP.BIS.createVersionSacile(" + baseCarico.ToString() + ",'" + login + "','" + data + "'," +
                 IdSetting.ToString() + "," + IdRunner.ToString() + ")}";
            //connection
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            int[] temp = tables.AsEnumerable().Select(r => r.Field<int>("Data")).ToArray();
            int IdVersion = temp[2];
            return IdVersion;
        }
    }
}
