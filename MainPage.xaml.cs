#if ANDROID

using WuDingCard.Platforms.Android;

#endif

namespace WuDingCard;

public partial class MainPage:ContentPage
{
	public MainPage() {
		InitializeComponent();
		StartService();
	}

	private async void ALOHA(object sender,EventArgs e) {
		await DisplayAlert("","欢迎使用本软件","确定","确定");
	}

	private static void StartService() {
#if ANDROID
		var intent = new Android.Content.Intent(Android.App.Application.Context,typeof(MyForegroundService));
		Android.App.Application.Context.StartForegroundService(intent);
#endif
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}