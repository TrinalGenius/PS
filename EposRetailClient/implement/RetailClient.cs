using sven.common.https;
using sven.common.log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EposRetailClient.implement
{
    class RetailClient
    {

        public static List<KeyValuePair<string, decimal>> getCurrTables(decimal currencyScale, String retailTableList) 
        {

            List<KeyValuePair<string, decimal>> list = new List<KeyValuePair<string, decimal>>();
            try
            {
                FormLogUtils.getInstance().info(" requested for table List ");


                SqlConnection sqlConn = new SqlConnection(ConfigurationManager.AppSettings["SQLString"]);

                using (sqlConn)
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = sqlConn.CreateCommand())
                    {

                        String sql = "select distinct tablefullname, billID, sumToPay from [bi_tempbill] where sumToPay > 0";
                        if (!String.IsNullOrEmpty(retailTableList))
                        {
                            sql += " and tableid in (" + retailTableList + ")";
                        }

                        sql += " order by tablefullname";


                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                String tableID = reader.GetString(0);
                                String billID = reader.GetString(1);
                                decimal sumToPay = reader.GetDecimal(2);

                                list.Add(new KeyValuePair<string, decimal>(tableID, sumToPay * currencyScale));
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                FormLogUtils.getInstance().error(e.ToString(),e);
            }
            return list;
        }

        public static bool doTransaction(String url, string terminalID, decimal amount, String sequence) 
        {

            bool result = false;

            String httpsReturn = HttpsUtils.getHttps(String.Format(url, terminalID, amount, sequence));

            if ("SUCCESS".Equals(httpsReturn))
            {
                result = true;
            }

            return result;
        }
    }
}
