using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;
using SimpleGreetingBot.Middleware;

namespace SimpleGreetingBot
{
    // CloudAdapter (推奨) によるアダプタ
    public class AdapterWithErrorHandler : CloudAdapter
    {
    public AdapterWithErrorHandler(BotFrameworkAuthentication auth, ILogger<CloudAdapter> logger, ActivityLoggingMiddleware activityLogging)
            : base(auth, logger)
        {
            Use(activityLogging);
            OnTurnError = async (turnContext, exception) =>
            {
                var reqId = turnContext.Activity?.Id;
                var convId = turnContext.Activity?.Conversation?.Id;
                logger.LogError(exception, "[GreetingBot] OnTurnError req={ReqId} conv={ConvId} error={Message}", reqId, convId, exception.Message);
                await turnContext.SendActivityAsync("申し訳ありません、エラーが発生しました。");
            };
        }
    }
}
