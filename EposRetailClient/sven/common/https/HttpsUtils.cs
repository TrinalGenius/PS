using sven.common.log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace sven.common.https
{
    class HttpsUtils
    {

        public static String getHttps(String url)
        {
            String responseFromServer = "";
            try
            {

                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }
            catch (Exception e)
            {

                FormLogUtils.getInstance().error(e.ToString(), e);
            }

            return responseFromServer;
        }
    }
}
