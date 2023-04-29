using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeTestBot.Config
{
    public class ConfigJSONReader
    {
        //Declare our Token & Prefix properties of this class
        public string discordToken { get; set; }
        public string discordPrefix { get; set; }

        public async Task ReadJSON() //This method must be run asynchronously
        {
            using (StreamReader sr = new StreamReader("config.json", new UTF8Encoding(false)))
            {
                //Read and then De-Serealize the config.json file
                string json = await sr.ReadToEndAsync();
                JSONStruct obj = JsonConvert.DeserializeObject<JSONStruct>(json);

                //Set our properties
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
