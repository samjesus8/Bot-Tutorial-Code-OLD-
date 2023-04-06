using System.Collections.Generic;

namespace YouTubeTestBot.Engine.ImageHandler
{
    public class GoogleImageHandler
    {
        public Dictionary<int, string> images = new Dictionary<int, string>();
        public static readonly GoogleImageHandler Instance = new GoogleImageHandler();

        static GoogleImageHandler() { }

        public string GetImageAtId(int id)
        {
            var imageURL = images.TryGetValue(id, out var image);
            return image;
        }
    }
}
