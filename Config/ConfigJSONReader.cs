using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeTestBot.Config
{
    public class ConfigJSONReader
    {
        public string discordToken { get; set; }
        public string discordPrefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json", new UTF8Encoding(false)))
            {
                string json = await sr.ReadToEndAsync();
                JSONStruct obj = JsonConvert.DeserializeObject<JSONStruct>(json);

                this.discordToken = obj.token;
                this.discordPrefix = obj.prefix;
            }
        }
    }

    internal sealed class JSONStruct
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
}
