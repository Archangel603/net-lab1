using System.Text.Json;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Db;
using Server.MessageHandlers;
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


builder.RegisterType<AuthRequestHandler>();
builder.Register<MessageHandlerFactory>(c =>
    new MessageHandlerFactory(c.Resolve<IComponentContext>())
        .WithHandler<AuthRequestHandler>(MessageType.AuthRequest)
).SingleInstance();

builder.RegisterType<SocketClient>();
builder.RegisterType<SocketServer>().SingleInstance();

var container = builder.Build();
var server = container.Resolve<SocketServer>();

await server.Start(config.Port);