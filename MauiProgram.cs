using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Handlers.Items;
using SQLite;
using Syncfusion.Maui.Core.Hosting;

namespace WuDingCard
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp() {
			Log.Info("开始记录日志");
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureSyncfusionCore()
				.ConfigureFonts(fonts => {
					fonts.AddFont("OpenSans-Regular.ttf","OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf","OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();

#endif
			return builder.Build();
		}
	}
}