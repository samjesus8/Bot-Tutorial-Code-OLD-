using YouTubeTestBot.Engine.ImageHandler;

namespace YouTubeTestBot
{
    internal class Program
    {
        public static GoogleImageHandler imageHandler;
        static void Main(string[] args)
        {
            imageHandler = GoogleImageHandler.Instance;
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
