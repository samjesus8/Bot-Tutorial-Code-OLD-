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
            // Replace with your own Custom Search Engine ID and API Key
            string cseId = "Custom-Search-Engine-ID";
            string apiKey = "API-KEY";

            var customSearchService = new CustomsearchService(new BaseClientService.Initializer
            {
                ApplicationName = "Discord Bot",
                ApiKey = apiKey,
            });

            var listRequest = customSearchService.Cse.List();
            listRequest.Cx = cseId;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            listRequest.Q = query;

            var search = await listRequest.ExecuteAsync();
            var results = search.Items;

            if (results == null || !results.Any())
            {
                await ctx.RespondAsync("No results found.");
                return;
            }

            // Get the first result from the search and send it as a message
            var firstResult = results.First();
            await ctx.RespondAsync(firstResult.Link);
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
