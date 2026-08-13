using Microsoft.Extensions.Logging;

using ReunionesDeAmigos.App.Services;

namespace ReunionesDeAmigos.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if ANDROID
		const string apiBaseUrl = "http://10.0.2.2:5080/";
#else
		const string apiBaseUrl = "http://localhost:5080/";
#endif

		builder.Services.AddSingleton(new HttpClient
		{
			BaseAddress = new Uri(apiBaseUrl)
		});
		builder.Services.AddSingleton<IAuthApiService, AuthApiService>();
		builder.Services.AddSingleton<ISessionService, SessionService>();
		builder.Services.AddSingleton<ISalidasApiService, SalidasApiService>();
		builder.Services.AddSingleton<ICiudadesApiService, CiudadesApiService>();
		builder.Services.AddSingleton<ILugaresApiService, LugaresApiService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
