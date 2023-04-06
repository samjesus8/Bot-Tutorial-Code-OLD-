using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;

namespace YouTubeTestBot.Engine.YouTube
{
    public class YouTubeEngine
    {
        public string channelId = "Channel-ID-Here";
        public string apiKey = "API-Key-Here";
        public YouTubeVideo _video = new YouTubeVideo();

        public YouTubeVideo GetLatestVideo(string channelId, string apiKey)
        {
            string videoId; //Temporary variables for video details
            string videoUrl;
            string videoTitle;
            DateTime? videoPublishedAt;

            var youtubeService = new YouTubeService(new BaseClientService.Initializer() //Initialising our API
            {
                ApiKey = apiKey,
                ApplicationName = "MyApp"
            });

            var searchListRequest = youtubeService.Search.List("snippet"); //Setting up our search
            searchListRequest.ChannelId = channelId;
            searchListRequest.MaxResults = 1;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            var searchListResponse = searchListRequest.Execute(); //Executing the search

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video") //We are looking for a youtube video here
                {
                    videoId = searchResult.Id.VideoId; //Setting our details
                    videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                    videoTitle = searchResult.Snippet.Title;
                    videoPublishedAt = searchResult.Snippet.PublishedAt;
                    var thumbnail = searchResult.Snippet.Thumbnails.Default__.Url;

                    return new YouTubeVideo() //Storing in a class for use in the bot
                    {
                        videoId = videoId,
                        videoUrl = videoUrl,
                        videoTitle = videoTitle,
                        thumbnail = thumbnail,
                        PublishedAt = videoPublishedAt
                    };
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
