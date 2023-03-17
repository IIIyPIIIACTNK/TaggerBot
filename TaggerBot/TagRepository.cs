using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace TaggerBot
{
    delegate string[] ReturnIdFromRepository(string tag);
    public class TagRepository
    {
        Dictionary<string, List<string>> tagAudioIdsPairs = new Dictionary<string, List<string>>();

        DataToJson dataToJson = new DataToJson();
        public Dictionary<string,List<string>> TagAudioIdsPairs { get { return tagAudioIdsPairs; } }
        public TagRepository()
        {
            Program.ReturnIdFromRepository += FileIdFromRepository;
            tagAudioIdsPairs = dataToJson.DeserialiseJson();
            DisplayAvaliableTags();
        }

        public void AppendRepository(string tag,Message message) 
        {
            string fileId = message.Audio.FileId;
            if (tagAudioIdsPairs != null && tagAudioIdsPairs.ContainsKey(tag))
            {
                tagAudioIdsPairs[tag].Add(fileId);
            }
            else
            {
                tagAudioIdsPairs.Add(tag, new List<string>() { fileId });
            }
            Console.WriteLine($"TaggingDone Tag: {tag} FileId: {fileId}");
        }

        public string[] FileIdFromRepository(string tag) 
        { 
            return tagAudioIdsPairs[tag].ToArray();
        }
        
        public void SaveRepository()
        {
            dataToJson.AudioIdsToJson(TagAudioIdsPairs);
        }

        private void DisplayAvaliableTags()
        {
            if (tagAudioIdsPairs!=null)
            {
                StringBuilder sb = new StringBuilder();
                int keysCount = 1;
                foreach (var e in tagAudioIdsPairs)
                {
                    sb.AppendLine($"------------{keysCount}------------");
                    sb.Append(e.Key);
                    if (e.Value.Count > 1)
                    {
                        sb.AppendLine($" {e.Value[0],-5}");
                        for (int i = 1; i < e.Value.Count; i++)
                        {
                            sb.AppendLine($" {e.Value[i],-10}");
                        }
                    }
                    else
                    {
                        sb.Append($"{e.Value[0],-5}");
                    }
                    keysCount++;
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}
