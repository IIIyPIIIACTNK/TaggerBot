using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;
using System.ComponentModel;

namespace TaggerBot
{
    internal class DataToJson
    {
        JsonArray jsonArray;
        readonly string path = @"data.json";

        private void SerializeToJson()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(jsonArray);
            }
        }

        public void EndlessSerializeToJson() 
        { 
            while(true)
            {
                Thread.Sleep(30000);
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(jsonArray);
                }
            }
        }
        public void AudioIdsToJson(Dictionary<string,List<string>> repository)
        {
            using(StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(JsonConvert.SerializeObject(repository),Formatting.Indented);
            }
        }

        public Dictionary<string, List<string>> DeserialiseJson()
        {
            Dictionary<string, List<string>> rep = new Dictionary<string, List<string>>();
            using (StreamReader sr = new StreamReader(path))
            {
                string json = sr.ReadToEnd();
                Console.WriteLine(json);
                rep = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                Console.WriteLine(rep);
            }
            return rep;
        }
    }
}
