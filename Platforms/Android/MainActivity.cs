using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace WuDingCard
{
	[Activity(Theme = "@style/Maui.SplashTheme",MainLauncher = true,LaunchMode = LaunchMode.SingleTop,ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity:MauiAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			// 设置状态栏颜色（以紫色为例，可替换为你想要的颜色）
			Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#009688")); // 这里填你想要的颜色
		}
	}
}