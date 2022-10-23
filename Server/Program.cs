using System.Text.Json;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Db;
using Server.RequestHandlers;
using Server.Services;
using Server.Socket;
using Shared.Message;

var configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

if (!File.Exists(configPath))
    throw new Exception("Config not found");

var config = JsonSerializer.Deserialize<ServerConfig>(File.ReadAllText(configPath));

var builder = new ContainerBuilder();

var dbOptionsBuilder = new DbContextOptionsBuilder()
    .UseNpgsql(config.ConnectionString);

builder.RegisterInstance(new ServerDbContext(dbOptionsBuilder.Options));

foreach (var handler in RequestExecutorFactory.FindRequestHandlers())
{
    var requestType = handler.GetInterfaces().First(i => i.IsGenericType && i.IsAssignableTo(typeof(IRequestHandler)));
    var messageType = requestType.GenericTypeArguments[0];

    builder.RegisterType(handler).As(requestType);
    
    RequestExecutorFactory.RegisterRequestHandler(messageType, requestType);
}
builder.Register<RequestExecutorFactory>(c => new RequestExecutorFactory(c.Resolve<IComponentContext>()));

builder.RegisterType<EventBus>().SingleInstance();
builder.RegisterType<ChatService>().SingleInstance();
builder.RegisterType<UserService>().SingleInstance();

builder.RegisterType<SocketClient>();
builder.RegisterType<SocketServer>().SingleInstance();

var container = builder.Build();
var server = container.Resolve<SocketServer>();

await server.Start(config.Port);