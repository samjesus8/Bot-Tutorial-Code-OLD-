using System.Collections.Generic;

namespace YouTubeTestBot.Engine
{
    public class SwearFilter
    {
        public List<string> filter = new List<string>();
        public SwearFilter() 
        {
            filter.Add("fuck");
            filter.Add("shit");
            filter.Add("cunt");
        }
    }
}
