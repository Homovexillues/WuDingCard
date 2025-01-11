namespace WuDingCard;

public partial class MainPage:ContentPage
{
	public MainPage() {
		InitializeComponent();
	}

	private async void ALOHA(object sender,EventArgs e) {
		await DisplayAlert("","欢迎使用本软件","确定","确定");
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}