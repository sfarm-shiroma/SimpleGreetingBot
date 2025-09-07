using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace SimpleGreetingBot.Middleware
{
    // 受信(ユーザー→ボット)と送信(ボット→ユーザー)のActivityをロギング
    public class ActivityLoggingMiddleware : Microsoft.Bot.Builder.IMiddleware
    {
        private readonly ILogger<ActivityLoggingMiddleware> _logger;
        private const string Tag = "[GreetingBot]"; // 共通タグ案

        public ActivityLoggingMiddleware(ILogger<ActivityLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            var a = turnContext.Activity;
            if (a != null)
            {
                if (a.Type == ActivityTypes.Message)
                {
                    var msg = a.AsMessageActivity();
                    _logger.LogInformation("{Tag} Incoming type={Type} from={FromId} conv={ConvId} text={Text}", Tag, a.Type, a.From?.Id, a.Conversation?.Id, msg?.Text);
                }
                else
                {
                    _logger.LogInformation("{Tag} Incoming type={Type} from={FromId} conv={ConvId}", Tag, a.Type, a.From?.Id, a.Conversation?.Id);
                }
            }

            turnContext.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                foreach (var outAct in activities)
                {
                    if (outAct.Type == ActivityTypes.Message)
                    {
                        var om = outAct.AsMessageActivity();
                        _logger.LogInformation("{Tag} Outgoing type={Type} to={RecipientId} conv={ConvId} text={Text}", Tag, outAct.Type, outAct.Recipient?.Id, outAct.Conversation?.Id, om?.Text);
                    }
                    else
                    {
                        _logger.LogInformation("{Tag} Outgoing type={Type} to={RecipientId} conv={ConvId}", Tag, outAct.Type, outAct.Recipient?.Id, outAct.Conversation?.Id);
                    }
                }
                return await nextSend();
            });

            await next(cancellationToken);
        }
    }
}
