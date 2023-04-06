using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using OpenAI_API;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeTestBot.Commands
{
    public class UserRequestedCommands : BaseCommandModule
    {
        [Command("image")]
        [Description("Searches Google Images for the given query.")]
        public async Task ImageSearch(CommandContext ctx, [RemainingText] string query)
        {
            Program.imageHandler.images.Clear();
            int IDCount = 0;

            // Replace with your own Custom Search Engine ID and API Key

            string cseId = "e14e788ce60ac4667";
            string apiKey = "AIzaSyCo6GQru37bLMaPRLv8gcj_v0yeY2S0wAo";

            //Initialise the API

            var customSearchService = new CustomsearchService(new BaseClientService.Initializer
            {
                ApplicationName = "Discord Bot",
                ApiKey = apiKey,
            });

            //Create your search request

            var listRequest = customSearchService.Cse.List();
            listRequest.Cx = cseId;
            listRequest.Num = 10;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            listRequest.Q = query;

            //Execute the search request & get the results

            var search = await listRequest.ExecuteAsync();
            var results = search.Items;

            foreach (var result in results)
            {
                Program.imageHandler.images.Add(IDCount, result.Link);
                IDCount++;
            }

            if (results == null || !results.Any())
            {
                await ctx.RespondAsync("No results found.");
                return;
            }
            else
            {
                //Create the buttons for this embed
                var previousEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":track_previous:"));
                var previousButton = new DiscordButtonComponent(ButtonStyle.Primary, "previousButton", "Previous", false, previousEmoji);

                var nextEmoji = new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":track_next:"));
                var nextButton = new DiscordButtonComponent(ButtonStyle.Primary, "nextButton", "Next", false, nextEmoji);

                //Display the First Result
                var firstResult = results.First();
                var imageMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("Results for: " + query)
                    .WithImageUrl(firstResult.Link)
                    )
                    .AddComponents(previousButton, nextButton);

                await ctx.Channel.SendMessageAsync(imageMessage);
            }
        }

        [Command("gpt")]
        public async Task ChatGPT(CommandContext ctx, params string[] message)
        {
            //Initialise the API
            var api = new OpenAIAPI("API-KEY-HERE");

            //Initialise a new Chat
            var chat = api.Chat.CreateConversation();
            chat.AppendSystemMessage("Type in a query");

            //Pass in the query to GPT
            chat.AppendUserInput(string.Join(" ", message));

            //Get the response
            string response = await chat.GetResponseFromChatbot();

            //Show in Discord Embed Message
            var responseMsg = new DiscordEmbedBuilder()
            {
                Title = string.Join(" ", message),
                Description = response,
                Color = DiscordColor.Green
            };

            await ctx.Channel.SendMessageAsync(embed: responseMsg);
        }
    }
}
