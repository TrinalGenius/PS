using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace RetailService.sven.common.json
{
    class JsonUtils
    {

        public static String code (Object obj) 
        {
                            
            String result = "";

            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())

            {

                json.WriteObject(stream, obj);

                result = Encoding.UTF8.GetString(stream.ToArray());

            }

            return result;
        }
    }
}
