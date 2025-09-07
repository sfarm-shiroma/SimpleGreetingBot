using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using SimpleGreetingBot;
using SimpleGreetingBot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with Newtonsoft.Json for Bot Framework payloads
builder.Services.AddControllers().AddNewtonsoftJson();

// Bot Framework auth and adapter
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBot, GreetingBot>();
builder.Services.AddSingleton<ActivityLoggingMiddleware>();

// HTTP ログ（リクエスト/レスポンス）
builder.Services.AddHttpLogging(o =>
{
	o.LoggingFields = HttpLoggingFields.Request | HttpLoggingFields.RequestHeaders |
					  HttpLoggingFields.Response | HttpLoggingFields.ResponseHeaders;
});

var app = builder.Build();

// アプリ ライフサイクル ログ
app.Lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Application started"));
app.Lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("Application stopping"));
app.Lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("Application stopped"));

app.UseHttpLogging();
app.UseRouting();

// 簡易ヘルスチェック
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
