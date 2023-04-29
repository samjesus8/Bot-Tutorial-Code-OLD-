using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace YouTubeTestBot.Engine.LevelSystem
{
    public class LevelEngine
    {
        public bool levelledUp = false;
        public bool StoreUserDetails(DUser user)
        {
            try
            {
                var path = @"D:\Visual Studio Projects\YouTubeDiscordBot\bin\Debug\UserInfo.json";

                var json = File.ReadAllText(path);
                var jsonObj = JObject.Parse(json);

                var members = jsonObj["members"].ToObject<List<DUser>>();
                members.Add(user);

                jsonObj["members"] = JArray.FromObject(members);
                File.WriteAllText(path, jsonObj.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckUserExists(string username, ulong guildID)
        {
            using (StreamReader sr = new StreamReader("UserInfo.json"))
            {
                string json = sr.ReadToEnd();
                LevelJSONFile userToGet = JsonConvert.DeserializeObject<LevelJSONFile>(json);

                foreach (var user in userToGet.members)
                {
                    if (user.UserName == username && user.guildID == guildID)
                    {
                        return true;
                    }
                    else { }
                }
            }
            return false;
        }

        public DUser GetUser(string username, ulong guildID)
        {
            using (StreamReader sr = new StreamReader("UserInfo.json"))
            {
                string json = sr.ReadToEnd();
                LevelJSONFile userToGet = JsonConvert.DeserializeObject<LevelJSONFile>(json);

                foreach (var user in userToGet.members)
                {
                    if (user.UserName == username && user.guildID == guildID)
                    {
                        return new DUser()
                        {
                            UserName = user.UserName,
                            guildID = user.guildID,
                            avatarURL = user.avatarURL,
                            XP = user.XP,
                            Level = user.Level
                        };
                    }
                    else { }
                }
            }
            return null;
        }

        public bool AddXP(string username, ulong guildID)
        {
            levelledUp = false;
            try
            {
                var path = @"D:\Visual Studio Projects\YouTubeDiscordBot\bin\Debug\UserInfo.json";

                var json = File.ReadAllText(path);
                var jsonObj = JObject.Parse(json);

                var members = jsonObj["members"].ToObject<List<DUser>>();
                foreach (var user in members)
                {
                    if (user.UserName == username && user.guildID == guildID)
                    {
                        user.XP = user.XP + 0.5;
                    }
                    if (user.XP >= 10)
                    {
                        levelledUp = true;
                        user.Level++;
                        user.XP = 0;
                    }
                }

                jsonObj["members"] = JArray.FromObject(members);
                File.WriteAllText(path, jsonObj.ToString());

                return true;
            }
            catch 
            { 
                return false; 
            }
        }
    }

    internal sealed class LevelJSONFile
    {
        public string userInfo { get; set; }
        public DUser[] members { get; set; }
    }
}
