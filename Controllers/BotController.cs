using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Logging;

namespace SimpleGreetingBot.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class BotController : ControllerBase
    {
    private const string Tag = "[GreetingBot]";
        private readonly IBot _bot;
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly ILogger<BotController> _logger;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, ILogger<BotController> logger)
        {
            _adapter = adapter;
            _bot = bot;
            _logger = logger;
        }

        [HttpPost, HttpGet]
        public async Task PostAsync()
        {
            _logger.LogInformation("{Tag} Incoming request: {Method} {Path}", Tag, Request.Method, Request.Path);
            await _adapter.ProcessAsync(Request, Response, _bot);
            _logger.LogInformation("{Tag} Request processed: {StatusCode}", Tag, Response?.StatusCode);
        }
    }
}
