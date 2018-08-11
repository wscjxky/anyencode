using System;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
namespace vodfile
{
    public class JsonStringModule
    {
        //转换name value 到 json
        public static String ToJsonString(Dictionary<string, string> dictionary)
        {
            string json = "{";
            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                json = json +"\""+ kvp.Key + "\""+":\"" + kvp.Value + "\",";

            }
            json = json + "}";
            json = json.Replace(",}", "}");
            return json;
        }

        //转换name value 到 json
        public static String ToJsonString(Dictionary<string, string>[] dictionaryArr)
        {
            string json = "[";
            foreach (Dictionary<string, string> dic in dictionaryArr)
            {
                json += "{";
                foreach (KeyValuePair<string, string> kvp in dic)
                {
                    json = json +"\""+ kvp.Key +"\"" +":\"" + kvp.Value + "\",";

                }
                json += "}" ;
                json = json.Replace(",}", "},");
            }
            json = json + "]";
            json = json.Replace(",]", "]");
            return json;
        }

        //转换name value 到 json
        public static String ToJsonString(List<Dictionary<string, string>> list)
        {
            string json = "[";
            foreach (Dictionary<string, string> dic in list)
            {
                json += "{";
                foreach (KeyValuePair<string, string> kvp in dic)
                {
                    json = json + "\""+kvp.Key +"\"" +":\"" + kvp.Value + "\",";

                }
                json += "}";
                json = json.Replace(",}", "},");
            }
            json = json + "]";
            json = json.Replace(",]", "]");
            return json;
        }  
         
         

    }
}