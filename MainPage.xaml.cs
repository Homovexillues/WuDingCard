namespace WuDingCard;

public partial class MainPage:ContentPage
{
	public MainPage() {
		InitializeComponent();
	}

	private void OnSwipedHandler(object sender,SwipedEventArgs e) {
		App.HandleGlobalSwipe(e);
	}
}