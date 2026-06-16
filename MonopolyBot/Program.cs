using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MonopolyBot.Application.Service;
using MonopolyBot.Application.Services;
using MonopolyBot.Core;
using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.Repository;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.DataAccess.ApiClients.Clients;
using MonopolyBot.DataAccess.Postgres;
using MonopolyBot.DataAccess.Postgres.Repositories;
using MonopolyBot.DataAccess.Postgres.UnitsOfWork;
using MonopolyBot.Telegram.Handlers.Callback;
using MonopolyBot.Telegram.Handlers.Command;
using MonopolyBot.Telegram.Handlers.Status;
using MonopolyBot.Telegram.Interfaces.Callback;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Telegram.Interfaces.Services;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Telegram.Services;
using Telegram.Bot;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var configuration = builder.Build();

Constants.BotID = configuration["BotSettings:Token"];

Constants.ApiAddress = configuration["ApiConnectionString:ApiAddress"];
Constants.ApiAccouuntHost = configuration["ApiConnectionString:ApiAccountHost"];
Constants.ApiRoomHost = configuration["ApiConnectionString:ApiRoomHost"];
Constants.ApiGameHost = configuration["ApiConnectionString:ApiGameHost"];

Constants.DBConnect = configuration["DbConnectionStrings:DefaultConnection"];

var service = new ServiceCollection();

service.AddSingleton<IAccountClient, MonopolyClient>();
service.AddSingleton<IRoomClient, MonopolyClient>();
service.AddSingleton<IGameClient, MonopolyClient>();
service.AddSingleton<ITradingClient, MonopolyClient>();

service.AddDbContext<BotDbContext>(
    options =>
    {
        options.UseNpgsql(Constants.DBConnect);
    });

service.AddScoped<IChatStatusRepository, ChatStatusRepository>();
service.AddScoped<IUserRepository, UserRepository>();

service.AddScoped<IUnitOfWork, UnitOfWork>();

service.AddScoped<IAuthorizationService, AuthorizationService>();
service.AddScoped<IContextService, ContextService>();

service.AddScoped<IAccountService, AccountService>();
service.AddScoped<IRoomService, RoomService>();
service.AddScoped<IGameService, GameService>();
service.AddScoped<ITradingService, TradingService>();

service.AddScoped<IStartCommandHandler, StartCommandHandler>();
service.AddScoped<IAccountCommandHandler, AccountCommandHandler>();
service.AddScoped<IRoomCommandHandler, RoomCommandHandler>();
service.AddScoped<IGameCommandHandler, GameCommandHandler>();
service.AddScoped<ITradeCommandHandler, TradeCommandHandler >();

service.AddScoped<IAccountStatusHandler, AccountStatusHandler>();
service.AddScoped<IRoomStatusHandler, RoomStatusHandler>();
service.AddScoped<IGameStatusHandler, GameStatusHandler>();
service.AddScoped<ITradeStatusHandler, TradeStatusHandler>();

service.AddScoped<IRoomCallbackHandler, RoomCallbackHandler>();
service.AddScoped<IGameCallbackHandler, GameCallbackHandler>();
service.AddScoped<ITradeCallbackHandler, TradeCallbackHandler>();

service.AddSingleton<IMessageFormatter, MessageFormatter>();
service.AddSingleton<IBroadcastService, BroadcastService>();

service.AddSingleton<ITelegramBotClient>(new TelegramBotClient(Constants.BotID));
service.AddSingleton<MonopolyBot.MonopolyBot>();

var provider = service.BuildServiceProvider();
var monopolyBot = provider.GetRequiredService<MonopolyBot.MonopolyBot>();
await monopolyBot.StartAsync();