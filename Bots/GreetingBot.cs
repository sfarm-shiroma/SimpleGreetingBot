using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace SimpleGreetingBot
{
    public class GreetingBot : ActivityHandler
    {
        private static readonly string[] Greetings = new[] { "おはよう", "こんにちは", "こんばんは" };
        private static readonly Random Rng = new Random();
        private readonly ILogger<GreetingBot> _logger;

        public GreetingBot(ILogger<GreetingBot> logger)
        {
            _logger = logger;
        }

        protected override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text ?? string.Empty;
            var reply = Greetings[Rng.Next(Greetings.Length)];
            _logger.LogInformation("OnMessageActivity: from={FromId}, text={Text}, reply={Reply}", turnContext.Activity.From?.Id, text, reply);
            return turnContext.SendActivityAsync(MessageFactory.Text(reply), cancellationToken);
        }

        protected override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient?.Id)
                {
                    var welcome = "メッセージを送ってください。『おはよう』『こんにちは』『こんばんは』のいずれかをランダムに返します。";
                    _logger.LogInformation("Welcome sent to member={MemberId}", member.Id);
                    turnContext.SendActivityAsync(MessageFactory.Text(welcome), cancellationToken);
                }
            }
            return Task.CompletedTask;
        }
    }
}
