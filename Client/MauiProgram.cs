using Microsoft.Extensions.Logging;
using Client.Data;
using Client.Services;
using Client.Socket;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS
using WinUIEx;
#endif

namespace Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
#if WINDOWS
			.ConfigureLifecycleEvents(events =>
			{
				events.AddWindows(windows =>
				{
					windows.OnWindowCreated((w) =>
					{
						w.MoveAndResize(w.Bounds.X, w.Bounds.Y, 800, 600);
					});
				});
			})
#endif
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddBlazorWebViewDeveloperTools();
		
#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<DialogService>();
		builder.Services.AddSingleton<SocketService>();
		builder.Services.AddScoped<EventSubManager>();

		return builder.Build();
	}
}
