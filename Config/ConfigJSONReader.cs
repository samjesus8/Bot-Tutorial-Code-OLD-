using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeTestBot.Config
{
    public class ConfigJSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json", new UTF8Encoding(false)))
            {
                string json = await sr.ReadToEndAsync();
                ConfigJSON obj = JsonConvert.DeserializeObject<ConfigJSON>(json);

                this.token = obj.Token;
                this.prefix = obj.Prefix;
            }
        }
    }
}
