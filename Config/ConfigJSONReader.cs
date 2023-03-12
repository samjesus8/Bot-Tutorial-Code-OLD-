using Newtonsoft.Json;
using System.IO;

namespace YouTubeTestBot.Config
{
    public class ConfigJSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public ConfigJSONReader() 
        {
            using (StreamReader sr = new StreamReader("config.json")) 
            {
                string json = sr.ReadToEnd();
                ConfigJSON obj = JsonConvert.DeserializeObject<ConfigJSON>(json);

                this.token = obj.Token;
                this.prefix = obj.Prefix;
            }
        }
    }
}
